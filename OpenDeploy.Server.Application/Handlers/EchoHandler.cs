using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Server.Handlers;

/// <summary>
/// 回声处理器
/// </summary>
public class EchoHandler(NettyContext context) : AbstractNettyHandler(context)
{
    public async Task Print()
    {
        await Response.WriteAsync("Echo/Print");
    }
}
