using Game.Model.Item;
using Util;

namespace Game.Model.Entity;

class GroundItem : BaseEntity
{
	
	public const int DEFAULT_RESPAWN_CYCLES = 50;
	public const int DEFAULT_PUBLIC_SPAWN_CYCLES = 100;
	public const int DEFAULT_DESPAWN_CYCLES = 600;
	
	public int Item { get; private set; }
	public int Amount { get; private set; }
	public PlayerUID? OwnerUID;
	
	public int CurrentCycle = 0;
	public int RespawnCycles = -1;
	public Dictionary<ItemAttribute, int> Attributes = new Dictionary<ItemAttribute, int>();
	
	public GroundItem(int item, int amount, Tile tile, Player? owner = null)
			: base(new Tile(tile), EntityType.GROUND_ITEM)
	{
		Item = item;
		Amount = amount;
		if (owner != null)
			OwnerUID = owner.UID;
	}
	
	public GroundItem(Item.Item item, Tile tile, Player? owner = null)
			: this(item.ID, item.Amount, tile, owner)
	{
	}
	
	public bool IsOwnedBy(Player p) => OwnerUID != null && p.UID.Value == OwnerUID.Value;
	public bool IsPublic() => OwnerUID == null;
	public bool CanBeViewedBy(Player p) => IsPublic() || IsOwnedBy(p);
	public void RemoveOwner() => OwnerUID = null;
	
	public GroundItem CopyAttributes(Dictionary<ItemAttribute, int> attributes)
	{
		Attributes.Merge(attributes);
		return this;
	}
	
	public bool IsSpawned(World world) => world.IsSpawned(this);
	
	public override string ToString()
	{
		return $"ToString not implemented";
	}
	
}