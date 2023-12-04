using OpenDeploy.Infrastructure.Extensions;

namespace OpenDeploy.Communication.Convention;

/// <summary> Netty消息头 </summary>
public class NettyHeader
{
    /// <summary> 请求消息唯一标识 </summary>
    public Guid RequestId { get; init; } = Guid.NewGuid();

    /// <summary> 是否同步消息, 默认false是异步消息 </summary>
    public bool Sync { get; init; }

    /// <summary> 终结点 (借鉴MVC,约定为Control/Action模式) </summary>
    public string EndPoint { get; init; } = string.Empty;

    /// <summary> 序列化为JSON字符串 </summary>
    public override string ToString() => this.ToJsonString();
}
