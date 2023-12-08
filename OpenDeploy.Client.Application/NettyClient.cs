using System.Net;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using OpenDeploy.Communication.Codec;
using OpenDeploy.Communication.Convention;

namespace OpenDeploy.Client;

/// <summary> Netty客户端 </summary>
public sealed class NettyClient(string serverHost, int serverPort) : IDisposable
{
    public EndPoint ServerEndPoint { get; } = new IPEndPoint(IPAddress.Parse(serverHost), serverPort);

    private static readonly Bootstrap bootstrap = new();
    private static readonly IEventLoopGroup eventLoopGroup = new SingleThreadEventLoop();

    private bool _disposed;
    private IChannel? _channel;
    public bool IsConnected => _channel != null && _channel.Open;
    public bool IsWritable => _channel != null && _channel.IsWritable;

    static NettyClient()
    {
        bootstrap
            .Group(eventLoopGroup)
            .Channel<TcpSocketChannel>()
            .Option(ChannelOption.SoReuseaddr, true)
            .Option(ChannelOption.SoReuseport, true)
            .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                //pipeline.AddLast("ping", new IdleStateHandler(0, 5, 0));
                pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                pipeline.AddLast("decoder", new DefaultDecoder());
                pipeline.AddLast("encoder", new DefaultEncoder());
                pipeline.AddLast("handler", new ClientMessageEntry());
            }));
    }

    /// <summary> 连接服务器 </summary>
    private async Task TryConnectAsync()
    {
        try
        {
            if (IsConnected) { return; }
            _channel = await bootstrap.ConnectAsync(ServerEndPoint);
        }
        catch (Exception ex)
        {
            throw new Exception($"连接服务器失败 : {ServerEndPoint} {ex.Message}");
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
        await SendAsync(message);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public async Task SendAsync(NettyMessage message)
    {
        if (message.IsSync())
        {
            var task = ClientMessageSynchronizer.TryAdd(message);
            try
            {
                await ConnectAndSendAsync(message);
                await task;
            }
            catch
            {
                ClientMessageSynchronizer.TryRemove(message);
                throw;
            }
        }
        else
        {
            await ConnectAndSendAsync(message);
        }
    }

    private async Task ConnectAndSendAsync(NettyMessage message)
    {
        await TryConnectAsync();
        await _channel!.WriteAndFlushAsync(message);
    }

    /// <summary> 释放连接(程序员手动释放, 一般在代码使用using语句,或在finally里面Dispose) </summary>
    public void Dispose()
    {
        Dispose(true); 
        GC.SuppressFinalize(this);
    }

    /// <summary> 释放连接 </summary>
    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        //释放托管资源,比如嵌套的对象
        if (disposing)
        {
            
        }

        //释放非托管资源
        if (_channel != null)
        {
            _channel.CloseAsync();
            _channel = null;
        }

        _disposed = true;
    }

    ~NettyClient()
    {
        Dispose(true);
    }
}
