namespace JSON;

using System.Text.Json.Serialization;


class DevSettingsJSON
{
	
	private static DevSettingsJSON instance = new DevSettingsJSON();
	private static JSONSerializable<DevSettingsJSON> Data;
	public static JSONSerializable<DevSettingsJSON> Instance { get => Data; }
	public static string Path { get; set; } = "./Data/dev-settings.json";
	
	[JsonInclude]
	public bool DebugExamines;
	[JsonInclude]
	public bool DebugObjects;
	[JsonInclude]
	public bool DebugButtons;
	[JsonInclude]
	public bool DebugItemActions;
	[JsonInclude]
	public bool DebugMagicSpells;
	
	static DevSettingsJSON() {
		Data = new JSONSerializable<DevSettingsJSON>(instance, Path);
	}
	
	public DevSettingsJSON()
	{
	}
	
}
