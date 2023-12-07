using System.IO;
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
    public int id;

    /// <summary> 项目名称 </summary>
    [ObservableProperty]
    public string projectName = string.Empty;

    /// <summary> 项目所在文件夹 </summary>
    [ObservableProperty]
    public string projectDir = string.Empty;

    /// <summary> 项目发布输出文件夹 </summary>
    [ObservableProperty]
    public string releaseDir = string.Empty;

    /// <summary> 是否Web项目 </summary>
    [ObservableProperty]
    public bool isWeb;






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
    /// 设置项目发布路径 - 打开弹窗
    /// </summary>
    [RelayCommand]
    private void OpenSetProjectReleaseDirDialog()
    {
        setProjectReleaseDirDialog = Dialog.Show(new SetProjectReleaseDirDialog(this));
    }

    /// <summary>
    /// 设置项目发布路径
    /// </summary>
    [RelayCommand]
    private async Task OkSetProjectReleaseDir()
    {
        if (string.IsNullOrEmpty(ReleaseDir) || !Directory.Exists(ReleaseDir))
        {
            Growl.ClearGlobal();
            Growl.ErrorGlobal("请正确设置项目发布路径");
            return;
        }

        var solutionRepo = Program.AppHost.Services.GetRequiredService<SolutionRepository>();
        await solutionRepo.UpdateProjectReleaseDir(Id, ReleaseDir);

        setProjectReleaseDirDialog?.Close();

        Growl.SuccessGlobal("操作成功");
    }


}
