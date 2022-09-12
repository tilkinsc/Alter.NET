using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Net.Packet;
using Util.IO;

namespace Net.Codec.Game;

class GamePacketEncoder : MessageToByteEncoder<GamePacket>
{
	
	private IsaacRandom? Random;
	
	public GamePacketEncoder(IsaacRandom? random)
	{
		Random = random;
	}
	
	protected override void Encode(IChannelHandlerContext ctx, GamePacket msg, IByteBuffer output)
	{
		if (msg.Type == PacketType.VARIABLE_BYTE && msg.Length >= 256) {
			return;
		} else if (msg.Type == PacketType.VARIABLE_SHORT && msg.Length >= 65536) {
			return;
		}
		output.WriteByte((byte) ((msg.Opcode + (Random != null ? Random.NextInt() : 0)) & 0xFF));
		switch (msg.Type)
		{
			case PacketType.VARIABLE_BYTE:
				output.WriteByte((byte) msg.Length);
				break;
			case PacketType.VARIABLE_SHORT:
				output.WriteByte((byte) (msg.Length << 8));
				output.WriteByte((byte) msg.Length);
				break;
		}
		
		if (msg.Opcode == 63) {
			for (int i=0; i<msg.Length; i++)
			{
				output.WriteByte((byte) (msg.Payload[i] + (Random != null ? Random.NextInt() : 0) & 0xFF));
			}
		} else {
			output.Write(msg.Payload, 0, msg.Payload.Length);
		}
	}
	
}