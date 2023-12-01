using System.Collections.Concurrent;
using OpenDeploy.Communication.Convention;

namespace OpenDeploy.Client;

/// <summary> Netty消息同步器 </summary>
public static class NettyMessageSynchronizer
{
    /// <summary> 同步请求暂存器 </summary>
    private static readonly ConcurrentDictionary<Guid, TaskCompletionSource<NettyMessage>> _syncStore = new();

    /// <summary> 发送同步请求,并等待响应 </summary>
    public static async Task<NettyMessage> SendSync(NettyMessage message, NettyClient nettyClient)
    {
        var tcs = new TaskCompletionSource<NettyMessage>();
        NettyMessage response;
        if (_syncStore.TryAdd(message.Header.RequestId, tcs))
        {
            await nettyClient.SendAsync(message);
            response = await tcs.Task;
        }
        else
        {
            throw new Exception("SendSync Error: messageId exists");
        }
        return response;
    }

    public static void TrySetResult(NettyMessage response)
    {
        if (_syncStore.TryRemove(response.Header.RequestId, out var tcs))
        {
            tcs.TrySetResult(response);
        }
    }

    public static bool IsSync(NettyMessage response)
    {
        return _syncStore.ContainsKey(response.Header.RequestId);
    }
}
