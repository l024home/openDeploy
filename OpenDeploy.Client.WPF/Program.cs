using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenDeploy.Client.Models;
using OpenDeploy.Client.WPF;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client;

class Program
{
    private static SplashScreen? _splashScreen;

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
        builder.Services.AddDbContext<OpenDeployDbContext>();
        builder.Services.AddTransient<SolutionRepository>();
        builder.Services.AddTransient<SolutionViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainWindow>();
        var host = builder.Build();

        //启动应用程序
        var app = new App() { AppHost = host };
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
