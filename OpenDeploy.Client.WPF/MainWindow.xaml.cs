using System.Windows;
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

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Logger.Info("MainWindow Loaded");
        mainViewModel.InitSolutions();
    }

}