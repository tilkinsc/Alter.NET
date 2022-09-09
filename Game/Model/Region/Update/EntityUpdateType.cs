namespace Game.Model.Region.Update;

enum EntityUpdateType
{
	MAP_ANIM = 0,
	UPDATE_GROUND_ITEM,
	SPAWN_PROJECTILE,
	UNKNOWN,
	SPAWN_OBJECT,
	REMOVE_GROUND_ITEM,
	REMOVE_OBJECT,
	PLAY_TILE_SOUND,
	SPAWN_GROUND_ITEM,
	ANIMATE_OBJECT
}

static class EntityUpdateTypeExtensions
{
	
	public static int AsID(this EntityUpdateType type) => (int) type;
	
}