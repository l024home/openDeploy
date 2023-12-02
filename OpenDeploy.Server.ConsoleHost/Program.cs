using OpenDeploy.Infrastructure;

namespace OpenDeploy.Server;

class Program
{
    static async Task Main()
    {
        Logger.Info("我是服务器");

        await NettyServer.RunAsync();

        Logger.Info("服务器进程关闭");
    }
}
