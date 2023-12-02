using System.Collections.Concurrent;
using OpenDeploy.Communication.Convention;

namespace OpenDeploy.Client;

/// <summary> Netty消息同步器 </summary>
public static class ClientMessageSynchronizer
{
    /// <summary> 同步请求暂存器 </summary>
    private static readonly ConcurrentDictionary<Guid, TaskCompletionSource<NettyMessage>> _syncStore = new();

    /// <summary> 保存同步请求 </summary>
    public static Task<NettyMessage> TryAdd(NettyMessage message)
    {
        var tcs = new TaskCompletionSource<NettyMessage>();
        if (_syncStore.TryAdd(message.Header.RequestId, tcs))
        {
            return tcs.Task;
        }
        throw new Exception("SendSync Error: messageId exists");
    }

    public static bool TryRemove(NettyMessage message)
    {
        return _syncStore.TryRemove(message.Header.RequestId, out _);
    }

    public static void TrySetResult(NettyMessage message)
    {
        if (_syncStore.TryRemove(message.Header.RequestId, out var tcs))
        {
            tcs.TrySetResult(message);
        }
    }

    public static bool IsSync(NettyMessage message)
    {
        return _syncStore.ContainsKey(message.Header.RequestId);
    }
}
