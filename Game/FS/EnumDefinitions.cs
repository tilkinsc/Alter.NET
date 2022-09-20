namespace Game.FS;

class EnumDefinitions
{
	
	public int EnumID;
	public Dictionary<int, object?> Values = new Dictionary<int, object?>();
	
	private World World;
	
	public EnumDefinitions(int id, World world)
	{
		EnumID = id;
		World = world;
	}
	
	public EnumDefinitions? Get()
	{
		var? _struct = World.Definitions.GetNullable(typeof(EnumDef), EnumID);
		if (_struct != null) {
			Values = _struct.Values;
		}
		return this;
	}
	
	public string? GetValueAsString(int param)
	{
		return (string?) Values.GetValueOrDefault(param, null);
	}
	
	public int? GetValueAsInt(int param)
	{
		return (int?) Values.GetValueOrDefault(param, null);
	}
	
	// TODO: I dont know if this works
	public bool GetValueAsBool(int param)
	{
		return GetValueAsString(param) == "yes";
	}
	
}