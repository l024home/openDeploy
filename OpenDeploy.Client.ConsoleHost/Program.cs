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

            for (int i = 0; i < 5; i++)
            {
                await Console.Out.WriteLineAsync();
                Logger.Write("测试同步消息 Before");
                await nettyClient.SendAndRecieveAsync(NettyMessage.Create("Echo/Print", true));
                Logger.Write("测试同步消息 After");
                await Console.Out.WriteLineAsync();
            }
        }

        Console.ReadLine();
    }
}
