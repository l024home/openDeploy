using System.Windows;
using System.Windows.Threading;
using HandyControl.Controls;
using OpenDeploy.Client.Windows;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.WPF;

public partial class App : Application
{
    public App()
    {
        Logger.Info($"App() ...");
        InitializeComponent();
        DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Logger.Info($"App.OnStartup...");
        MainWindow.Show();
        Logger.Info($"MainWindow.Show...");
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            e.Handled = true;
            Logger.Info(e.Exception.ToString());
            Growl.ErrorGlobal("捕获未处理异常:" + e.Exception.Message);
        }
        catch (Exception ex)
        {
            Growl.ErrorGlobal("程序发生致命错误，将终止，请联系运营商！:" + ex.Message);
        }
    }
}
