using System.Reflection.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenDeploy.Client.Demos;
using OpenDeploy.Infrastructure;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client.ConsoleHost;

class HostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.Info("StartAsync");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.Info("StopAsync");
        return Task.CompletedTask;
    }
}

class Program
{
    static async Task Main()
    {
        Logger.Info("我是客户端");

        GitDemos.GetChangesSinceLastPublish();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        builder.Services.AddDbContext<OpenDeployDbContext>();
        builder.Services.AddTransient<SolutionRepository>();
        builder.Services.AddHostedService<HostedService>();

        using IHost host = builder.Build();

        Logger.Info("After Build, RunAsync");

        await host.RunAsync();

        Logger.Info("End");
    }
}
