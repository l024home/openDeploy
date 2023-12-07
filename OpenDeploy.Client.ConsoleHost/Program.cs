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

        string[] solutionFiles = Directory.GetFiles(repoPath, "*.sln", SearchOption.AllDirectories);
        if (solutionFiles == null || solutionFiles.Length == 0)
        {
            throw new Exception("未找到解决方案");
        }
        string[] projectFilePaths = Directory.GetFiles(repoPath, "*.csproj", SearchOption.AllDirectories);
        if (projectFilePaths == null || projectFilePaths.Length == 0)
        {
            throw new Exception("未找到项目");
        }

        var solutionFilePath = solutionFiles[0];
        var solutionFileName = Path.GetFileName(solutionFilePath);
        var solutionDir = Path.GetDirectoryName(solutionFilePath);
        var solutionName = Path.GetFileNameWithoutExtension(solutionFileName);

        await Console.Out.WriteLineAsync(solutionFilePath);
        await Console.Out.WriteLineAsync(solutionDir);
        await Console.Out.WriteLineAsync(solutionFileName);
        await Console.Out.WriteLineAsync(solutionName);
        await Console.Out.WriteLineAsync();


        foreach (var projectFilePath in projectFilePaths)
        {
            var projectFileName = Path.GetFileName(projectFilePath);
            var projectDir = Path.GetDirectoryName(projectFilePath);
            var projectName = Path.GetFileNameWithoutExtension(projectFileName);

            var webConfigFiles = Directory.GetFiles(projectDir!, "web.config", SearchOption.TopDirectoryOnly);

            await Console.Out.WriteLineAsync(projectFilePath);
            await Console.Out.WriteLineAsync(projectDir);
            await Console.Out.WriteLineAsync(projectFileName);
            await Console.Out.WriteLineAsync(projectName);
            await Console.Out.WriteLineAsync(webConfigFiles.Length.ToString());
            await Console.Out.WriteLineAsync();
        }

        //GitDemos.GetChangesSinceLastPublish(repoPath);

        //HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        //builder.Services.AddDbContext<OpenDeployDbContext>();
        //builder.Services.AddTransient<SolutionRepository>();
        //builder.Services.AddHostedService<HostedService>();

        //using IHost host = builder.Build();

        //Logger.Info("After Build, RunAsync");

        //await host.RunAsync();

        Logger.Info("End");
    }
}
