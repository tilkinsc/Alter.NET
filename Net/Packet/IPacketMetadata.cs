namespace Net.Packet;

interface IPacketMetadata
{
	public PacketType? GetType(int opcode);
	public int GetLength(int opcode);
	public bool ShouldIgnore(int opcode);
}