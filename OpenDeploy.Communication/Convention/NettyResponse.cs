namespace OpenDeploy.Communication.Convention;

/// <summary> Netty响应封装 </summary>
public class NettyResponse(NettyContext context)
{
    public NettyContext NettyContext { get; } = context;

    public async Task WriteAsync(NettyMessage message)
    {
        message.Header.RequestId = NettyContext.Request.Header.RequestId;
        message.Header.Sync = NettyContext.Request.Header.Sync;
        await NettyContext.Channel.WriteAndFlushAsync(message);
    }
}