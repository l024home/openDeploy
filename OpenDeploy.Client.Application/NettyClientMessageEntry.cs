using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using OpenDeploy.Client.Handlers;
using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client;

/// <summary> 客户端消息入口 </summary>
public class NettyClientMessageEntry : ChannelHandlerAdapter
{
    private readonly DefaultNettyHandlerSelector handlerSelector = new();

    public NettyClientMessageEntry()
    {
        //注册Netty处理器
        handlerSelector.RegisterHandlerTypes(typeof(EchoHandler).Assembly.GetTypes());
    }

    /// <summary> 通道激活 </summary>
    public override void ChannelActive(IChannelHandlerContext context)
    {
        Logger.Write($"ChannelActive: {context.Channel}");
    }

    /// <summary> 通道关闭 </summary>
    public override void ChannelInactive(IChannelHandlerContext context)
    {
        Logger.Write($"ChannelInactive: {context.Channel}");
    }

    /// <summary>
    /// 收到服务器的消息
    /// </summary>
    public override async void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is not NettyMessage nettyMessage)
        {
            Logger.Write("从服务器接收消息为空");
            return;
        }

        Logger.Write($"收到服务器的消息: {nettyMessage}");

        //处理同步消息
        if (NettyMessageSynchronizer.IsSync(nettyMessage))
        {
            NettyMessageSynchronizer.TrySetResult(nettyMessage);
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

    public override void ChannelReadComplete(IChannelHandlerContext context)
    {
        context.Flush();
    }

    /// <summary>
    ///  异常处理
    /// </summary>
    public override async void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Logger.Write($"Client ExceptionCaught: {exception}");
        await context.CloseAsync();
    }
}