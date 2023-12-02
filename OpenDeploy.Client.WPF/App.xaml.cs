using System.Windows;
using HandyControl.Controls;
using OpenDeploy.Client.Windows;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.WPF;

public partial class App : Application
{
    private readonly SplashWindow _splashWindow = new SplashWindow();

    public App()
    {
        _splashWindow.Show();
        Logger.Info($"Client 启动了...");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        MainWindow = new MainWindow();
        MainWindow.Show();
        _splashWindow.Close();
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
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
