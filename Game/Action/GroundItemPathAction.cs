using Game.Model.Entity;

namespace Game.Action;

class GroundItemPathAction
{
	
	public const int ITEM_ON_GROUND_ITEM_OPTION = 1;
	
	private void HandleAction(Player p, GroundItem groundItem, int opt)
	{
		if (!p.World.IsSpawned(groundItem))
			return;
		
		if (opt == 3)
		{
			if (!p.World.Plugins.CanPickupGroundItem(p, groundItem.Item))
				return;
			
			p.Inventory.GetItemCount(groundItem.Item);
		}
	}
	
}