using System.Windows;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.Models;
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

        DataContext = new MainViewModel();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void MainWindow_Activated(object? sender, EventArgs e)
    {
        
    }

    private void MainWindow_Deactivated(object? sender, EventArgs e)
    {
        
    }

}