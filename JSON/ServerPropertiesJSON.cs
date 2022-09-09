namespace JSON;

using System.Text.Json.Serialization;


class ServerPropertiesJSON
{
	
	private static ServerPropertiesJSON instance = new ServerPropertiesJSON();
	private static JSONSerializable<ServerPropertiesJSON> Data;
	public static JSONSerializable<ServerPropertiesJSON> Instance { get => Data; }
	public static string Path { get; set; } = "./Data/api.json";
	
	[JsonInclude]
	public Dictionary<string, object> Properties;
	
	public T? Get<T>(string key, T? def = default(T))
	{
		return (T?) (Properties.TryGetValue(key, out object? res) ? res : def);
	}
	
	static ServerPropertiesJSON() {
		Data = new JSONSerializable<ServerPropertiesJSON>(instance, Path);
	}
	
	public ServerPropertiesJSON()
	{
		Properties = new Dictionary<string, object>();
	}
	
}
