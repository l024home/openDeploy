using DotNetty.Transport.Channels;

namespace OpenDeploy.Communication.Convention;

/// <summary> Netty连接上下文, 相当于MVC里面的HttpContext </summary>
public class NettyContext
{
    /// <summary> Netty通道 </summary>
    public IChannel Channel { get; }

    /// <summary> Netty请求封装 </summary>
    public NettyRequest Request { get; }

    /// <summary> Netty响应封装 </summary>
    public NettyResponse Response { get; }

    public NettyContext(IChannel channel, NettyMessage message)
    {
        Channel = channel;
        Request = new NettyRequest(this, message);
        Response = new NettyResponse(this, message);
    }
}
