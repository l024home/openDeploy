﻿using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenDeploy.Client.Models;
using OpenDeploy.Client.WPF;
using OpenDeploy.Infrastructure;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client;

class Program
{
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    public static void Main()
    {
        Logger.Info("Start Main");

        //启动画面
        new SplashScreen("/Resources/Images/OpenDeploy.png").Show(true, true);

        Logger.Info("Before Build");

        //依赖注入
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddLogging(options => { options.ClearProviders(); });
        builder.Services.AddDbContext<OpenDeployDbContext>();
        builder.Services.AddTransient<SolutionRepository>();
        builder.Services.AddTransient<SolutionViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainWindow>();
        IHost host = builder.Build();

        Logger.Info("After Build");

        //启动应用程序
        var app = new App() { AppHost = host };
        app.Run();

        Logger.Info("After Run");
    }
}
