using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Exceptions;
using Net.Packet;
using Util.IO;

namespace Net.Codec.Game;

class GamePacketDecoder : StatefulFrameDecoder<GameDecoderState>
{
	
	private int Opcode = 0;
	private int Length = 0;
	private PacketType Type = PacketType.FIXED;
	private bool Ignore = false;
	
	private IsaacRandom? Random;
	private IPacketMetadata PacketMetadata;
	
	public GamePacketDecoder(IsaacRandom? random, IPacketMetadata packetMetadata)
			: base(GameDecoderState.OPCODE)
	{
		Random = random;
		PacketMetadata = packetMetadata;
	}
	
	private void DecodeOpcode(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output)
	{
		if (!buf.IsReadable())
			return;
		
		Opcode = buf.ReadByte() - (Random != null ? Random.NextInt() : 0) & 0xFF;
		PacketType? packetType = PacketMetadata.GetType(Opcode);
		if (packetType == null) {
			buf.SkipBytes(buf.ReadableBytes);
			return;
		}
		Type = (PacketType) packetType;
		Ignore = PacketMetadata.ShouldIgnore(Opcode);
		
		switch (Type)
		{
			case PacketType.FIXED:
				Length = PacketMetadata.GetLength(Opcode);
				if (Length != 0) {
					SetState(GameDecoderState.PAYLOAD);
				} else if (!Ignore) {
					output.Add(new GamePacket(Opcode, Type, Unpooled.Empty));
				}
				break;
			case PacketType.VARIABLE_BYTE:
			case PacketType.VARIABLE_SHORT:
				SetState(GameDecoderState.LENGTH);
				break;
			default:
				throw new IllegalStateException($"Unhandled packet type {Type} for opcode {Opcode}");
		}
	}
	
	private void DecodeLength(IByteBuffer buf, List<object> output)
	{
		if (!buf.IsReadable())
			return;
		
		Length = Type == PacketType.VARIABLE_SHORT ? buf.ReadUnsignedShort() : buf.ReadByte();
		if (Length != 0) {
			SetState(GameDecoderState.PAYLOAD);
		} else if (!Ignore) {
			output.Add(new GamePacket(Opcode, Type, Unpooled.Empty));
		}
	}
	
	private void DecodePayload(IByteBuffer buf, List<object> output)
	{
		if (buf.ReadableBytes < Length)
			return;
		IByteBuffer payload = buf.ReadBytes(Length);
		SetState(GameDecoderState.OPCODE);
		
		if (!Ignore) {
			output.Add(new GamePacket(Opcode, Type, payload));
		}
	}
	
	public override void Decode(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output, GameDecoderState state)
	{
		switch (state)
		{
			case GameDecoderState.OPCODE:
				DecodeOpcode(ctx, buf, output);
				break;
			case GameDecoderState.LENGTH:
				DecodeLength(buf, output);
				break;
			case GameDecoderState.PAYLOAD:
				DecodePayload(buf, output);
				break;
		}
	}
	
}