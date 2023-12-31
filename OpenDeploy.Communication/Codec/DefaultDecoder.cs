﻿using System.Text;
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
        //消息总长度
        var totalLength = input.ReadableBytes;

        //消息头长度
        var headerLength = input.GetInt(input.ReaderIndex);

        //消息体长度
        var bodyLength = totalLength - 4 - headerLength; 

        //读取消息头字节数组
        var headerBytes = new byte[headerLength];
        input.GetBytes(input.ReaderIndex + 4, headerBytes, 0, headerLength);

        byte[]? bodyBytes = null;
        string? rawHeaderString = null;
        NettyHeader? header;

        try
        {
            //把消息头字节数组,反序列化为JSON
            rawHeaderString = Encoding.UTF8.GetString(headerBytes);
            header = JsonSerializer.Deserialize<NettyHeader>(rawHeaderString);
        }
        catch (Exception ex)
        {
            Logger.Error($"解码失败: {rawHeaderString}, {ex}");
            return;
        }

        if (header is null)
        {
            Logger.Error($"解码失败: {rawHeaderString}");
            return;
        }

        //读取消息体字节数组
        if (bodyLength > 0)
        {
            bodyBytes = new byte[bodyLength];
            input.GetBytes(input.ReaderIndex + 4 + headerLength, bodyBytes, 0, bodyLength);
        }

        //封装为NettyMessage对象
        var message = new NettyMessage
        {
            Header = header,
            Body = bodyBytes,
            RawHeaderString = rawHeaderString,
        };

        output.Add(message);
    }
}
