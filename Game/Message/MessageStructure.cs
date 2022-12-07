using Net.Packet;

namespace Game.Message;

class MessageStructure : IEquatable<MessageStructure>
{
	
	public PacketType Type;
	public int[] Opcodes;
	public int Length;
	public bool Ignore;
	public Dictionary<string, MessageValue> Values;
	
	public MessageStructure(PacketType type, int[] opcodes, int length, bool ignore, Dictionary<string, MessageValue> values)
	{
		Type = type;
		Opcodes = opcodes;
		Length = length;
		Ignore = ignore;
		Values = values;
	}
	
	public override bool Equals(object? other)
	{
		return Equals(other as MessageStructure);
	}
	
	public bool Equals(MessageStructure? other)
	{
		if (other == null)
			return false;
		
		// TODO: doesn't check if contents is equal of Values
		if (Type != other.Type || Length != other.Length || Ignore != other.Ignore || Values != other.Values)
			return false;
		return true;
	}
	
	public override int GetHashCode()
	{
		int hash = Type.GetHashCode();
		// TODO: gets the hash code instead of contents hash code
		hash = 31 * hash + Opcodes.GetHashCode();
		hash = 31 * hash + Length;
		hash = 31 * hash + Ignore.GetHashCode();
		// TODO: gets the hash code instead of contents hash code
		hash = 31 * hash + Values.GetHashCode();
		return hash;
	}
	
	
}