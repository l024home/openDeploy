using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;

namespace OpenDeploy.Client.Models;

/// <summary> 项目视图模型 </summary>
public partial class ProjectViewModel
{
    /// <summary> 项目名称 </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary> 项目所在文件夹 </summary>
    public string ProjectDir { get; set; } = string.Empty;

    /// <summary> 项目发布输出文件夹 </summary>
    public string ReleaseDir { get; set; } = string.Empty;

    /// <summary> 是否Web项目 </summary>
    public bool IsWeb { get; set; }






    /// <summary>
    /// 打开项目所在的文件夹
    /// </summary>
    [RelayCommand]
    private void OpenProjectDir()
    {
        try
        {
            Helper.ShellUtil.ExplorerFile(ProjectDir);
        }
        catch (Exception)
        {
            Growl.ErrorGlobal("打开项目所在的文件夹失败,可能被杀毒软件阻止了");
        }
    }
}
