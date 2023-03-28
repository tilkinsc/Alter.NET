using System.Collections;
using Game.FS;
using Game.FS.Def;
using Game.Model.Container.Key;
using Game.Model.Item;

namespace Game.Model.Container;

class ItemContainer : IEnumerable<Item.Item?>
{
	
	public DefinitionSet Definitions { get; private set; }
	public ContainerKey Key { get; private set; }
	
	private ContainerStackType _stackType { get => Key.StackType; }
	private List<Item.Item?> _items;
	public int Capacity { get => Key.Capacity; }
	public bool Dirty = true;
	public List<Item.Item?> RawItems { get => _items; }
	
	public ItemContainer(DefinitionSet definitions, ContainerKey key)
	{
		Definitions = definitions;
		Key = key;
		_items = new List<Item.Item?>(Capacity);
		_items.AddRange(Enumerable.Repeat((Item.Item?) null, _items.Capacity));
	}
	
	public ItemContainer(DefinitionSet definitions, int capacity, ContainerStackType stackType)
			: this(definitions, new ContainerKey("", capacity, stackType))
	{
	}
	
	public ItemContainer(ItemContainer other)
			: this(other.Definitions, other.Capacity, other._stackType)
	{
		for (int i=0; i<Capacity; i++)
		{
			Item.Item? item = _items.ElementAtOrDefault(i);
			if (item == null)
				item = new Item.Item(other[i]!);
			_items[i] = item;
		}
	}
	
	public Item.Item? this[int i] => _items[i];
	
	public bool Contains(int itemId) => _items.Any(it => it?.ID == itemId);
	
	public bool ContainsAny(params int[] items)
	{
		foreach (int itemId in items)
		{
			if (Contains(itemId))
				return true;
		}
		return false;
	}
	
	public bool HasAt(int slot, int itemId) => _items[slot]?.ID == itemId;
	
	public int NextFreeSlot { get => _items.IndexOf(null); }
	
	public int FreeSlotCount
	{
		get
		{
			int count = 0;
			for (int i=0; i<_items.Count; i++)
			{
				if (_items[i] == null)
					count++;
			}
			return count;
		}
	}
	
	public int getLastFreeSlot()
	{
		int lastEmpty = -1;
		for (int i=_items.Count-1; i>=0; i--)
		{
			if (_items[i] != null)
				break;
			lastEmpty = i;
		}
		return lastEmpty;
	}
	
	public int OccupiedSlotCount
	{
		get
		{
			int count = 0;
			for (int i=0; i<_items.Count; i++)
			{
				if (_items[i] != null)
					count++;
			}
			return count;
		}
	}
	
	public bool IsFull => _items.IndexOf(null) == -1;
	
	public bool IsEmpty
	{
		get
		{
			for (int i=0; i<_items.Count; i++)
			{
				if (_items[i] != null)
					return false;
			}
			return true;
		}
	}
	
	public bool HasAny
	{
		get
		{
			for (int i=0; i<_items.Count; i++)
			{
				if (_items[i] != null)
					return true;
			}
			return false;
		}
	}
	
	public bool HasSpace => NextFreeSlot != -1;
	
	public int GetItemCount(int itemId)
	{
		long amount = 0;
		foreach (Item.Item? item in _items)
		{
			if (item == null)
				continue;
			amount += item.ID == itemId ? item.Amount : 0;
		}
		return (int) Math.Min(Int32.MaxValue, amount);
	}
	
	public int GetItemIndex(int itemId, bool skipAttribItems)
	{
		for (int i=0; i<Capacity; i++)
		{
			Item.Item? item = _items[i];
			if (item == null)
				continue;
			if (item.ID == itemId && (!skipAttribItems || !item.HasAttributes()))
				return i;
		}
		return -1;
	}
	
	public Dictionary<int, Item.Item> ToMap()
	{
		Dictionary<int, Item.Item> map = new Dictionary<int, Item.Item>();
		foreach (Item.Item? item in _items)
		{
			if (item == null)
				continue;
			map.Add(item.ID, item);
		}
		return map;
	}
	
	public void RemoveAll()
	{
		for (int i=0; i<_items.Count; i++)
			_items[i] = null;
	}
	
	public ItemTransaction Add(int item, int amount = 1, bool assureFullInsertion = true, bool forceNoStack = false, int beginSlot = -1)
	{
		ItemDef? def = Definitions.Get<ItemDef>(item);
		
		bool shouldStack = !forceNoStack && _stackType != ContainerStackType.NO_STACK && (def.IsStackable || _stackType == ContainerStackType.STACK);
		int previousAmount = shouldStack ? GetItemCount(item) : 0;
		
		if (previousAmount == Int32.MaxValue)
			return new ItemTransaction(amount, 0, new List<SlotItem>());
		
		int freeSlotCount = FreeSlotCount;
		if (freeSlotCount == 0 && (!shouldStack || shouldStack && previousAmount == 0))
			return new ItemTransaction(amount, 0, new List<SlotItem>());
		
		if (assureFullInsertion)
		{
			if (shouldStack)
			{
				if (previousAmount > Int32.MaxValue - amount)
					return new ItemTransaction(amount, 0, new List<SlotItem>());
			}
			else
			{
				if (amount > freeSlotCount)
					return new ItemTransaction(amount, 0, new List<SlotItem>());
			}
		}
		else
		{
			if (shouldStack)
			{
				if (previousAmount == Int32.MaxValue)
					return new ItemTransaction(amount, 0, new List<SlotItem>());
			}
			else
			{
				if (freeSlotCount == 0)
					return new ItemTransaction(amount, 0, new List<SlotItem>());
			}
		}
		
		int completed = 0;
		List<SlotItem> added = new List<SlotItem>();
		
		if (!shouldStack)
		{
			int startSlot = Math.Max(0, beginSlot);
			for (int i=startSlot; i<Capacity; i++)
			{
				if (_items[i] != null)
					continue;
				Item.Item add = new Item.Item(item);
				_items[i] = add;
				added.Add(new SlotItem(i, add));
				if (++completed >= amount)
					break;
			}
		}
		else
		{
			int stackIndex = GetItemIndex(item, true);
			if (stackIndex == -1)
			{
				if (beginSlot == -1)
				{
					stackIndex = NextFreeSlot;
				}
				else
				{
					for (int i=beginSlot; i<Capacity; i++)
					{
						if (_items[i] == null)
						{
							stackIndex = i;
							break;
						}
					}
				}
				if (stackIndex == -1)
					return new ItemTransaction(amount, completed, new List<SlotItem>());
			}
			
			int stackAmount = _items[stackIndex]?.Amount ?? 0;
			int total = (int) Math.Min((long) Int32.MaxValue, (long) stackAmount + amount);
			Item.Item add = new Item.Item(item, total);
			_items[stackIndex] = add;
			added.Add(new SlotItem(stackIndex, add));
			completed = total - stackAmount;
		}
		return new ItemTransaction(amount, completed, added);
	}
	
	public ItemTransaction Add(Item.Item item, bool assureFullInsertion = true, bool forceNoStack = false, int beginSlot = -1)
	{
		return Add(item.ID, item.Amount, assureFullInsertion, forceNoStack, beginSlot);
	}
	
	public ItemTransaction Remove(int item, int amount = 1, bool assureFullRemoval = false, int beginSlot = -1)
	{
		int hasAmount = GetItemCount(item);
		
		if (assureFullRemoval)
		{
			if (hasAmount < amount)
				return new ItemTransaction(amount, 0, new List<SlotItem>());
		}
		else
		{
			if (hasAmount < 1)
				return new ItemTransaction(amount, 0, new List<SlotItem>());
		}
		
		int totalRemoved = 0;
		List<SlotItem> removed = new List<SlotItem>();
		
		List<int>? skippedIndices = beginSlot != -1 ? Enumerable.Range(0, beginSlot).ToList() : null;
		
		int index = beginSlot != -1 ? beginSlot : 0;
		for (int i=index; i<Capacity; i++)
		{
			Item.Item? curItem = _items[i];
			if (curItem == null)
				continue;
			
			if (curItem.ID == item)
			{
				int removeCount = Math.Min(curItem.Amount, amount - totalRemoved);
				totalRemoved += removeCount;
				curItem.Amount -= removeCount;
				if (curItem.Amount == 0)
				{
					Item.Item removedItem = new Item.Item(_items[i]!);
					_items[i] = null;
					removed.Add(new SlotItem(i, removedItem));
				}
				if (totalRemoved == amount)
					break;
			}
		}
		
		if (skippedIndices != null && totalRemoved < amount)
		{
			foreach (int i in skippedIndices)
			{
				Item.Item? curItem = _items[i];
				if (curItem == null)
					continue;
				if (curItem.ID == item)
				{
					int removeCount = Math.Min(curItem.Amount, amount - totalRemoved);
					totalRemoved += removeCount;
					curItem.Amount -= removeCount;
					if (curItem.Amount == 0)
					{
						Item.Item removedItem = new Item.Item(_items[i]!);
						_items[i] = null;
						removed.Add(new SlotItem(i, removedItem));
					}
					if (totalRemoved == amount)
						break;
				}
			}
		}
		
		Dirty = totalRemoved > 0 ? true : Dirty;
		return new ItemTransaction(amount, totalRemoved, removed);
	}
	
	public ItemTransaction Remove(Item.Item item, bool assureFullRemoval = false, int beginSlot = -1)
	{
		return Remove(item.ID, item.Amount, assureFullRemoval, beginSlot);
	}
	
	public bool Replace(int remove, int add, int slot = -1)
	{
		return Remove(remove, beginSlot: slot).HasSucceeded()
				? Add(add, beginSlot: slot).HasSucceeded()
				: false;
	}
	
	public bool ReplaceWithItemRequirement(int remove, int add, int required, int slot = -1)
	{
		return Contains(required)
				? Replace(remove, add, slot)
				: false;
	}
	
	public bool ReplaceAndRemoveAnother(int remove, int add, Item.Item other, int slot = -1, int otherSlot = -1)
	{
		bool taken = Remove(other, true, otherSlot).HasSucceeded();
		bool replaced = Replace(remove, add, slot);
		if (replaced && taken)
		{
			return true;
		}
		else if (taken)
		{
			Add(other, true, beginSlot: otherSlot);
			return false;
		}
		return false;
	}
	
	public bool ReplaceAndRemoveAnotherWithItemRequirement(int remove, int add, Item.Item other, int required, int slot = -1, int otherSlot = -1)
	{
		return Contains(required)
				? ReplaceAndRemoveAnother(remove, add, other, slot, otherSlot)
				: false;
	}
	
	public bool ReplaceBoth(int removeItem, int addItem, int otherItem, int otherAddItem, int slot = -1, int otherSlot = -1)
	{
		bool taken = Replace(otherItem, otherAddItem, otherSlot);
		bool taken2 = Replace(removeItem, addItem, slot);
		if (taken && taken2)
		{
			return true;
		}
		else if(taken)
		{
			Replace(otherAddItem, otherItem, otherSlot);
			return false;
		}
		return false;
	}
	
	public void Swap(int from, int to)
	{
		Item.Item? temp = _items[from];
		_items[from] = _items[to];
		_items[to] = temp;
	}
	
	public void Shift()
	{
		
	}
	
	public IEnumerator<Item.Item?> GetEnumerator()
	{
		return _items.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return _items.GetEnumerator();
	}
	
}