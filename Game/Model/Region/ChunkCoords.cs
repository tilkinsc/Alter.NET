namespace Game.Model.Region;

class ChunkCoords : IEquatable<ChunkCoords>
{
	
	public int X;
	public int Z;
	
	public ChunkCoords(int x, int z)
	{
		X = x;
		Z = z;
	}
	
	public Tile ToTile()
	{
		return new Tile((X + 6) << 3, (Z + 6) << 3);
	}
	
	public List<ChunkCoords> GetSurroundingCoords(int chunkRadius = Chunk.CHUNK_VIEW_RADIUS)
	{
		List<ChunkCoords> surrounding = new List<ChunkCoords>();
		
		for (int x=-chunkRadius; x<chunkRadius; x++)
		{
			for (int z=-chunkRadius; z<chunkRadius; z++)
			{
				surrounding.Add(new ChunkCoords(X + x, Z + z));
			}
		}
		
		return surrounding;
	}
	
	public override string ToString()
	{
		return "ChunkCoords.cs Unimplemented";
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as ChunkCoords);
	}
	
	public bool Equals(ChunkCoords? coords)
	{
		if (coords == null)
			return false;
		if (X == coords.X && Z == coords.Z)
			return true;
		return false;
	}
	
	public override int GetHashCode()
	{
		return (X << 16) | Z;
	}
	
	public ChunkCoords FromTile(int x, int z)
	{
		return new ChunkCoords(x, z);
	}
	
	public ChunkCoords FromTile(Tile tile)
	{
		return FromTile(tile.TopLeftRegionX, tile.TopLeftRegionZ);
	}
	
}
