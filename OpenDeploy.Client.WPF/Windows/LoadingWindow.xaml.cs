using System.Windows;

namespace OpenDeploy.Client.Windows;

public partial class LoadingWindow : Window
{
    public LoadingWindow()
    {
        InitializeComponent();
    }

    public LoadingWindow(Window owner)
    {
        InitializeComponent();
        Owner = owner;
        Width = owner.ActualWidth;
        Height = owner.ActualHeight;
    }
}
