using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using OpenDeploy.Communication.Codec;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Server;

/// <summary>
/// Netty服务器提供者
/// </summary>
public static class NettyServer
{
    /// <summary>
    /// 开启Netty服务
    /// </summary>
    public static async Task RunAsync(int port = 20007)
    {
        var bossGroup = new MultithreadEventLoopGroup(1);
        var workerGroup = new MultithreadEventLoopGroup();

        try
        {
            var bootstrap = new ServerBootstrap().Group(bossGroup, workerGroup);

            bootstrap
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .Option(ChannelOption.SoReuseaddr, true)
                .Option(ChannelOption.SoReuseport, true)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast("decoder", new DefaultDecoder());
                    pipeline.AddLast("encoder", new DefaultEncoder());
                    pipeline.AddLast("handler", new ServerMessageEntry());
                }));

            var boundChannel = await bootstrap.BindAsync(port);

            Logger.Info($"NettyServer启动成功...{boundChannel}");

            Console.ReadLine();

            await boundChannel.CloseAsync();

            Logger.Info($"NettyServer关闭监听了...{boundChannel}");
        }
        finally
        {
            await Task.WhenAll(
                bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
            );

            Logger.Info($"NettyServer退出了...");
        }

    }
}
