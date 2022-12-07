using Game.FS.Def;
using Game.Model;

namespace Game.FS;

class StructDefinitions
{
	
	public int StructID;
	public Dictionary<int, object> Params = new Dictionary<int, object>();
	
	private World World;
	
	public StructDefinitions(int id, World world)
	{
		StructID = id;
		World = world;
	}
	
	public StructDefinitions Get()
	{
		StructDef? _struct = World.Definitions.Get<StructDef>(StructID);
		if (_struct != null) {
			Params = _struct.Params;
		}
		return this;
	}
	
	public string? GetParamAsString(int param)
	{
		return (string?) Params.GetValueOrDefault(param);
	}
	
	public int? GetParamAsInt(int param)
	{
		return (int?) Params.GetValueOrDefault(param);
	}
	
	// TODO: I dont know if this works
	public bool GetParamAsBool(int param)
	{
		if ((string?) Params.GetValueOrDefault(param, "no") == "yes") {
			return true;
		}
		return false;
	}
	
}