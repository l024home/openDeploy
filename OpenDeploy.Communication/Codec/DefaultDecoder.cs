using System.Text;
using System.Text.Json;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using OpenDeploy.Communication.Convention;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Communication.Codec;

/// <summary> 默认解码器 </summary>
public class DefaultDecoder : MessageToMessageDecoder<IByteBuffer>
{
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        var totalLength = input.ReadableBytes; //消息总长度
        var headerLength = input.GetInt(input.ReaderIndex); //消息头长度
        var bodyLength = totalLength - 4 - headerLength; //消息体长度

        var headerBytes = new byte[headerLength];
        input.GetBytes(input.ReaderIndex + 4, headerBytes, 0, headerLength);

        byte[]? bodyArray = null;
        string? rawHeaderString = null;
        Header? header;

        try
        {
            rawHeaderString = Encoding.UTF8.GetString(headerBytes);
            header = JsonSerializer.Deserialize<Header>(rawHeaderString);
        }
        catch (Exception ex)
        {
            Logger.Write($"解码失败: {rawHeaderString}, {ex}");
            return;
        }

        if (header is null)
        {
            Logger.Write($"解码失败: {rawHeaderString}");
            return;
        }

        if (bodyLength > 0)
        {
            bodyArray = new byte[bodyLength];
            input.GetBytes(input.ReaderIndex + 4 + headerLength, bodyArray, 0, bodyLength);
        }

        var message = NettyMessage.Create(header, bodyArray);

        output.Add(message);
    }
}
