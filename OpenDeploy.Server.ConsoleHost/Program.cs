using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenDeploy.Infrastructure;
using OpenDeploy.Infrastructure.Extensions;

namespace OpenDeploy.Server;

class Program
{
    

    static async Task Main()
    {
        Logger.Info("我是服务器");

        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddLogging(options => options.ClearProviders());
        builder.Services.Configure<List<ProjectConfig>>(builder.Configuration.GetSection("Projects"));
        var host = builder.Build();
        var server = new NettyServer(host);
        await host.StartAsync();
        await server.RunAsync();
        await host.StopAsync();

        Logger.Info("服务器进程关闭");
    }
}
