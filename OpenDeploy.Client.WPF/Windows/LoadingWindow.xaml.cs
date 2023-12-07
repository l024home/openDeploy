using System.Windows;
using System.Windows.Media;

namespace OpenDeploy.Client.Windows;

public partial class LoadingWindow : Window
{
    /// <summary>
    /// 关闭超时时间/毫秒
    /// </summary>
    private readonly int closeTimeout = 0;

    public LoadingWindow(Window owner , int closeTimeout = 1000, bool tranparent = true)
    {
        InitializeComponent();

        if (!tranparent)
        {
            grid.Background = Brushes.White;
        }

        Owner = owner;
        Width = owner.ActualWidth;
        Height = owner.ActualHeight;
        this.closeTimeout = closeTimeout;
        Loaded += LoadingWindow_Loaded;
    }

    private async void LoadingWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (closeTimeout > 0)
        {
            await Task.Delay(closeTimeout);
        }
        Close();
    }
}
