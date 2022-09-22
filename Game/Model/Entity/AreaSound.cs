namespace Game.Model.Entity;

class AreaSound : BaseEntity
{
	
	public int ID;
	public int Radius;
	public int Volume;
	public int Delay;
	
	public AreaSound(Tile tile, int id, int radius, int volume, int delay = 0)
			: base(tile, EntityType.AREA_SOUND)
	{
		ID = id;
		Radius = radius;
		Volume = volume;
		Delay = delay;
	}
	
}