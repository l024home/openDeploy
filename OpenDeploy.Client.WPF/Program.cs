using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenDeploy.Client.Models;
using OpenDeploy.Client.WPF;
using OpenDeploy.Infrastructure;
using OpenDeploy.Infrastructure.Extensions;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client;

class Program
{
    private static SplashScreen? _splashScreen;
    public static IHost AppHost { get; private set; } = default!;

    /// <summary> 应用程序的主入口点。 </summary>
    [STAThread]
    public static void Main()
    {
        //启动画面
        _splashScreen = new SplashScreen("/Resources/Images/OpenDeploy.png");
        _splashScreen.Show(autoClose: false, topMost: true);

        //依赖注入
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddLogging(options => { options.ClearProviders(); });
        builder.Services.AddSingleton<OpenDeployDbContext>();
        builder.Services.AddSingleton<SolutionRepository>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainWindow>();
        AppHost = builder.Build();

        //环境变量
        var env = AppHost.Services.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            Logger.Info(env.ToJsonString(true));
        }

        //应用程序声明周期
        var lifeTime = AppHost.Services.GetRequiredService<IHostApplicationLifetime>();
        lifeTime.ApplicationStarted.Register(() =>
        {
            Logger.Info("ApplicationStarted");
        });
        lifeTime.ApplicationStopping.Register(() =>
        {
            Logger.Info("ApplicationStopping");
        });
        lifeTime.ApplicationStopped.Register(() =>
        {
            Logger.Info("ApplicationStopped");
        });

        //启动应用程序
        var app = new App();
        app.Run();
    }

    public static void CloseSplashScreen()
    {
        if (_splashScreen != null)
        {
            _splashScreen.Close(TimeSpan.FromSeconds(0));
            _splashScreen = null;
        }
    }
}
