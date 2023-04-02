using Game.Model.Entity;
using static Game.Model.Attr.Attributes;

namespace Game.Model.Shop;


class Shop
{
	public const int DEFAULT_STOCK_SIZE = 40;
	public const int DEFAULT_RESUPPLY_AMOUNT = 1;
	public const int DEFAULT_RESUPPLY_CYCLES = 25;
	
	public string Name { get; private set; }
	public StockType StockType { get; private set; }
	public PurchasePolicy PurchasePolicy { get; private set; }
	public ShopCurrency Currency { get; private set; }
	public List<ShopItem?> Items { get; private set; }
	
	public readonly List<PlayerUID> Viewers = new List<PlayerUID>();
	
	private int _currentCycle = 0;
	
	public Shop(string name, StockType stockType, PurchasePolicy purchasePolicy, ShopCurrency currency, List<ShopItem?> items)
	{
		Name = name;
		StockType = stockType;
		PurchasePolicy = purchasePolicy;
		Currency = currency;
		Items = items;
	}
	
	public void Refresh(World world)
	{
		for (int i=Viewers.Count; i>=0; i++)
		{
			Player player = world.GetPlayerForUID(Viewers[i]);
			if (player == null)
			{
				Viewers.RemoveAt(i);
				continue;
			}
			if (player.Attributes[CURRENT_SHOP_ATTR] == this)
			{
				player.ShopDirty = true;
			}
			else
			{
				Viewers.RemoveAt(i);
			}
		}
	}
	
	public void Cycle(World world)
	{
		bool refresh = false;
		for (int i=0; i<Items.Count; i++)
		{
			ShopItem? item = Items.ElementAtOrDefault(i);
			if (item == null)
				continue;
			if (item.CurrentAmount != item.Amount && _currentCycle % item.ResupplyCycles == 0)
			{
				refresh = true;
				int amount = item.CurrentAmount > item.Amount
						? Math.Max(item.Amount, item.CurrentAmount - item.ResupplyAmount)
						: Math.Min(item.Amount, item.CurrentAmount + item.ResupplyAmount);
				if (amount == 0 && item.Amount == 0)
				{
					Items.RemoveAt(i);
					continue;
				}
				item.CurrentAmount = amount;
			}
		}
		
		if (refresh)
			Refresh(world);
		
		_currentCycle++;
		if (_currentCycle == Int32.MaxValue)
			_currentCycle = 0;
	}
	
	public override int GetHashCode()
	{
		int hash = Name.GetHashCode();
		hash = 31 * hash + StockType.GetHashCode();
		hash = 31 * hash + Items.ContentHashCode();
		return hash;
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as Shop);
	}
	
	public bool Equals(Shop? shop)
	{
		if (shop == null)
			return false;
		if (Name != shop.Name)
			return false;
		if (StockType != shop.StockType)
			return false;
		if (!Items.ContentEquals(shop.Items))
			return false;
		return true;
	}
	
}