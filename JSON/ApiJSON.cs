namespace JSON;

using System.Text.Json.Serialization;


class ApiJSON
{
	
	private static ApiJSON instance = new ApiJSON();
	private static JSONSerializable<ApiJSON> Data;
	public static JSONSerializable<ApiJSON> Instance { get => Data; }
	public static string Path { get; set; } = "./Data/api.json";
	
	[JsonInclude]
	public string Org;
	[JsonInclude]
	public string OrgWebsite;
	
	static ApiJSON() {
		Data = new JSONSerializable<ApiJSON>(instance, Path);
	}
	
	public ApiJSON()
	{
		Org = "RSPS";
		OrgWebsite = "http://127.0.0.1:80";
	}
	
}
