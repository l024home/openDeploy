using OpenDeploy.Infrastructure;

namespace OpenDeploy.Communication.Convention;

/// <summary> Netty响应封装 </summary>
public class NettyResponse(NettyContext context, NettyMessage nettyMessage)
{
    public NettyContext NettyContext { get; } = context;
    public NettyMessage RequestMessage { get; } = nettyMessage;

    /// <summary> 向客户端写消息 </summary>
    public async Task WriteAsync(string endpoint, byte[]? body = null)
    {
        try
        {
            var IsWritable = NettyContext.Channel.IsWritable;
            if (IsWritable)
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
            else
            {
                Logger.Error($"NettyResponse.WriteAsync: IsWritable:{IsWritable} {NettyContext.Channel}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"NettyResponse.WriteAsync: {ex}");
        }
    }
}