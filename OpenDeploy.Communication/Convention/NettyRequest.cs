namespace OpenDeploy.Communication.Convention;

/// <summary> Netty请求封装 </summary>
public class NettyRequest
{
    /// <summary> 保存NettyContext的引用 </summary>
    public NettyContext NettyContext { get; init; }

    /// <summary> 请求的消息 </summary>
    public NettyMessage Message { get; init; }

    /// <summary> 请求的消息头 </summary>
    public Header Header { get; init; }

    /// <summary> 请求的消息体 </summary>
    public byte[]? Body { get; init; }

    /// <summary> 请求的消息头的原始JSON字符串 </summary>
    public string RawHeaderString { get; init; }

    /// <summary> 控制器名称 </summary>
    public string ControllerName { get; init; }

    /// <summary> 方法名称 </summary>
    public string ActionName { get; init; }

    public NettyRequest(NettyContext context, NettyMessage message)
    {
        NettyContext = context;
        Message = message;
        RawHeaderString = message.Header.ToString();
        Header = message.Header;
        Body = message.Body;

        var splitResult = message.Header.EndPoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (splitResult.Length != 2)
        {
            throw new Exception("Invalid EndPoint");
        }
        ControllerName = splitResult[0];
        ActionName = splitResult[1];
    }
}
