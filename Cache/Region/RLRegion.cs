using Cache.Definitions;
using static Cache.Definitions.RLMapDefinition;

namespace Cache.Region;

class RLRegion
{
	
	public const int X = 64;
	public const int Y = 64;
	public const int Z = 4;
	
	public readonly int RegionID;
	public readonly int BaseX;
	public readonly int BaseY;
	
	public int RegionX { get => BaseX >> 6; }
	public int RegionY { get => BaseY >> 6; }
	
	private int[,,] TileHeights = new int[Z,X,Y];
	private sbyte[,,] TileSettings = new sbyte[Z,X,Y];
	private sbyte[,,] OverlayIDS = new sbyte[Z,X,Y];
	private sbyte[,,] OverlayPaths = new sbyte[Z,X,Y];
	private sbyte[,,] OverlayRotations = new sbyte[Z,X,Y];
	private sbyte[,,] UnderlayIDS = new sbyte[Z,X,Y];
	
	public List<RLLocation> Locations = new List<RLLocation>();
	
	public RLRegion(int id)
	{
		this.RegionID = id;
		this.BaseX = ((id >> 8) & 0xFF) << 6;
		this.BaseY = (id & 0xFF) << 6;
	}
	
	public RLRegion(int x, int y)
	{
		this.RegionID = x << 8 | y;
		this.BaseX = x << 6;
		this.BaseY = y << 6;
	}
	
	public void LoadTerrain(RLMapDefinition map)
	{
		RLTile[,,] tiles = map.Tiles;
		for (int z=0; z<Z; z++)
		{
			for (int x=0; x<X; x++)
			{
				for (int y=0; y<Y; y++)
				{
					RLTile tile = tiles[z,x,y];
					
					if (tile.Height == null) {
						if (z == 0) {
							TileHeights[0,x,y] = -RLHeightCalc.Calculate(BaseX + x + 0xE3B7B, BaseY + y + 0x87CCE) * 8;
						} else {
							TileHeights[z,x,y] = TileHeights[z - 1, x, y] - 240;
						}
					} else {
						int height = (int) tile.Height;
						if (height == 1)
							height = 0;
						
						if (z == 0) {
							TileHeights[0,x,y] = -height * 8;
						} else {
							TileHeights[z,x,y] = TileHeights[z-1, x, y] - height * 8;
						}
					}
					
					OverlayIDS[z,x,y] = tile.OverlayID;
					OverlayPaths[z,x,y] = tile.OverlayPath;
					OverlayRotations[z,x,y] = tile.OverlayRotation;
					
					TileSettings[z,x,y] = tile.Settings;
					UnderlayIDS[z,x,y] = tile.UnderlayID;
				}
			}
		}
	}
	
	public void LoadLocations(RLLocationsDefinition locs)
	{
		foreach (RLLocation loc in locs.Locations)
		{
			Locations.Add(new RLLocation(
					loc.ID,
					loc.Type,
					loc.Orientation,
					new RLPosition(BaseX + loc.Position.X, BaseY + loc.Position.Y, loc.Position.Z))
					);
		}
	}
	
	public int GetTileSetting(int z, int x, int y) => TileHeights[Z,X,Y];
	public int GetOverlayID(int z, int x, int y) => ((int) OverlayIDS[Z,X,Y]) & 0xFF;
	public sbyte GetOverlayPath(int z, int x, int y) => OverlayPaths[Z,X,Y];
	public sbyte GetOverlayRotation(int z, int x, int y) => OverlayRotations[Z,X,Y];
	public int GetUnderlayID(int z, int x, int y) => ((int) UnderlayIDS[Z,X,Y]) & 0xFF;
	
}