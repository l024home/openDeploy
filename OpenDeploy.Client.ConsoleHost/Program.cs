using System.Reflection.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenDeploy.Client.Demos;
using OpenDeploy.Domain.Models;
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
    static string repoPath = "D:\\Projects\\Back\\dotnet\\Study\\OpenDeploy.TestWebProject";
    static string id = "fd67a7c800ce2f305fd11ace7aae85a5a4554b99";

    static async Task Main()
    {
        Logger.Info("我是客户端");

        var lastCommit = GitHelper.GetLastCommit(repoPath);
        if (lastCommit != null)
        {
            await Console.Out.WriteLineAsync(lastCommit.Id.ToString());
            await Console.Out.WriteLineAsync(lastCommit.Sha);
            await Console.Out.WriteLineAsync(lastCommit.MessageShort);
        }

        Logger.Info("End");
    }
}
