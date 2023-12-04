namespace OpenDeploy.Communication.Convention;

/// <summary> Netty处理器选择器 </summary>
public interface INettyHandlerSelector
{
    AbstractNettyHandler SelectHandler(NettyContext context);
}
