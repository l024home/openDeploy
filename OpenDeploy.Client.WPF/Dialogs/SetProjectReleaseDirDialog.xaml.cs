using OpenDeploy.Client.Models;

namespace OpenDeploy.Client.Dialogs;

/// <summary>
/// 设置项目发布路径弹窗
/// </summary>
public partial class SetProjectReleaseDirDialog
{
    public SetProjectReleaseDirDialog(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;
    }
}
