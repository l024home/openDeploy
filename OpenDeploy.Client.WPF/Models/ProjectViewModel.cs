using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using Microsoft.Extensions.DependencyInjection;
using OpenDeploy.Client.Dialogs;
using OpenDeploy.Client.WPF;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client.Models;

/// <summary> 项目视图模型 </summary>
public partial class ProjectViewModel : ObservableObject
{
    /// <summary> 项目Id </summary>
    [ObservableProperty]
    private Guid projectId;

    /// <summary> 项目名称 </summary>
    [ObservableProperty]
    private string projectName = string.Empty;

    /// <summary> 项目所在文件夹 </summary>
    [ObservableProperty]
    private string projectDir = string.Empty;

    /// <summary> 项目发布输出文件夹 </summary>
    [ObservableProperty]
    private string releaseDir = string.Empty;

    /// <summary> 是否Web项目 </summary>
    [ObservableProperty]
    private bool isWeb;

    /// <summary> Web项目发布对应的服务器IP地址(自动发布部署的服务器地址) </summary>
    [ObservableProperty]
    private string serverIp = string.Empty;

    /// <summary> 自动发布部署的服务器端口 </summary>
    [ObservableProperty]
    private int serverPort = 20007;


    /// <summary>
    /// 打开项目目录
    /// </summary>
    [RelayCommand]
    private void OpenProjectDir()
    {
        try
        {
            Helper.ShellUtil.ExplorerFile(ProjectDir);
        }
        catch (Exception ex)
        {
            Growl.ErrorGlobal($"打开项目目录失败,可能被杀毒软件阻止了: {ex}");
        }
    }

    /// <summary>
    /// 打开项目发布目录
    /// </summary>
    [RelayCommand]
    private void OpenProjectReleaseDir()
    {
        if (string.IsNullOrEmpty(ReleaseDir))
        {
            Growl.ClearGlobal();
            Growl.WarningGlobal("请配置项目发布目录");
            return;
        }
        try
        {
            Helper.ShellUtil.ExplorerFile(ReleaseDir);
        }
        catch (Exception ex)
        {
            Growl.ErrorGlobal($"打开项目发布目录失败,可能被杀毒软件阻止了: {ex}");
        }
    }

    /// <summary> 设置项目发布路径弹窗 </summary>
    private Dialog? setProjectReleaseDirDialog;

    /// <summary>
    /// 设置项目发布信息 - 打开弹窗
    /// </summary>
    [RelayCommand]
    private void OpenSetProjectReleaseDirDialog()
    {
        setProjectReleaseDirDialog = Dialog.Show(new SetProjectReleaseDirDialog(this));
    }

    /// <summary>
    /// 设置项目发布信息
    /// </summary>
    [RelayCommand]
    private async Task OkSetProjectReleaseDir()
    {
        try
        {
            ThrowIfConfigIsInvalid();
        }
        catch (Exception ex)
        {
            Growl.ClearGlobal();
            Growl.WarningGlobal(ex.Message);
            return;
        }

        var solutionRepo = Program.AppHost.Services.GetRequiredService<SolutionRepository>();
        await solutionRepo.UpdateProjectReleaseInfo(ProjectId, ReleaseDir, ServerIp, ServerPort);

        setProjectReleaseDirDialog?.Close();

        Growl.SuccessGlobal("操作成功");
    }

    /// <summary>
    /// 检查项目配置
    /// </summary>
    public void ThrowIfConfigIsInvalid()
    {
        if (string.IsNullOrEmpty(ReleaseDir) || !Directory.Exists(ReleaseDir))
        {
            throw new Exception("请正确设置Web项目的发布路径");
        }

        if (string.IsNullOrEmpty(ServerIp) || !IPAddress.TryParse(ServerIp, out _))
        {
            throw new Exception("请正确设置Web项目的服务器的IP地址");
        }

        if (ServerPort <= 0 || ServerPort > 65535)
        {
            throw new Exception("请正确设置Web项目对应的自动发布工具在服务器的端口");
        }
    }
}
