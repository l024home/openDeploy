using OpenDeploy.Infrastructure;

namespace OpenDeploy.Server;

class Program
{
    static async Task Main()
    {
        Logger.Write("NettyServer开始启动");

        await NettyServer.RunAsync();

        Logger.Write("NettyServer进程关闭");
    }
}
