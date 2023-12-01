using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Server.Handlers;

/// <summary>
/// 回声处理器
/// </summary>
public class EchoHandler(NettyContext nettyContext) : AbstractNettyHandler(nettyContext)
{
    public async Task Print()
    {
        var response = NettyMessage.Create("Echo/Print");
        await NettyContext.Response.WriteAsync(response);
    }
}
