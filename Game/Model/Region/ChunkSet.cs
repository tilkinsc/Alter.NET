using Game.Model.Collision;
using Util;

namespace Game.Model.Region;


class ChunkSet
{
	
	public World World;
	public Dictionary<ChunkCoords, Chunk> Chunks = new Dictionary<ChunkCoords, Chunk>();
	
	public List<int> ActiveRegions = new List<int>();
	
	public ChunkSet(World world)
	{
		World = world;
	}
	
	public ChunkSet CopyChunksWithinRadius(ChunkCoords chunkCoords, int height, int radius)
	{
		ChunkSet newSet = new ChunkSet(World);
		List<ChunkCoords> surrounding = chunkCoords.GetSurroundingCoords(radius);
		
		foreach (ChunkCoords coords in surrounding)
		{
			Chunk chunk = Get(coords, true)!;
			Chunk copy = new Chunk(coords, chunk.Heights);
			copy.SetMatrix(height, new CollisionMatrix(chunk.GetMatrix(height)));
			newSet.Chunks[coords] = copy;
		}
		return newSet;
	}
	
	public int GetActiveChunkCount() => Chunks.Count;
	public int GetActiveRegionCount() => ActiveRegions.Count;
	
	public Chunk GetOrCreateChunk(Tile tile)
	{
		return Get(tile.ChunkCoords, true)!;
	}
	
	public Chunk? Get(Tile tile, bool createIfNeeded = true)
	{
		return Get(tile.ChunkCoords, createIfNeeded);
	}
	
	public Chunk? Get(ChunkCoords coords, bool createIfNeeded = false)
	{
		Chunk? chunk = null;
		if (Chunks.TryGetValue(coords, out chunk)) {
			return chunk;
		} else if (!createIfNeeded) {
			return null;
		}
		int regionId = coords.ToTile().RegionID;
		Chunk newChunk = new Chunk(coords, Tile.TOTAL_HEIGHT_LEVELS);
		newChunk.CreateEntityContainers();
		
		Chunks[coords] = newChunk;
		if (ActiveRegions.AddIfNotExist(regionId)) {
			World.Definitions.CreateRegion(World, regionId);
		}
		return newChunk;
	}
	
	public bool Remove(ChunkCoords coords) => Chunks.Remove(coords);
	
}
