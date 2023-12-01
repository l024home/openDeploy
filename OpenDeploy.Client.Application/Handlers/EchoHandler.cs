using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.Handlers;

/// <summary>
/// 客户端回声处理器
/// </summary>
public class EchoHandler(NettyContext nettyContext) : AbstractNettyHandler(nettyContext)
{
    public void Print()
    {
        
    }
}
