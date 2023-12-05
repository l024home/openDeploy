using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Hosting;
using OpenDeploy.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace OpenDeploy.Client.WPF;

public partial class App : Application
{
    public IHost AppHost { get; init; } = default!;

    public App()
    {
        InitializeComponent();
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        ShutdownMode = ShutdownMode.OnMainWindowClose;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
        MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
    }

    /// <summary>
    /// 全局异常捕获
    /// </summary>
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            e.Handled = true;
            Logger.Info(e.Exception.ToString());
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }
}
