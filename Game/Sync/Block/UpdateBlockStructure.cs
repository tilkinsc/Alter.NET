namespace Game.Sync.Block;

class UpdateBlockStructure
{
	public int Bit;
	public List<MessageValue> Values;
	
	public UpdateBlockStructure(int bit, List<MessageValue> values)
	{
		Bit = bit;
		Values = values;
	}
}