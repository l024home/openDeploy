using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.ConsoleHost;

class Program
{
    static async Task Main()
    {
        Logger.Write("我是客户端");

        {
            using var nettyClient = new NettyClient("127.0.0.1", 20007);

            for (int i = 0; i < 50; i++)
            {
                await Console.Out.WriteLineAsync();
                Logger.Write("测试消息 Before");
                await nettyClient.SendAsync("Echo/Print");
                Logger.Write("测试消息 After");
            }
        }

        Console.ReadLine();
    }
}
