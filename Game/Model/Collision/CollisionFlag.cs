namespace Game.Model.Collision;

class CollisionFlag
{
	
	public static readonly CollisionFlag PAWN_NORTH_WEST = new CollisionFlag(1);
	public static readonly CollisionFlag PAWN_NORTH = new CollisionFlag(2);
	public static readonly CollisionFlag PAWN_NORTH_EAST = new CollisionFlag(3);
	public static readonly CollisionFlag PAWN_EAST = new CollisionFlag(4);
	public static readonly CollisionFlag PAWN_SOUTH_EAST = new CollisionFlag(5);
	public static readonly CollisionFlag PAWN_SOUTH = new CollisionFlag(6);
	public static readonly CollisionFlag PAWN_SOUTH_WEST = new CollisionFlag(7);
	public static readonly CollisionFlag PAWN_WEST = new CollisionFlag(8);
	public static readonly CollisionFlag PROJECTILE_NORTH_WEST = new CollisionFlag(9);
	public static readonly CollisionFlag PROJECTILE_NORTH = new CollisionFlag(10);
	public static readonly CollisionFlag PROJECTILE_NORTH_EAST = new CollisionFlag(11);
	public static readonly CollisionFlag PROJECTILE_EAST = new CollisionFlag(12);
	public static readonly CollisionFlag PROJECTILE_SOUTH_EAST = new CollisionFlag(13);
	public static readonly CollisionFlag PROJECTILE_SOUTH = new CollisionFlag(14);
	public static readonly CollisionFlag PROJECTILE_SOUTH_WEST = new CollisionFlag(15);
	public static readonly CollisionFlag PROJECTILE_WEST = new CollisionFlag(16);
	
	public int Bit { get; }
	public short ShortBit { get => (short) (1 << Bit); }
	
	private CollisionFlag(int bit)
	{
		this.Bit = bit;
	}
	
	public static readonly List<CollisionFlag> PAWN_FLAGS = new List<CollisionFlag>(new CollisionFlag[] {
		PAWN_NORTH_WEST,
		PAWN_NORTH,
		PAWN_NORTH_EAST,
		PAWN_WEST,
		PAWN_EAST,
		PAWN_SOUTH_WEST,
		PAWN_SOUTH,
		PAWN_SOUTH_EAST
	});
	
	public static readonly List<CollisionFlag> PROJECTILE_FLAGS = new List<CollisionFlag>(new CollisionFlag[] {
		PROJECTILE_NORTH_WEST,
		PROJECTILE_NORTH,
		PROJECTILE_NORTH_EAST,
		PROJECTILE_WEST,
		PROJECTILE_EAST,
		PROJECTILE_SOUTH_WEST,
		PROJECTILE_SOUTH,
		PROJECTILE_SOUTH_EAST
	});
	
	public static List<CollisionFlag> GetFlags(bool projectiles) => projectiles ? PROJECTILE_FLAGS : PAWN_FLAGS;
	public static List<CollisionFlag> PawnFlags() => PAWN_FLAGS;
	public static List<CollisionFlag> ProjectileFlags() => PROJECTILE_FLAGS;
	
}