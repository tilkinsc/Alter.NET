namespace Game.Model.Bits;

abstract class StorageBits
{
	public int StartBit;
	public int EndBit;
	
	public StorageBits(int startBit, int endBit)
	{
		StartBit = startBit;
		EndBit = endBit;
	}
}