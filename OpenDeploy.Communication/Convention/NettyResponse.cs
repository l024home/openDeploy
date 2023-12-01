namespace OpenDeploy.Communication.Convention;

/// <summary> Netty响应封装 </summary>
public class NettyResponse(NettyContext context, NettyMessage nettyMessage)
{
    public NettyContext NettyContext { get; } = context;
    public NettyMessage RequestMessage { get; } = nettyMessage;

    public async Task WriteAsync(string endpoint, byte[]? body = null)
    {
        var response = new NettyMessage
        {
            Header = new NettyHeader
            {
                RequestId = RequestMessage.Header.RequestId,
                Sync = RequestMessage.Header.Sync,
                EndPoint = endpoint,
            },
            Body = body
        };
        await NettyContext.Channel.WriteAndFlushAsync(response);
    }
}