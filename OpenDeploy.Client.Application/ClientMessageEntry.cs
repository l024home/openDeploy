using DotNetty.Transport.Channels;
using OpenDeploy.Client.Handlers;
using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client;

/// <summary> 客户端消息入口 </summary>
public class ClientMessageEntry : ChannelHandlerAdapter
{
    private readonly DefaultNettyHandlerSelector handlerSelector = new();

    public ClientMessageEntry()
    {
        //注册Netty处理器
        handlerSelector.RegisterHandlerTypes(typeof(EchoHandler).Assembly.GetTypes());
    }

    /// <summary> 通道激活 </summary>
    public override void ChannelActive(IChannelHandlerContext context)
    {
        Logger.Warn($"ChannelActive: {context.Channel}");
    }

    /// <summary> 通道关闭 </summary>
    public override void ChannelInactive(IChannelHandlerContext context)
    {
        Logger.Warn($"ChannelInactive: {context.Channel}");
    }

    /// <summary>
    /// 收到服务器的消息
    /// </summary>
    public override async void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is not NettyMessage nettyMessage)
        {
            Logger.Error("从服务器接收消息为空");
            return;
        }

        try
        {
            Logger.Info($"收到服务器的消息: {nettyMessage}");

            //处理同步消息
            if (nettyMessage.IsSync())
            {
                ClientMessageSynchronizer.TrySetResult(nettyMessage);
            }
            else
            {
                //处理异步消息
                var handler = handlerSelector.SelectHandler(new NettyContext(context.Channel, nettyMessage));
                if (handler != null)
                {
                    await handler.ProcessAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }

    }
}