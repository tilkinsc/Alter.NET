using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Net.Codec.Filestore;

class FilestoreEncoder : MessageToByteEncoder<FilestoreResponse>
{
	
	protected override void Encode(IChannelHandlerContext ctx, FilestoreResponse msg, IByteBuffer output)
	{
		output.WriteByte(msg.Index);
		output.WriteShort(msg.Archive);
		foreach (byte b in msg.Data)
		{
			if (output.WriterIndex % 512 == 0) {
				output.WriteByte(-1);
			}
			output.WriteByte(b);
		}
	}
	
}