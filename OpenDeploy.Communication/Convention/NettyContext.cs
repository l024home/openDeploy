using DotNetty.Transport.Channels;

namespace OpenDeploy.Communication.Convention;

/// <summary> Netty连接上下文 </summary>
public class NettyContext
{
    /// <summary> Netty通道 </summary>
    public IChannel Channel { get; init; }

    /// <summary> Netty请求封装 </summary>
    public NettyRequest Request { get; init; }

    /// <summary> Netty响应封装 </summary>
    public NettyResponse Response { get; init; }

    public NettyContext(IChannel channel, NettyMessage message)
    {
        Channel = channel;
        Request = new NettyRequest(this, message);
        Response = new NettyResponse(this);
    }
}
