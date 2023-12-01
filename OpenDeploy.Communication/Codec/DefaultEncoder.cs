using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using OpenDeploy.Communication.Convention;

namespace OpenDeploy.Communication.Codec;

/// <summary> 默认编码器 </summary>
public class DefaultEncoder : MessageToByteEncoder<NettyMessage>
{
    protected override void Encode(IChannelHandlerContext context, NettyMessage message, IByteBuffer output)
    {
        //消息头转为字节数组
        var headerBytes = message.GetHeaderBytes();

        //写入消息头长度
        output.WriteInt(headerBytes.Length);

        //写入消息头字节数组
        output.WriteBytes(headerBytes);

        //写入消息体字节数组
        if (message.Body != null && message.Body.Length > 0)
        {
            output.WriteBytes(message.Body);
        }
    }
}
