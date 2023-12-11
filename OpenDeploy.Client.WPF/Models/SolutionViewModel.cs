using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Channels;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using OpenDeploy.Client.Dialogs;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.WPF;
using OpenDeploy.Communication.Convention;
using OpenDeploy.Domain.Convention;
using OpenDeploy.Domain.NettyHeaders;
using OpenDeploy.Infrastructure;
using OpenDeploy.Infrastructure.Extensions;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client.Models;

/// <summary> 解决方案视图模型 </summary>
public partial class SolutionViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid solutionId;

    /// <summary> 解决方案名称 </summary>
    [ObservableProperty]
    private string solutionName = string.Empty;

    /// <summary> 解决方案Git仓储路径 </summary>
    [ObservableProperty]
    public string gitRepositoryPath = string.Empty;

    /// <summary> 解决方案上次发布时间 </summary>
    [ObservableProperty]
    private string lastPublishTime = string.Empty;

    /// <summary> 首次发布Git提交ID </summary>
    [ObservableProperty]
    private string firstPublishGitCommitId = string.Empty;

    /// <summary> 项目列表 </summary>
    [ObservableProperty]
    private List<ProjectViewModel> projects = [];

    /// <summary> 自上次发布以来的改动 </summary>
    [ObservableProperty]
    private List<PatchEntryChanges>? changesSinceLastCommit;

    /// <summary> 待发布的文件 </summary>
    [ObservableProperty]
    private List<DeployFileInfo>? publishFiles;

    /// <summary> 是否首次发布(首次发布需要人工操作) </summary>
    [ObservableProperty]
    private bool firstRelease;

    /// <summary> Web项目视图模型 </summary>
    private ProjectViewModel? webProject = default!;

    /// <summary> 解决方案仓储 </summary>
    private SolutionRepository solutionRepo => Program.AppHost.Services.GetRequiredService<SolutionRepository>();

    /// <summary> 一键发布解决方案弹窗 </summary>
    private Dialog? quickDeployDialog;

    /// <summary> 打开一键发布弹窗 </summary>
    [RelayCommand]
    public async Task OpenQuickDeploySolutionDialog()
    {
        webProject = Projects.FirstOrDefault(a => a.IsWeb);
        if (webProject == null)
        {
            Growl.ClearGlobal();
            Growl.ErrorGlobal("暂未发现Web项目,默认规则是必须带web.config的才是Web项目");
            return;
        }
        if (string.IsNullOrEmpty(webProject.ReleaseDir))
        {
            Growl.ClearGlobal();
            Growl.ErrorGlobal("请配置Web项目的发布目录");
            return;
        }

        //获取上次发布记录
        var lastPublish = await solutionRepo.GetLastPublishAsync(SolutionId);

        //没有发布过,本次将执行首次发布
        if (lastPublish == null)
        {
            FirstRelease = true;
            LastPublishTime = "暂无发布记录";
        }
        else
        {
            FirstRelease = false;
            LastPublishTime = lastPublish.PublishTime.ToString("yyyy-MM-dd HH:mm:ss");

            //获取自上次发布以来的改动
            var changes = GitHelper.GetChangesSinceLastPublish(GitRepositoryPath, lastPublish?.GitCommitId);
            if (changes.IsEmpty())
            {
                Growl.WarningGlobal("暂无提交记录");
                return;
            }
            ChangesSinceLastCommit = changes;

            //从Git变化解析出待发布的文件
            var files = GetPublishFiles(changes.Select(a => a.Path.Replace("/", "\\")));
            PublishFiles = files;
        }

        quickDeployDialog = Dialog.Show(new QuickDeployDialog(this));
    }

    /// <summary> 确定发布 </summary>
    [RelayCommand]
    private async Task OkPublishSolution()
    {
        //首次发布
        if (FirstRelease)
        {
            await RunFirstPublishAsync();
            return;
        }

        await RunPublishAsync();
    }

    /// <summary>
    /// 首次人工发布,只记录提交Id
    /// </summary>
    private async Task RunFirstPublishAsync()
    {
        if (string.IsNullOrEmpty(FirstPublishGitCommitId))
        {
            Growl.ClearGlobal();
            Growl.WarningGlobal($"请输入Git提交ID");
            return;
        }
        if (!GitHelper.ExistsCommit(GitRepositoryPath, FirstPublishGitCommitId))
        {
            Growl.ClearGlobal();
            Growl.WarningGlobal($"请输入正确的Git提交ID");
            return;
        }
        //保存首次人工发布记录
        await solutionRepo.SaveFirstPublishAsync(SolutionId, SolutionName, FirstPublishGitCommitId);
        Growl.SuccessGlobal($"操作成功");
        quickDeployDialog?.Close();
    }

    /// <summary>
    /// 非首次发布
    /// </summary>
    private async Task RunPublishAsync()
    {
        if (PublishFiles == null || PublishFiles.Count == 0)
        {
            Growl.ClearGlobal();
            Growl.WarningGlobal("没有需要发布的文件");
            return;
        }

        foreach (var file in PublishFiles)
        {
            if (!File.Exists(file.PublishFileAbsolutePath))
            {
                Growl.ClearGlobal();
                Growl.ErrorGlobal($"文件不存在,请检查发布目录是否包含该文件,请确认项目包含了该文件,重新编译后重试: \n {file.PublishFileAbsolutePath}");
                return;
            }
        }

        var loading = Loading.Show();

        //待发布文件打包zip
        var zipResult = await ZipHelper.CreateZipAsync(PublishFiles.Select(a => new ZipFileInfo(a.PublishFileAbsolutePath, a.PublishFileRelativePath)));

        try
        {
            //读取zip字节数组,填充到 NettyMessage 的 Body
            var body = await File.ReadAllBytesAsync(zipResult.FullFileName);

            //NettyHeader
            var header = new DeployRequestHeader()
            {
                Files = PublishFiles,
                SolutionName = SolutionName,
                ProjectName = webProject!.ProjectName,
                ZipFileName = zipResult.FileName,
            };

            var nettyMessage = new NettyMessage { Header = header, Body = body };

            //创建 NettyClient
            Logger.Info("开始发送");
            using var nettyClient = new NettyClient("127.0.0.1", 20007);
            await nettyClient.SendAsync(nettyMessage);
            Logger.Info("完成发送");

            Growl.SuccessGlobal($"发布成功");
            quickDeployDialog?.Close();
        }
        catch (Exception ex)
        {
            Growl.ClearGlobal();
            Growl.ErrorGlobal(ex.Message);
        }
        finally
        {
            _ = Task.Run(async () =>
            {
                ShellUtil.ExplorerFile(zipResult.FullFileName);
                await Task.Delay(1000);
                File.Delete(zipResult.FullFileName);
            });
            loading.Close();
        }
    }


    /// <summary>
    /// 从Git修改记录提取出待发布文件
    /// </summary>
    private List<DeployFileInfo> GetPublishFiles(IEnumerable<string> changedFilePaths)
    {
        var fileInfos = new List<DeployFileInfo>(changedFilePaths.Count());
        foreach (string changedPath in changedFilePaths)
        {
            var fi = DeployFileInfo.Create(changedPath);
            if (fi.IsUnKnown) continue;
            fileInfos.Add(fi);
        }
        foreach (var fi in fileInfos)
        {
            fi.ChangedFileAbsolutePath = Path.Combine(GitRepositoryPath, fi.ChangedFileRelativePath);

            //所属项目
            var project = Projects
                .Where(a => fi.ChangedFileRelativePath.Contains(a.ProjectName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (project == null) continue;

            fi.ProjectName = project.ProjectName;
            if (fi.IsDLL)
            {
                fi.FileName = $"{project.ProjectName}.dll";
                fi.PublishFileRelativePath = $"bin\\{fi.FileName}";
            }
            else
            {
                fi.PublishFileRelativePath = fi.ChangedFileAbsolutePath.Replace(project.ProjectDir, "").TrimStart(Path.DirectorySeparatorChar);
            }
            fi.PublishFileAbsolutePath = Path.Combine(webProject!.ReleaseDir, fi.PublishFileRelativePath);

            //Logger.Info(fi.ToJsonString(true));
        }
        //按照 PublishFileAbsolutePath 去重
        return fileInfos.Distinct(new DeployFileInfoComparer()).ToList();
    }

    #region Git相关命令

    /// <summary> gitk </summary>
    [RelayCommand]
    public async Task RunGitK()
    {
        await RunGitCommand("gitk");
    }

    /// <summary> git status </summary>
    [RelayCommand]
    public async Task RunGitStatus()
    {
        await RunGitCommand("git status");
    }

    /// <summary> git add . </summary>
    [RelayCommand]
    public async Task RunGitAdd()
    {
        await RunGitCommand("git add .");
    }

    /// <summary> 打开 git commit 窗口 </summary>
    [RelayCommand]
    public void OpenGitCommit()
    {
        GitCommitOptionsVisibility = Visibility.Visible;
        GitCommitMessage = string.Empty;
    }
    /// <summary> 执行 git commit </summary>
    [RelayCommand]
    public async Task RunGitCommit()
    {
        if (string.IsNullOrEmpty(GitCommitMessage))
        {
            _ = HandyControl.Controls.MessageBox.Show("请输入提交描述");
            return;
        }
        await RunGitCommand($"git commit -am \"{GitCommitMessage}\"");
        GitCommitMessage = string.Empty;
        GitCommitOptionsVisibility = Visibility.Hidden;
    }

    /// <summary> git pull origin master </summary>
    [RelayCommand]
    public async Task RunGitPull()
    {
        await RunGitCommand("git pull origin master");
    }

    /// <summary> git push origin master </summary>
    [RelayCommand]
    public async Task RunGitPush()
    {
        await RunGitCommand("git push origin master");
    }

    /// <summary> git 命令输出 </summary>
    [ObservableProperty]
    private string logText = string.Empty;

    /// <summary> git commit 显示控制 </summary>
    [ObservableProperty]
    private Visibility gitCommitOptionsVisibility = Visibility.Hidden;

    /// <summary> git commit message </summary>
    [ObservableProperty]
    private string gitCommitMessage = string.Empty;

    /// <summary> 执行git命令 </summary>
    private async Task RunGitCommand(string cmd)
    {
        var loading = Loading.Show();
        string output = string.Empty;
        LogText = string.Empty;

        await Task.Run(() =>
        {
            var _process = new Process();
            _process.StartInfo.WorkingDirectory = GitRepositoryPath;
            _process.StartInfo.FileName = "cmd.exe";
            _process.StartInfo.Arguments = "/C " + cmd;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.Start();//启动程序

            output = _process.StandardOutput.ReadToEnd();

            if (string.IsNullOrEmpty(output))
            {
                output = _process.StandardError.ReadToEnd();
                if (string.IsNullOrEmpty(output))
                {
                    output = "没有返回值";
                }
            }

            _process.WaitForExit();
            _process.Close();
        });

        LogText = output;
        loading.Close();
    }

    #endregion
}
