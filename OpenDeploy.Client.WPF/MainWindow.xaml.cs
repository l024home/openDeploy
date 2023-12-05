using System.Windows;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.Models;
using OpenDeploy.Client.Windows;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.WPF;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;

        DataContext = mainViewModel;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Logger.Info("MainWindow Loaded");
    }

}