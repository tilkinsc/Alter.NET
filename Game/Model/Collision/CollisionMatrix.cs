using Exceptions;

namespace Game.Model.Collision;

class CollisionMatrix
{
	
	public const short NO_COLLISION = 0;
	public const short FULL_COLLISION = short.MaxValue;
	public const short ALLOW_PROJECTILE_COLLISION = -256;
	
	public int Length;
	public int Width;
	public List<short> Matrix;
	
	private CollisionMatrix(int length, int width, List<short> matrix)
	{
		Length = length;
		Width = width;
		Matrix = matrix;
	}
	
	public CollisionMatrix(int width, int length)
		: this(length, width, new List<short>(width * length))
	{
	}
	
	private List<short> CopyMatrix()
	{
		short[] copy = new short[Matrix.Count];
		Matrix.CopyTo(copy);
		return new List<short>(copy);
	}
	
	public CollisionMatrix(CollisionMatrix other)
		: this(other.Length, other.Width, other.CopyMatrix())
	{
	}
	
	public void Block(int x, int y, bool impenetrable = true)
	{
		Set(x, y, (short) (impenetrable ? FULL_COLLISION : ALLOW_PROJECTILE_COLLISION));
	}
	
	public void Set(int x, int y, CollisionFlag flag)
	{
		Set(x, y, flag.ShortBit);
	}
	
	public void Set(int x, int y, short flag)
	{
		Matrix[y * Width + x] = flag;
	}
	
	public int Get(int x, int y)
	{
		return (int) Matrix[y * Width + x] & 0xFFFF;
	}
	
	public bool HasFlag(int x, int y, CollisionFlag flag)
	{
		return (Get(x, y) & flag.ShortBit) > 0;
	}
	
	public bool HasAllFlags(int x, int y, params CollisionFlag[] flags)
	{
		for (int i=0; i<flags.Length; i++)
		{
			if (!HasFlag(x, y, flags[i]))
				return false;
		}
		return true;
	}
	
	public bool HasAnyFlag(int x, int y, params CollisionFlag[] flags)
	{
		for (int i=0; i<flags.Length; i++)
		{
			if (HasFlag(x, y, flags[i]))
				return true;
		}
		return false;
	}
	
	// TODO: not sure if correct
	public bool IsClipped(int x, int y)
	{
		return Get(x, y) > 0;
	}
	
	public void AddFlag(int x, int y, CollisionFlag flag)
	{
		AddFlag(x, y, flag.ShortBit);
	}
	
	public void AddFlag(int x, int y, short flag)
	{
		Set(x, y, (short) (Matrix[y * Width + x] | flag));
	}
	
	public void RemoveFlag(int x, int y, CollisionFlag flag)
	{
		RemoveFlag(x, y, flag.ShortBit);
	}
	
	public void RemoveFlag(int x, int y, short flag)
	{
		Set(x, y, (short) ~flag);
	}
	
	public void Reset()
	{
		for (int x=0; x<Width; x++)
		{
			for (int y=0; y<Width; y++)
			{
				Reset(x, y);
			}
		}
	}
	
	public void Reset(int x, int y)
	{
		Set(x, y, NO_COLLISION);
	}
	
	public bool IsBlocked(int x, int y, Direction dir, bool projectile)
	{
		List<CollisionFlag> flags = projectile ? CollisionFlag.PROJECTILE_FLAGS : CollisionFlag.PAWN_FLAGS;
		
		int northwest = 0;
		int north = 1;
		int northeast = 2;
		int west = 3;
		int east = 4;
		int southwest = 5;
		int south = 6;
		int southeast = 7;
		
		if (dir == Direction.NORTH_WEST)
			return HasAnyFlag(x, y, new CollisionFlag[] {flags[northwest], flags[north], flags[west]});
		if (dir == Direction.NORTH)
			return HasFlag(x, y, flags[north]);
		if (dir == Direction.NORTH_EAST)
			return HasAnyFlag(x, y, new CollisionFlag[] {flags[northeast], flags[north], flags[east]});
		if (dir == Direction.EAST)
			return HasFlag(x, y, flags[east]);
		if (dir == Direction.SOUTH_EAST)
			return HasAnyFlag(x, y, new CollisionFlag[] {flags[southeast], flags[south], flags[east]});
		if (dir == Direction.SOUTH)
			return HasFlag(x, y, flags[south]);
		if (dir == Direction.SOUTH_WEST)
			return HasAnyFlag(x, y, new CollisionFlag[] {flags[southwest], flags[south], flags[west]});
		if (dir == Direction.WEST)
			return HasFlag(x, y, flags[west]);
		throw new IllegalArgumentException($"Unrecognized direction");
	}
	
	public override string ToString()
	{
		return "Unimplemented tostring CollisionMatrix";
	}
	
	public static List<CollisionMatrix> CreateMatrices(int count, int width, int length)
	{
		List<CollisionMatrix> matrices = new List<CollisionMatrix>(count);
		for (int i=0; i<count; i++)
		{
			matrices.Add(new CollisionMatrix(width, length));
		}
		return matrices;
	}
	
}