using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.Demos;

public class NettyClientDemos
{
    public static async Task Test()
    {
        using var nettyClient = new NettyClient("127.0.0.1", 20007);
        for (int i = 0; i < 5; i++)
        {
            await Console.Out.WriteLineAsync();
            Logger.Info("发送消息 Before");
            await nettyClient.SendAsync("Echo/Print", true);
            Logger.Info("发送消息 After");
        }
    }
}
