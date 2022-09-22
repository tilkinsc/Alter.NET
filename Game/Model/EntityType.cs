namespace Game.Model;

enum EntityType
{
	NONE = -1,
	PLAYER = 0,
	CLIENT,
	NPC,
	STATIC_OBJECT,
	DYNAMIC_OBJECT,
	MAP_ANIM,
	GROUND_ITEM,
	PROJECTILE,
	AREA_SOUND
}

static class EntityTypeExtensions
{
	
	public static bool IsHumanControlled(this EntityType type) => type == EntityType.CLIENT;
	public static bool IsPlayer(this EntityType type) => type == EntityType.CLIENT || type == EntityType.PLAYER;
	public static bool IsNPC(this EntityType type) => type == EntityType.NPC;
	public static bool IsObject(this EntityType type) => type == EntityType.STATIC_OBJECT || type == EntityType.DYNAMIC_OBJECT;
	public static bool IsProjectile(this EntityType type) => type == EntityType.PROJECTILE;
	public static bool IsGroundItem(this EntityType type) => type == EntityType.GROUND_ITEM;
	public static bool IsTransient(this EntityType type) => type == EntityType.PROJECTILE || type == EntityType.AREA_SOUND || type == EntityType.MAP_ANIM;
	
}
