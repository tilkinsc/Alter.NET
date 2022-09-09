namespace Cache.Region;


class RLLocation
{
	public int ID { get; }
	public int Type { get; }
	public int Orientation { get; }
	public RLPosition Position;
	
	public RLLocation(int id, int type, int orientation, RLPosition position)
	{
		this.ID = id;
		this.Type = type;
		this.Orientation = orientation;
		this.Position = position;
	}
	
}
