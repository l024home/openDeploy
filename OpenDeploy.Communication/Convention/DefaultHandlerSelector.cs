using System.Reflection;

namespace OpenDeploy.Communication.Convention;


/// <summary> Netty处理器选择器 </summary>
public interface INettyHandlerSelector
{
    AbstractNettyHandler SelectHandler(NettyContext context);
}

/// <summary> 默认的Netty处理器选择器 </summary>
public class DefaultNettyHandlerSelector : INettyHandlerSelector
{
    /// <summary>
    /// 所有的Netty处理器
    /// </summary>
    private readonly List<Type> handlers = [];

    /// <summary>
    /// 注册Netty处理器
    /// </summary>
    public void RegisterHandlerTypes(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            if (typeof(AbstractNettyHandler).IsAssignableFrom(type))
            {
                handlers.Add(type);
            }
        }
    }

    /// <summary>
    /// 选择处理器
    /// </summary>
    public virtual AbstractNettyHandler SelectHandler(NettyContext context)
    {
        //基于约定, 选择类型名称前缀是 ControllerName 的
        Type? handlerType =
            handlers.FirstOrDefault(a => a.Name.StartsWith(context.Request.ControllerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception("404: Handler not find");

        if (Activator.CreateInstance(handlerType, [context]) is not AbstractNettyHandler handler)
        {
            throw new Exception("404: Handler not find");
        }

        return handler;
    }
}
