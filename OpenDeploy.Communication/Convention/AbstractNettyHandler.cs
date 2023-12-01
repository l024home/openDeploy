using System.Reflection;
using System.Text.Json;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Communication.Convention;

/// <summary> Netty处理器约定抽象类 </summary>
public abstract class AbstractNettyHandler(NettyContext context)
{
    /// <summary> Netty上下文 </summary>
    protected NettyContext NettyContext { get; } = context;
    protected NettyRequest Request => NettyContext.Request;
    protected NettyResponse Response => NettyContext.Response;

    /// <summary> 处理请求 </summary>
    public virtual async Task ProcessAsync()
    {
        //通过ActionName反射获取方法
        var actionName = Request.ActionName;
        var bindAttr = BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreReturn;
        MethodInfo? mi = this.GetType().GetMethod(actionName, bindAttr);
        if (mi == null) return;

        //反射生成方法需要的参数
        object[]? parameters = null;
        ParameterInfo[] parameterInfos = mi.GetParameters();
        if (parameterInfos != null && parameterInfos.Length > 0)
        {
            var parameterInfo = parameterInfos[0];
            try
            {
                var parameter = JsonSerializer.Deserialize(Request.RawHeaderString, parameterInfo.ParameterType);
                if (parameter != null)
                {
                    parameters = [parameter];
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
            }
        }

        object? result;
        if (mi.IsStatic)
            result = mi.Invoke(null, parameters);
        else
            result = mi.Invoke(this, parameters);

        if (result is Task task)
        {
            await task;
        }

    }
}
