using Game.Model.Entity;

namespace Game.Model.Shop;

interface ShopCurrency
{
	public void OnSellValueMessage(Player p, ShopItem shopItem);
	public void OnBuyValueMessage(Player p, Shop shop, int item);
	public void GetSellPrice(World world, int item);
	public void GetBuyPrice(World world, int item);
	public void SellToPlayer(Player p, Shop shop, int slot, int amount);
	public void BuyFromPlayer(Player p, Shop shop, int slot, int amount);
}