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

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="endpoint">终结点</param>
    /// <param name="sync">是否同步等待响应</param>
    /// <param name="body">正文</param>
    public async Task SendAsync(string endpoint, bool sync = false, byte[]? body = null)
    {
        var message = NettyMessage.Create(endpoint, sync, body);
        if (sync)
        {
            await NettyMessageSynchronizer.SendSync(message, this);
        }
        else
        {
            await SendAsync(message);
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public async Task SendAsync(NettyMessage message)
    {
        await TryConnectAsync();
        await _channel!.WriteAndFlushAsync(message);
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
