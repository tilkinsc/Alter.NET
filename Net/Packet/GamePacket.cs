namespace Net.Packet;

class GamePacket
{
	
	public int Opcode;
	public PacketType Type;
	public byte[] Payload;
	
	public int Length { get => Payload.Length; }
	
	public GamePacket(int opcode, PacketType type, byte[] payload)
	{
		Opcode = opcode;
		Type = type;
		Payload = payload;
	}
	
}