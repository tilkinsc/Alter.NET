using Game.Model.Region;

namespace Game.Model.Instance;

class InstancedMap
{
	
	public Area Area;
	public InstancedChunkSet Chunks;
	public Tile ExitTile;
	public PlayerUID? Owner;
	public InstancedMapAttribute[] Attributes;
	
	public InstancedMap(Area area, InstancedChunkSet chunks, Tile exitTile, PlayerUID? owner, InstancedMapAttribute[] attr)
	{
		Area = area;
		Chunks = chunks;
		ExitTile = exitTile;
		Owner = owner;
		Attributes = attr;
	}
	
	public int[] GetCoordinates(Tile relative)
	{
		int heights = Tile.TOTAL_HEIGHT_LEVELS;
		int bounds = Chunk.CHUNKS_PER_REGION;
		
		int[] coordinates = new int[heights * bounds * bounds];
		
		int startX = relative.X - 48;
		int startZ = relative.Z - 48;
		
		int index = 0;
		for (int height=0; height<heights; height++)
		{
			for (int x=0; x<bounds; x++)
			{
				for (int z=0; z<bounds; z++)
				{
					Tile absolute = new Tile(startX + (x << 3), startZ + (z << 3));
					int chunkX = (absolute.X - Area.BottomLeftX) >> 3;
					int chunkZ = (absolute.Z - Area.BottomLeftZ) >> 3;
					
					var? coord = InstancedChunkSet.GetCoordinates(chunkX, chunkZ, height);
					var? chunk = Chunks.Values[coord];
					
					coordinates[index++] = chunk?.Packed ?: -1;
				}
			}
		}
		return coordinates;
	}
	
}