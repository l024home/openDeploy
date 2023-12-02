using DotNetty.Transport.Channels;
using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;
using OpenDeploy.Server.Handlers;

namespace OpenDeploy.Server;

/// <summary> 服务端消息入口 </summary>
public class ServerMessageEntry : ChannelHandlerAdapter
{
    /// <summary> Netty处理器选择器 </summary>
    private readonly DefaultNettyHandlerSelector handlerSelector = new();

    public ServerMessageEntry()
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

    /// <summary> 收到客户端的消息 </summary>
    public override async void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is not NettyMessage nettyMessage)
        {
            Logger.Error("从客户端接收消息为空");
            return;
        }

        try
        {
            Logger.Info($"收到客户端的消息: {nettyMessage}");

            //封装请求
            var nettyContext = new NettyContext(context.Channel, nettyMessage);

            //选择处理器
            AbstractNettyHandler handler = handlerSelector.SelectHandler(nettyContext);

            //处理请求
            await handler.ProcessAsync();
        }
        catch(Exception ex)
        {
            Logger.Error($"ServerMessageEntry.ChannelRead: {ex}");
        }
    }
}
