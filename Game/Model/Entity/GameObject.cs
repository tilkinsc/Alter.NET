using Game.FS;
using Game.FS.Def;
using Game.Model.Attr;
using Game.Model.Timer;

namespace Game.Model.Entity;

class GameObject : BaseEntity
{
	
	public int ID { get; private set; }
	public byte Settings { get; private set; }
	
	public AttributeMap AttributeMap { get; private set; } = new AttributeMap();
	public TimerMap TimerMap { get; private set; } = new TimerMap();
	
	public int Type { get => Settings >> 2; }
	public int Rotation { get => Settings & 0x3; }
	
	private GameObject(int id, int settings, Tile tile)
			: base(tile)
	{
		ID = id;
		Settings = (byte) settings;
		Tile = tile;
	}
	
	public GameObject(int id, int type, int rot, Tile tile)
			: this(id, (type << 2) | rot, tile)
	{
	}
	
	public ObjectDef? GetObjectDef(DefinitionSet definitions)
	{
		return definitions.Get<ObjectDef>(ID);
	}
	
	public bool IsSpawned(World world) => world.IsSpawned(this);
	
	public int GetTransform(Player player)
	{
		World world = player.World;
		ObjectDef? def = GetObjectDef(world.Definitions);
		
		if (def.VarBit != -1)
		{
			VarBitDef? varbitDef = world.Definitions.Get<VarBitDef>(def.VarBit);
			int state = player.Varps.GetBit(varbitDef.Varp, varbitDef.StartBit, varbitDef.EndBit);
			return def.Transforms[state];
		}
		
		if (def.Varp != -1)
		{
			var state = player.Varps.GetState(def.Varp);
			return def.Transforms[state];
		}
		
		return ID;
	}
	
	public override string ToString()
	{
		return $"ToString not implemented";
	}
	
}