namespace Game.Model.Item;

class SlotItem
{
	public int Slot;
	public Item Item;
	
	public SlotItem(int slot, Item item)
	{
		Slot = slot;
		Item = item;
	}
}