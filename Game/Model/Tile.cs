using Exceptions;
using Game.Model.Region;
using Util;

namespace Game.Model;

class Tile : IEquatable<Tile>
{
	
	public const int TOTAL_HEIGHT_LEVELS = 4;
	
	public int Coordinate { get; set; }
	
	public int X { get => Coordinate & 0x7FFF; }
	public int Z { get => (Coordinate >> 15) & 0x7FFF; }
	public int Height { get => (Coordinate >> 30) & 2; }
	
	public int TopLeftRegionX { get => (X >> 3) - 6; }
	public int TopLeftRegionZ { get => (Z >> 3) - 6; }
	
	public int RegionID { get => ((X >> 6) << 8) | (Z >> 6); }
	
	public Tile RegionBase { get => new Tile(((X >> 3) - (Chunk.MAX_VIEWPORT >> 4)) << 3, ((Z >> 3) - (Chunk.MAX_VIEWPORT >> 4)) << 3, Height); }
	
	public ChunkCoords ChunkCoords { get => ChunkCoords.FromTile(this); }
	
	public int As30BitInteger { get => (Z >> 13) | ((X >> 13) << 8) | ((Height & 0x3) << 16); }
	
	public int AsTileHashMultiplier { get => (Z >> 13) | ((X >> 13) << 8) | ((Height & 0x3) << 16); }
	
	public Tile(int coord)
	{
		Coordinate = coord;
		if (Height < TOTAL_HEIGHT_LEVELS)
			Logger.Warn($"Height {Height} exceeded maximum tile height.");
	}
	
	public Tile(int x, int z, int height = 0)
		: this((x & 0x7FFF) | ((z & 0x7FFF) << 15) | (height << 30))
	{
	}
	
	public Tile(Tile other)
		: this(other.Coordinate)
	{
	}
	
	public Tile Transform(int x, int z, int height)
	{
		return new Tile(X + x, Z + z, Height + height);
	}
	
	public Tile Transform(int x, int z)
	{
		return new Tile(X + x, Z + z, Height);
	}
	
	public Tile Transform(int height)
	{
		return new Tile(X, Z, Height + height);
	}
	
	public bool IsViewableFrom(Tile other, int viewDistance = 15)
	{
		return GetDistance(other) <= viewDistance;
	}
	
	public Tile Step(Direction direction, int steps = 1)
	{
		return new Tile(X + (steps * direction.GetDeltaX()), Z + (steps * direction.GetDeltaZ()), Height);
	}
	
	public Tile? TransformAndRotate(int localX, int localZ, int orientation, int width = 1, int length = 1)
	{
		int localWidth = Chunk.CHUNK_SIZE - 1;
		int localLength = Chunk.CHUNK_SIZE - 1;
		
		switch (orientation)
		{
			case 0: return Transform(localX, localZ);
			case 1: return Transform(localZ, localLength - localX - (width - 1));
			case 2: return Transform(localWidth - localX - (width - 1), localLength - localZ - (length - 1));
			case 3: return Transform(localWidth - localZ - (length - 1), localX);
			default:
				throw new IllegalArgumentException($"Illegal orientation! Value must be in bounds [0-3]. [orientation={orientation}");
		};
	}
	
	public bool IsWithinRadius(int otherX, int otherZ, int otherHeight, int radius)
	{
		if (otherHeight != Height)
			return false;
		
		int dx = Math.Abs(X - otherX);
		int dz = Math.Abs(Z - otherZ);
		return dx <= radius && dz <= radius;
	}
	
	public bool IsWithinRadius(Tile other, int radius) => IsWithinRadius(other.X, other.Z, other.Height, radius);
	
	public bool IsNextTo(Tile other)
	{
		return Math.Abs(X - other.X) + Math.Abs(Z - other.Z) == 1;
	}
	
	public bool IsWithinSameChunk(Tile other)
	{
		return (X >> 3) == (other.X >> 3) && (Z >> 3) == (other.Z >> 3);
	}
	
	public int GetDistance(Tile other)
	{
		int dx = X - other.X;
		int dz = Z - other.Z;
		return (int) Math.Ceiling(Math.Sqrt((double) (dx * dx + dz * dz)));
	}
	
	public int GetDelta(Tile other)
	{
		return Math.Abs(X - other.X) + Math.Abs(Z - other.Z);
	}
	
	public Tile ToLocal(Tile other)
	{
		return new Tile(((other.X >> 3) - (X >> 3)) << 3, ((other.Z >> 3) - (Z >> 3)) >> 3, Height);
	}
	
	public int ToRotatedInteger(int rot)
	{
		return ((Height & 0x3) << 24) | (((X >> 3) & 0x3FF) << 14) | (((Z >> 3) & 0x7FF) << 3) | ((rot & 0x3) << 1);
	}
	
	public bool SameAs(Tile other)
	{
		return other.X == X && other.Z == Z && other.Height == Height;
	}
	
	public bool SameAs(int x, int z)
	{
		return x == X && z == Z;
	}
	
	public override string ToString()
	{
		return "Tile ToString not implemented";
	}
	
	public override int GetHashCode()
	{
		return Coordinate;
	}
	
	public override bool Equals(object? other)
	{
		return Equals(other as Tile);
	}
	
	public bool Equals(Tile? tile)
	{
		if (tile == null)
			return false;
		return tile.Coordinate == Coordinate;
	}
	
	public static Tile operator +(Tile self, Tile other)
			=> new Tile(self.X + other.X, self.Z + other.Z, self.Height + other.Height);

	public static Tile operator -(Tile self, Tile other)
			=> new Tile(self.X - other.X, self.Z - other.Z, self.Height - other.Height);
	
	public static Tile FromRotatedHash(int packed)
	{
		int x = ((packed >> 14) & 0x3FF) << 3;
		int z = ((packed >> 3) & 0x7FF) << 3;
		int height = (packed >> 28) & 0x3;
		return new Tile(x, z, height);
	}
	
	public static Tile From30BitHash(int packed)
	{
		int x = ((packed >> 14) & 0x3FFF);
		int z = ((packed) & 0x3FFF);
		int height = (packed >> 28);
		return new Tile(x, z, height);
	}
	
	public static Tile FromRegion(int region)
	{
		int x = ((region >> 8) << 6);
		int z = ((region & 0xFF) << 6);
		return new Tile(x, z);
	}
	
}