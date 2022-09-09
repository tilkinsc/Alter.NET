using Exceptions;

namespace Game.Model;

class Direction
{
	
	public static readonly Direction NONE = new Direction(-1, -1, -1);
	public static readonly Direction NORTH_WEST = new Direction(0, 5, 0);
	public static readonly Direction NORTH = new Direction(1, 6, 1);
	public static readonly Direction NORTH_EAST = new Direction(2, 7, 2);
	public static readonly Direction WEST = new Direction(3, 3, 3);
	public static readonly Direction EAST = new Direction(4, 4, 4);
	public static readonly Direction SOUTH_WEST = new Direction(5, 0, 5);
	public static readonly Direction SOUTH = new Direction(6, 1, 6);
	public static readonly Direction SOUTH_EAST = new Direction(7, 2, 7);
	
	public int OrientationValue { get; }
	public int PlayerWalkValue { get; }
	public int NpcWalkValue { get; }
	
	private Direction(int orientationValue, int playerWalkValue, int npcWalkValue)
	{
		OrientationValue = orientationValue;
		PlayerWalkValue = playerWalkValue;
		NpcWalkValue = npcWalkValue;
	}
	
	public bool IsDiagonal()
	{
		return this == SOUTH_EAST || this == SOUTH_WEST || this == NORTH_EAST || this == NORTH_WEST;
	}
	
	public int GetDeltaX()
	{
		if (this == SOUTH_EAST || this == NORTH_EAST || this == EAST)
			return 1;
		if (this == SOUTH_WEST || this == NORTH_WEST || this == WEST)
			return -1;
		return 0;
	}
	
	public int GetDeltaZ()
	{
		if (this == NORTH_WEST || this == NORTH_EAST || this == NORTH)
			return 1;
		if (this == SOUTH_WEST || this == SOUTH_EAST || this == SOUTH)
			return -1;
		return 0;
	}
	
	public Direction GetOpposite()
	{
		if (this == NORTH)
			return SOUTH;
		if (this == SOUTH)
			return NORTH;
		if (this == EAST)
			return WEST;
		if (this == WEST)
			return EAST;
		if (this == NORTH_WEST)
			return SOUTH_EAST;
		if (this == NORTH_EAST)
			return SOUTH_WEST;
		if (this == SOUTH_EAST)
			return NORTH_WEST;
		if (this == SOUTH_WEST)
			return NORTH_EAST;
		return NONE;
	}
	
	public List<Direction> GetDiagonalComponents()
	{
		if (this == NORTH_EAST)
			return new List<Direction>(new Direction[] {NORTH, EAST});
		if (this == NORTH_WEST)
			return new List<Direction>(new Direction[] {NORTH, WEST});
		if (this == SOUTH_EAST)
			return new List<Direction>(new Direction[] {SOUTH, EAST});
		if (this == SOUTH_WEST)
			return new List<Direction>(new Direction[] {SOUTH, WEST});
		throw new IllegalArgumentException("Must provide a diagonal direction");
	}
	
	public int GetAngle()
	{
		if (this == SOUTH)
			return 0;
		if (this == SOUTH_WEST)
			return 256;
		if (this == WEST)
			return 512;
		if (this == NORTH_WEST)
			return 768;
		if (this == NORTH)
			return 1024;
		if (this == NORTH_EAST)
			return 1280;
		if (this == EAST)
			return 1536;
		if (this == SOUTH_EAST)
			return 1792;
		throw new IllegalStateException($"Invalid direction {this}");
	}
	
	public static List<Direction> NESW = new List<Direction>(new Direction[] {NORTH, EAST, SOUTH, WEST});
	public static List<Direction> WNES = new List<Direction>(new Direction[] {WEST, NORTH, EAST, SOUTH});
	public static List<Direction> WNES_DIAGONAL = new List<Direction>(new Direction[] {NORTH_WEST, NORTH_EAST, SOUTH_EAST, SOUTH_WEST});
	public static List<Direction> RS_ORDER = new List<Direction>(new Direction[] {WEST, EAST, NORTH, SOUTH, SOUTH_WEST, SOUTH_EAST, NORTH_WEST, NORTH_EAST});
	public static List<Direction> ANGLED_ORDER = new List<Direction>(new Direction[] {NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST});
	
	public static Direction GetForAngle(int angle) => ANGLED_ORDER[angle / 45];
	
	public static Direction Between(Tile current, Tile next)
	{
		int deltaX = next.X - current.X;
		int deltaZ = next.Z - current.Z;
		return FromDeltas(deltaX, deltaZ);
	}
	
	public static Direction FromDeltas(int deltaX, int deltaZ)
	{
		switch (deltaZ)
		{
			case 1:
				switch (deltaX)
				{
					case 1: return NORTH_EAST;
					case 0: return NORTH;
					case -1: return NORTH_WEST;
				}
				break;
			case 0:
				switch (deltaX)
				{
					case 1: return EAST;
					case 0: return NONE;
					case -1: return WEST;
				}
				break;
			case -1:
				switch (deltaX)
				{
					case 1: return SOUTH_EAST;
					case 0: return SOUTH;
					case -1: return SOUTH_WEST;
				}
				break;
		}
		throw new IllegalArgumentException($"Unhandled delta difference. [{deltaX}, {deltaZ}]");
	}
	
}
