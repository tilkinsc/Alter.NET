using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Net.Codec.Login;

class LoginEncoder : MessageToByteEncoder<LoginResponse>
{
	
	protected override void Encode(IChannelHandlerContext ctx, LoginResponse msg, IByteBuffer output)
	{
		output.WriteByte(2);
		output.WriteByte(29);
		output.WriteByte(0);
		output.WriteInt(0);
		output.WriteByte(msg.Privilege);
		output.WriteBoolean(true);
		output.WriteShort(msg.Index);
		output.WriteBoolean(true);
		output.WriteLong(0);
		output.WriteLong(0);
	}
	
}