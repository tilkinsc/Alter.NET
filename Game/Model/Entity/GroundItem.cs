namespace Game.Model.Entity;

class GroundItem : BaseEntity
{
	
	public int CurrentCycle = 0;
	public int RespawnCycles = -1;
	// internal val attr = EnumMap<ItemAttribute, Int>(ItemAttribute::class.java)
	
	public GroundItem(int item, int amount, Tile tile, Player? owner = null)
			: base(new Tile(tile), EntityType.GROUND_ITEM)
	{
	}
	
	public GroundItem(Item item, Tile tile, Player? owner = null)
			: this(item.ID, item.Amount, tile, owner)
	{
	}
	
}