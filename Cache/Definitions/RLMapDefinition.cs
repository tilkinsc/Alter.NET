namespace Cache.Definitions;

class RLMapDefinition
{
	public const int X = 64;
	public const int Y = 64;
	public const int Z = 4;
	
	public class RLTile
	{
		public int? Height;
		public int AttrOpcode;
		public sbyte Settings;
		public sbyte OverlayID;
		public sbyte OverlayPath;
		public sbyte OverlayRotation;
		public sbyte UnderlayID;
	}
	
	public int RegionX;
	public int RegionY;
	public RLTile[,,] Tiles = new RLTile[Z,X,Y];
	
}