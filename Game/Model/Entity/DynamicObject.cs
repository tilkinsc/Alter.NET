namespace Game.Model.Entity;

class DynamicObject : GameObject
{
	
	public DynamicObject(int id, int type, int rot, Tile tile)
			: base(id, type, rot, tile)
	{
		EntityType = EntityType.DYNAMIC_OBJECT;
	}
	
	public DynamicObject(GameObject other)
			: this(other.ID, other.Type, other.Rotation, new Tile(other.Tile))
	{
	}
	
	public DynamicObject(GameObject other, int id)
			: this(id, other.Type, other.Rotation, new Tile(other.Tile))
	{
	}
	
}