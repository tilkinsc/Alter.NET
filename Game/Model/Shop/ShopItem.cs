namespace Game.Model.Shop;

class ShopItem
{
	
	public int Item { get; private set; }
	public int Amount { get; private set; }
	public int? SellPrice { get; private set; }
	public int? BuyPrice { get; private set; }
	public int ResupplyAmount { get; private set; }
	public int ResupplyCycles { get; private set; }
	
	public int CurrentAmount;
	
	public bool IsTemporary => Amount == 0;

	public ShopItem(int item, int amount, int? sellPrice = null, int? buyPrice = null, int resupplyAmount =  Shop.DEFAULT_RESUPPLY_AMOUNT, int resupplyCycles = Shop.DEFAULT_RESUPPLY_CYCLES)
	{
		Item = item;
		Amount = amount;
		SellPrice = sellPrice;
		BuyPrice = buyPrice;
		ResupplyAmount = resupplyAmount;
		ResupplyCycles = resupplyCycles;
	}
	
}