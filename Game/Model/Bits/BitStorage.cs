using Game.Model.Attr;
using Game.Model.Entity;
using Util;

namespace Game.Model.Bits;

class BitStorage
{
	
	public AttributeKey<int> Key;
	
	public BitStorage(AttributeKey<int> key)
	{
		Key = key;
	}
	
	public BitStorage(string persistenceKey)
			: this(new AttributeKey<int>(persistenceKey))
	{
	}
	
	private int GetPackedBits(Pawn p)
	{
		object? attrib = p.Attributes[Key];
		if (attrib == null)
			return 0;
		return (int) attrib;
	}
	
	public int Get(Pawn p, StorageBits bits)
	{
		return BitManipulation.GetBit(GetPackedBits(p), bits.StartBit, bits.EndBit);
	}
	
	private void Set(Pawn p, int packed)
	{
		p.Attributes.Set(Key, packed);
	}
	
	public void Set(Pawn p, StorageBits bits, int value)
	{
		Set(p, BitManipulation.SetBit(GetPackedBits(p), bits.StartBit, bits.EndBit, value));
	}
	
}