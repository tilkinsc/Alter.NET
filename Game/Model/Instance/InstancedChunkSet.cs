using Exceptions;

namespace Game.Model.Instance;

class InstancedChunkSet
{
	
	public static int GetCoordinates(int x, int z, int height)
	{
		return ((height & 0x3) << 28) | ((x & 0x3FF) << 14) | (z & 0x7FF);
	}
	
	public int RegionSize;
	public Dictionary<int, InstancedChunk> Values;
	
	public InstancedChunkSet(int regionSize, Dictionary<int, InstancedChunk> values)
	{
		RegionSize = regionSize;
		Values = values;
	}
	
	public class Builder
	{
		
		private int RegionSize = 1;
		private Dictionary<int, InstancedChunk> Chunks = new Dictionary<int, InstancedChunk>();
		
		public InstancedChunkSet Build()
		{
			return new InstancedChunkSet(RegionSize, Chunks);
		}
		
		public Builder Set(int chunkX, int chunkZ, Tile copy, int height = 0, int rot = 0)
		{
			if (height < 0 && height > Tile.TOTAL_HEIGHT_LEVELS)
				throw new IllegalStateException("Height must be in bounds [0-3]");
			if (rot < 0 && rot > 3)
				throw new IllegalStateException("Rotation must be in bounds [0-3]");
			
			if (RegionSize < (chunkX >> 3) + 1 || RegionSize < (chunkZ >> 3) + 1) {
				RegionSize = Math.Max((chunkX >> 3) + 1, (chunkZ >> 3) + 1);
			}
			
			int packed = copy.ToRotatedInteger(rot);
			InstancedChunk chunk = new InstancedChunk(packed);
			
			int coordinates = GetCoordinates(chunkX, chunkZ, height);
			Chunks[coordinates] = chunk;
			return this;
		}
		
	}
	
}