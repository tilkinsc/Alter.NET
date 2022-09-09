using Game.FS;
using Game.FS.Def;

namespace Game.Model.Entity;

class GameObject : BaseEntity
{
	
	public int ID;
	public byte Settings;
	
	public AttributeMap AttributeMap;
	public TimerMap TimerMap;
	
	public int Type { get => Settings >> 2; }
	public int Rotation { get => Settings & 0x3; }
	
	private GameObject(int id, int settings, Tile tile)
	{
		ID = id;
		Settings = (byte) settings;
		Tile = tile;
	}
	
	public GameObject(int id, int type, int rot, Tile tile)
			: this(id, (type << 2) | rot, tile)
	{
	}
	
	public ObjectDef GetObjectDef(DefinitionSet definitions)
	{
		return definitions.get(typeof(ObjectDef), id);
	}
	
	public bool IsSpawned(World world)
	{
		world.IsSpawned(this);
	}
	
	public int GetTransform(Player player)
	{
		World world = player.World;
		ObjectDef def = GetObjectDef(world.Definitions);
		
		if (def.VarBit != -1) {
			VarbitDef varbitDef = world.Definitions.get(VarbitDef::class.java, def.VarBit);
			var state = player.Barps.GetBit(varbitDef.Varp, varbitDef.StartBit, varbitDef.EndBit);
			return def.Transforms[state];
		}
		
		if (def.Varp != -1) {
			var state = player.Varps.GetState(def.Varp);
			return def.Transforms[state];
		}
		
		return id;
	}
	
}