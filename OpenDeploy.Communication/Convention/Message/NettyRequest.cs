namespace OpenDeploy.Communication.Convention;

/// <summary> Netty请求封装 </summary>
public class NettyRequest
{
    /// <summary> 保存NettyContext的引用 </summary>
    public NettyContext NettyContext { get; }

    /// <summary> 请求的消息头 </summary>
    public NettyHeader Header { get; }

    /// <summary> 请求的消息体 </summary>
    public byte[]? Body { get; }

    /// <summary> 请求的消息头的原始JSON字符串 </summary>
    public string RawHeaderString { get; }

    /// <summary> 控制器名称 </summary>
    public string ControllerName { get; }

    /// <summary> 方法名称 </summary>
    public string ActionName { get; }

    public NettyRequest(NettyContext context, NettyMessage message)
    {
        NettyContext = context;
        Header = message.Header;
        Body = message.Body;
        RawHeaderString = message.RawHeaderString;

        var splitResult = message.Header.EndPoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (splitResult.Length != 2)
        {
            throw new Exception("Invalid EndPoint");
        }
        ControllerName = splitResult[0];
        ActionName = splitResult[1];
    }
}
