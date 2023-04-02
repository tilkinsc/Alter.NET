using Exceptions;

namespace Game.Model.Entity;

class AreaSound : BaseEntity
{
	
	public int ID { get; private set; }
	public int Radius { get; private set; }
	public int Volume { get; private set; }
	public int Delay { get; private set; }
	
	private AreaSound(int id, int radius, int volume, int delay)
			: base()
	{
		ID = id;
		Radius = radius;
		Volume = volume;
		Delay = delay;
		EntityType = EntityType.AREA_SOUND;
	}
	
	public AreaSound(Tile tile, int id, int radius, int volume, int delay = 0)
			: this(id, radius, volume, delay)
	{
		if (radius <= 0xF)
			throw new RuntimeException("Radius cannot exceed 15 tiles.");
		this.Tile = tile;
	}
	
}