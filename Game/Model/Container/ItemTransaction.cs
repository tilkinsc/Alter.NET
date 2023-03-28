using System.Collections;
using Game.Model.Item;

namespace Game.Model.Container;

class ItemTransaction : IEnumerable<SlotItem>
{
	public int Requested;
	public int Completed;
	public List<SlotItem> Items;
	
	public ItemTransaction(int requested, int completed, List<SlotItem> items)
	{
		Requested = requested;
		Completed = completed;
		Items = items;
	}
	
	public int GetLeftOver() => Requested - Completed;
	public bool HasSucceeded() => Completed == Requested;
	public bool HasFailed() => !HasSucceeded();
	
	public IEnumerator<SlotItem> GetEnumerator()
	{
		return Items.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return Items.GetEnumerator();
	}
	
}