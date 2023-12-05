using OpenDeploy.Client.Models;

namespace OpenDeploy.Client.Dialogs;

/// <summary>
/// SolutionConfigDialog.xaml 的交互逻辑
/// </summary>
public partial class SolutionConfigDialog
{
    public SolutionConfigDialog(MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}
