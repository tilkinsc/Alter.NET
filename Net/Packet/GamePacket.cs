using DotNetty.Buffers;

namespace Net.Packet;

class GamePacket
{
	
	public int Opcode { get; private set; }
	public PacketType Type { get; private set; }
	public IByteBuffer Payload { get; private set; }
	
	public int Length { get => Payload.ReadableBytes; }
	
	public GamePacket(int opcode, PacketType type, IByteBuffer payload)
	{
		Opcode = opcode;
		Type = type;
		Payload = payload;
	}
	
	public override string ToString()
	{
		return $"ToString not implemented";
	}
	
}