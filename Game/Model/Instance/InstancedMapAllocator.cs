using Game.Model.Entity;
using Game.Model.Region;

namespace Game.Model.Instance;

class InstancedMapAllocator
{
	
	public static readonly Area VALID_AREA = new Area(6400, 0, 9600, 6400);
	public const int SCAN_MAPS_CYCLES = 25;
	
	private List<InstancedMap> Maps = new List<InstancedMap>();
	private int DeallocationScanCycle = 0;
	
	public int ActiveMapCount => Maps.Count;
	
	private InstancedMap Allocate(int x, int z, InstancedChunkSet chunks, InstancedMapConfiguration configs)
	{
		return new InstancedMap(new Area(x, z, x + chunks.RegionSize * Chunk.REGION_SIZE, z + chunks.RegionSize * Chunk.REGION_SIZE), chunks, configs.ExitTile, configs.Owner, configs.Attributes);
	}
	
	public InstancedMap? Allocate(World world, InstancedChunkSet chunks, InstancedMapConfiguration configs)
	{
		Area area = VALID_AREA;
		int step = 64;
		
		int totalTiles = chunks.RegionSize * Chunk.REGION_SIZE;
		
		for (int x=area.BottomLeftX; x<area.TopRightX; x += step)
		{
			for (int z=area.BottomLeftZ; z<area.TopRightZ; z += step)
			{
				foreach (InstancedMap map in Maps)
				{
					if (map.Area.Contains(x, z) || map.Area.Contains(x + totalTiles - 1, z + totalTiles - 1))
						goto skip;
				}
				
				InstancedMap map = Allocate(x, z, chunks, configs);
				ApplyCollision(world, map, configs.BypassObjectChunkBounds);
				Maps.Add(map);
				return map;
			skip:
				continue;
			}
		}
		return null;
	}
	
	private void Deallocate(World world, InstancedMap map)
	{
		if (Maps.Remove(map)) {
			RemoveCollision(world, map);
			world.RemoveAll(map.Area);
			
			foreach (var? p in world.Players)
			{
				if (map.Area.Contains(p.Tile))
					p.MoveTo(map.ExitTile);
			}
		}
	}
	
	public InstancedMap? GetMap(Tile tile) => Maps.Find((obj) => obj.Area.Contains(tile));
	
	public void Logout(Player player)
	{
		World world = player.World;
		
		GetMap(player.Tile).
		
		getMap(player.tile)?.let { map ->
            player.moveTo(map.exitTile)

            if (map.attr.contains(InstancedMapAttribute.DEALLOCATE_ON_LOGOUT)) {
                val mapOwner = map.owner!! // If map has this attribute, they should also set an owner.
                if (player.uid == mapOwner) {
                    deallocate(world, map)
                }
            }
        }
	}
	
	public void Death(Player player)
	{
		World world = player.World;
		
		getMap(player.tile)?.let { map ->
            if (map.attr.contains(InstancedMapAttribute.DEALLOCATE_ON_DEATH)) {
                val mapOwner = map.owner!! // If map has this attribute, they should also set an owner.
                if (player.uid == mapOwner) {
                    deallocate(world, map)
                }
            }
        }
	}
	
	public void Cycle(World world)
	{
		if (DeallocationScanCycle++ == SCAN_MAPS_CYCLES) {
			for (int i=0; i<Maps.Count; i++)
			{
				InstancedMap map = Maps[i];
				if (world.players.none { map.area.contains(it.tile) }) {
                    Deallocate(world, map);
                }
			}
			DeallocationScanCycle = 0;
		}
	}
	
}