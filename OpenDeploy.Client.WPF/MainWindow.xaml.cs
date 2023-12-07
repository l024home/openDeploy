using System.Windows;
using HandyControl.Tools.Extension;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.Models;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.WPF;

public partial class MainWindow : System.Windows.Window
{
    private readonly MainViewModel mainViewModel;

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        DataContext = mainViewModel;
        this.mainViewModel = mainViewModel;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Program.CloseSplashScreen();

        await mainViewModel.InitAsync();

        LoadingPlaceHolder.Hide();
    }

}