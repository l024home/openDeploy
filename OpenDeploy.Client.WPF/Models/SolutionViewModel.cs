using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using OpenDeploy.Client.Dialogs;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.WPF;
using OpenDeploy.Domain.Convention;
using OpenDeploy.Infrastructure;
using OpenDeploy.Infrastructure.Extensions;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client.Models;

/// <summary> 解决方案视图模型 </summary>
public partial class SolutionViewModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    /// <summary> 解决方案名称 </summary>
    [ObservableProperty]
    private string solutionName = string.Empty;

    /// <summary> 解决方案Git仓储路径 </summary>
    [ObservableProperty]
    public string gitRepositoryPath = string.Empty;

    /// <summary> 解决方案上次发布时间 </summary>
    [ObservableProperty]
    private string lastPublishTime = string.Empty;

    /// <summary> 项目列表 </summary>
    [ObservableProperty]
    private List<ProjectViewModel> projects = [];

    /// <summary> 自上次发布以来的改动 </summary>
    [ObservableProperty]
    private List<PatchEntryChanges>? changesSinceLastCommit;

    /// <summary> 待发布的文件 </summary>
    [ObservableProperty]
    private List<DeployFileInfo>? publishFiles;

    /// <summary> 是否执行全量发布 </summary>
    [ObservableProperty]
    private bool fullRelease;

    /// <summary> Web项目视图模型 </summary>
    private ProjectViewModel? webProject = default!;




    /// <summary> 一键发布解决方案弹窗 </summary>
    private Dialog? quickDeployDialog;

    /// <summary> 打开一键发布弹窗 </summary>
    [RelayCommand]
    public void OpenQuickDeploySolutionDialog()
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
        var solutionRepo = Program.AppHost.Services.GetRequiredService<SolutionRepository>();
        var lastPublishCommit = solutionRepo.GetLastPublishCommit(Id);
        LastPublishTime = lastPublishCommit != null ? lastPublishCommit.PublishTime.ToString("yyyy-MM-dd HH:mm:ss") : "暂无发布记录";

        //没有发布过,本次将执行全量发布
        if (lastPublishCommit == null)
        {
            FullRelease = true;
        }
        else
        {
            //获取自上次发布以来的改动
            var changes = GitHelper.GetChangesSinceLastPublish(GitRepositoryPath, lastPublishCommit?.GitCommitId);
            if (changes.IsEmpty())
            {
                Growl.WarningGlobal("暂无提交记录");
                return;
            }
            ChangesSinceLastCommit = changes;

            //解析出需要发布的文件
            var files = GetPublishFiles(changes.Select(a => Path.Combine(GitRepositoryPath, a.Path.Replace("/", "\\"))));
            PublishFiles = files;
        }

        quickDeployDialog = Dialog.Show(new QuickDeployDialog(this));
    }



    /// <summary> 确定发布 </summary>
    [RelayCommand]
    private void OkPublishSolution()
    {
        var releaseDir = webProject!.ReleaseDir;

        if (FullRelease)
        {
            Growl.InfoGlobal($"全量发布: {releaseDir}");

            var zipPath = Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.zip");

            ZipFile.CreateFromDirectory(releaseDir, zipPath);

            ShellUtil.ExplorerFile(zipPath);

            return;
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
            //所属项目
            var project = Projects
                .Where(a => fi.ChangedFilePath.Contains(a.ProjectName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (project == null) continue;
            fi.ProjectName = project.ProjectName;
            if (fi.IsDLL)
            {
                fi.FileName = $"{project.ProjectName}.dll";
                fi.RelativeFilePath = $"bin\\{fi.FileName}";
            }
            else
            {
                fi.RelativeFilePath = fi.ChangedFilePath.Replace(project.ProjectDir, "").TrimStart(Path.DirectorySeparatorChar);
            }
            fi.AbsoluteFilePath = Path.Combine(project.ReleaseDir, fi.RelativeFilePath);
        }
        //按照 AbsoluteFilePath 去重
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
