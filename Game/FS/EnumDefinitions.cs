using Game.FS.Def;
using Game.Model;

namespace Game.FS;

class EnumDefinitions
{
	
	public int EnumID;
	public Dictionary<int, object> Values = new Dictionary<int, object>();
	
	private World World;
	
	public EnumDefinitions(int id, World world)
	{
		EnumID = id;
		World = world;
	}
	
	public EnumDefinitions? Get()
	{
		EnumDef? _struct = World.Definitions.Get<EnumDef>(EnumID);
		if (_struct != null) {
			Values = _struct.Values;
		}
		return this;
	}
	
	public string? GetValueAsString(int param)
	{
		return (string?) Values.GetValueOrDefault(param);
	}
	
	public int? GetValueAsInt(int param)
	{
		return (int?) Values.GetValueOrDefault(param);
	}
	
	public bool GetValueAsBool(int param)
	{
		if ((string?) Values.GetValueOrDefault(param, "no") == "yes") {
			return true;
		}
		return false;
	}
	
}