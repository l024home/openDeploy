using System.Net;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using OpenDeploy.Communication.Codec;
using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client;

/// <summary> Netty客户端 </summary>
public sealed class NettyClient(string serverHost, int serverPort) : IDisposable
{
    public EndPoint ServerEndPoint { get; } = new IPEndPoint(IPAddress.Parse(serverHost), serverPort);

    private static readonly Bootstrap bootstrap = new();
    private static readonly IEventLoopGroup eventLoopGroup = new SingleThreadEventLoop();

    private IChannel? _channel;
    public bool IsConnected => _channel != null && _channel.Active;

    static NettyClient()
    {
        bootstrap
            .Group(eventLoopGroup)
            .Channel<TcpSocketChannel>()
            .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                //pipeline.AddLast("ping", new IdleStateHandler(0, 5, 0));
                pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                pipeline.AddLast("decoder", new DefaultDecoder());
                pipeline.AddLast("encoder", new DefaultEncoder());
                pipeline.AddLast("handler", new NettyClientMessageEntry());
            }));
    }

    /// <summary> 连接服务器 </summary>
    private async Task TryConnectAsync()
    {
        try
        {
            if (IsConnected) { return; }
            _channel = await bootstrap.ConnectAsync(ServerEndPoint);
            Logger.Write("连接服务器成功");
        }
        catch (Exception ex)
        {
            throw new Exception($"连接服务器失败 : {ServerEndPoint} {ex}");
        }
    }

    /// <summary> 发送异步消息(不等待服务器返回) </summary>
    public async Task SendAsync(NettyMessage message)
    {
        await TryConnectAsync();
        await _channel!.WriteAndFlushAsync(message);
    }

    /// <summary> 发送同步消息(等待服务器返回) </summary>
    public Task<NettyMessage> SendAndRecieveAsync(NettyMessage message)
    {
        if (!message.Header.Sync)
        {
            throw new Exception("请在 NettyMessage.Create 中指定消息类型为同步");
        }
        return NettyMessageSynchronizer.SendSync(message, this);
    }

    /// <summary>
    /// 释放连接
    /// </summary>
    public void Dispose()
    {
        if (_channel != null)
        {
            _channel.CloseAsync();
            _channel = null;
        }
    }
}
