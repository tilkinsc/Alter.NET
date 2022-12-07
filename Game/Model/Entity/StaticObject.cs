namespace Game.Model.Entity;

class StaticObject : GameObject
{
	
	public StaticObject(int id, int type, int rot, Tile tile)
			: base(id, type, rot, tile)
	{
		EntityType = EntityType.STATIC_OBJECT;
	}
	
}