namespace Game.Model;

class Area
{
	
	public int BottomLeftX;
	public int BottomLeftZ;
	public int TopRightX;
	public int TopRightZ;
	
	public Tile Center => new Tile(BottomLeftX + (TopRightX - BottomLeftX), BottomLeftZ + (TopRightZ - BottomLeftZ));
	public Tile BottomLeft => new Tile(BottomLeftX, BottomLeftZ);
	public Tile TopRight => new Tile(TopRightX, TopRightZ);
	
	public Area(int bottomLeftX, int bottomLeftZ, int topRightX, int topRightZ)
	{
		BottomLeftX = bottomLeftX;
		BottomLeftZ = bottomLeftZ;
		TopRightX = topRightX;
		TopRightZ = topRightZ;
	}
	
	public bool Contains(int x, int z)
	{
		return (x >= BottomLeftX) && (x <= TopRightX) && (z >= BottomLeftZ && z <= TopRightZ);
	}
	
	public bool Contains(Tile tile)
	{
		return Contains(tile.X, tile.Z);
	}
	
}