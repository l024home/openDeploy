using System.Windows;
using OpenDeploy.Client.Helper;
using OpenDeploy.Client.Windows;

namespace OpenDeploy.Client.WPF;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void BtnTestLoading_Click(object sender, RoutedEventArgs e)
    {
        Loading.Show();

        await Task.Delay(1000);
        
        Loading.Hide();
    }
}