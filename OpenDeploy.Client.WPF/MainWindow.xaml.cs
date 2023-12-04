using System.Windows;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.Windows;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.WPF;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;
        Activated += MainWindow_Activated;
        Deactivated += MainWindow_Deactivated;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Logger.Info($"MainWindow.Loaded...");
    }

    private void MainWindow_Activated(object? sender, EventArgs e)
    {
        Logger.Info($"MainWindow_Activated...{IsActive}");
    }

    private void MainWindow_Deactivated(object? sender, EventArgs e)
    {
        Logger.Info($"MainWindow_Deactivated...{IsActive}");
    }


    private void BtnTestLoading_Click(object sender, RoutedEventArgs e)
    {
        Loading.Show();
    }
}