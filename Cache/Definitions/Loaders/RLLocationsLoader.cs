using Cache.Region;

namespace Cache.Definitions.Loaders;


class RLLocationsLoader
{
	
	private int ReadUnsignedShortSmart(MemoryStream stream)
	{
		BinaryReader reader = new BinaryReader(stream);
		int peek = reader.PeekChar() & 0xFF;
		return peek < 128 ? reader.ReadByte() : reader.ReadUInt16() - 0x8000;
	}
	
	private int ReadUnsignedIntSmartShortCompat(MemoryStream stream)
	{
		int var1 = 0;
		
		int var2;
		for (var2 = ReadUnsignedShortSmart(stream); var2 == 32767; var2 = ReadUnsignedShortSmart(stream))
		{
			var1 += 32767;
		}
		
		var1 += var2;
		return var1;
	}
	
	private void LoadLocations(RLLocationsDefinition loc, byte[] b)
	{
		MemoryStream mem = new MemoryStream(b);
		
		int id = -1;
		int idOffset;
		while ((idOffset = ReadUnsignedIntSmartShortCompat(mem)) != 0)
		{
			id += idOffset;
			
			int position = 0;
			int positionOffset;
			while ((positionOffset = ReadUnsignedShortSmart(mem)) != 0)
			{
				position += positionOffset - 1;
				
				int localY = position & 0x3F;
				int localX = (position >> 6) & 0x3F;
				int height = (position >> 12) & 0x03;
				
				int attributes = mem.ReadByte();
				int type = attributes >> 2;
				int orientation = attributes & 0x03;
				
				loc.Locations.Add(new RLLocation(id, type, orientation, new RLPosition(localX, localY, height)));
			}
		}
	}
	
	public RLLocationsDefinition Load(int regionX, int regionY, byte[] b)
	{
		RLLocationsDefinition loc = new RLLocationsDefinition();
		loc.RegionX = regionX;
		loc.RegionY = regionY;
		LoadLocations(loc, b);
		return loc;
	}
	
}