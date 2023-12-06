using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.Windows;
using OpenDeploy.Domain.Models;
using OpenDeploy.Infrastructure;

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

    /// <summary> 清空模型 </summary>
    public void Clear()
    {
        Id = 0;
        SolutionName = string.Empty;
        GitRepositoryPath = string.Empty;
    }

    /// <summary> 映射到领域模式(用于持久化到数据库) </summary>
    public Solution Map2Entity()
    {
        var solution = new Solution()
        {
            SolutionName = SolutionName,
            GitRepositoryPath = GitRepositoryPath
        };
        return solution;
    }


    /// <summary> 打开一键发布弹窗 </summary>
    [RelayCommand]
    public void OpenQuickDeploySolutionDialog()
    {
        Growl.InfoGlobal("打开一键发布弹窗");
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
            _process.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
            _process.StartInfo.CreateNoWindow = true;//不显示程序窗口
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
