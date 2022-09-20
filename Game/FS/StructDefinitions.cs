namespace Game.FS;

class StructDefinitions
{
	
	public int StructID;
	public Dictionary<int, object?> Params = new Dictionary<int, object?>();
	
	private World World;
	
	public StructDefinitions(int id, World world)
	{
		StructID = id;
		World = world;
	}
	
	public StructDefinitions Get()
	{
		var? _struct = World.Definitions.GetNullable(typeof(StructDef), StructID);
		if (_struct != null) {
			Params = _struct.Params;
		}
		return this;
	}
	
	public string? GetParamAsString(int param)
	{
		return (string?) Params.GetValueOrDefault(param, null);
	}
	
	public int? GetParamAsInt(int param)
	{
		return (int?) Params.GetValueOrDefault(param, null);
	}
	
	// TODO: I dont know if this works
	public bool GetParamAsBool(int param)
	{
		return GetParamAsString(param) == "yes";
	}
	
}