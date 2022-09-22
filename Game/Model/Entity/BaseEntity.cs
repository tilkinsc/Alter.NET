namespace Game.Model.Entity;

abstract class BaseEntity
{
	
	public const string NOTHING_INTERESTING_HAPPENS = "Nothing interesting happens.";
	public const string YOU_CANT_REACH_THAT = "I can't reach that!";
	public const string MAGIC_STOPS_YOU_FROM_MOVING = "A magical force stops you from moving.";
	public const string YOURE_STUNNED = "You're stunned!";
	
	public Tile? Tile;
	public EntityType EntityType;
	
	public BaseEntity(Tile tile, EntityType entityType)
	{
		Tile = tile;
		EntityType = entityType;
	}
	
	public BaseEntity(Tile tile)
			: this (tile, EntityType.NONE)
	{
	}
	
	public BaseEntity()
	{
	}
	
}