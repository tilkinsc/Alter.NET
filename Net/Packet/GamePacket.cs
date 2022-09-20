using DotNetty.Buffers;

namespace Net.Packet;

class GamePacket
{
	
	public int Opcode;
	public PacketType Type;
	public IByteBuffer Payload;
	
	public int Length { get => Payload.ReadableBytes; }
	
	public GamePacket(int opcode, PacketType type, IByteBuffer payload)
	{
		Opcode = opcode;
		Type = type;
		Payload = payload;
	}
	
}