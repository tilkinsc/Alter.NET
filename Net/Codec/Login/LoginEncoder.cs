using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Net.Codec.Login;

class LoginEncoder : MessageToByteEncoder<LoginResponse>
{
	
	protected override void Encode(IChannelHandlerContext ctx, LoginResponse msg, IByteBuffer output)
	{
		BinaryWriter stream = new BinaryWriter(output);
		stream.Write((byte) 2);
		stream.Write((byte) 29);
		stream.Write((byte) 0);
		stream.Write((int) 0);
		stream.Write(true);
		stream.Write((short) msg.Index);
		stream.Write(true);
		stream.Write((long) 0);
		stream.Write((long) 0);
	}
	
}