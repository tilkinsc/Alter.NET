using static Cache.Definitions.RLMapDefinition;

namespace Cache.Definitions.Loaders;


class RLMapLoader
{
	
	public RLMapDefinition Load(int regionX, int regionY, byte[] b)
	{
		RLMapDefinition map = new RLMapDefinition();
		map.RegionX = regionX;
		map.RegionY = regionY;
		LoadTerrain(map, b);
		return map;
	}
	
	private void LoadTerrain(RLMapDefinition map, byte[] buf)
	{
		RLTile[,,] tiles = map.Tiles;
		
		MemoryStream mem = new MemoryStream(buf);
		for (int z=0; z<Z; z++)
		{
			for (int x=0; x<X; x++)
			{
				for (int y=0; y<Y; y++)
				{
					RLTile tile = tiles[z,x,y] = new RLTile();
					while (true)
					{
						int attrib = mem.ReadByte();
						if (attrib == 0) {
							break;
						} else if (attrib == 1) {
							int height = mem.ReadByte();
							tile.Height = height;
							break;
						} else if (attrib <= 49) {
							tile.AttrOpcode = attrib;
							tile.OverlayID = (sbyte) mem.ReadByte();
							tile.OverlayPath = (sbyte) ((attrib - 2) / 4);
							tile.OverlayRotation = (sbyte) (attrib - 2 & 3);
						} else if (attrib <= 81) {
							tile.Settings = (sbyte) (attrib - 49);
						}
					}
				}
			}
		}
	}
	
}