using System.Text.Json;
using Util;

namespace JSON;

class JSONSerializable<T> where T : new()
{
	public bool IsLoadFailed { get; private set; } = false;
	public bool IsLoaded { get; private set; } = false;
	
	public T Handle { get; protected set; }
	
	public string Path { get; set; }
	
	public JSONSerializable(T data, string path) {
		Handle = data;
		this.Path = path;
	}
	
	public void Save()
	{
		if (Handle == null)
			throw new NullReferenceException($"JSONSerializable<{typeof(T).FullName}> was never successfully initialized. It should be initializable without parameters.");
		JsonSerializerOptions jso = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
		byte[] json = JsonSerializer.SerializeToUtf8Bytes<T>(Handle, jso);
		string? dir = System.IO.Path.GetDirectoryName(Path);
		if (dir != null && !File.Exists(dir))
			File.Create(dir);
		if (!File.Exists(Path))
			File.Create(Path);
		File.WriteAllBytes(Path, json);
	}
	
	public void Load(bool forced = false)
	{
		if (IsLoaded && !forced)
			return;
		
		try {
			using (StreamReader sr = new StreamReader(Path)) {
				string json = sr.ReadToEnd();
				var handle = JsonSerializer.Deserialize<T>(json);
				if (handle != null)
					Handle = handle;
				IsLoaded = true;
				return;
			}
		} catch (Exception e) {
			Console.Error.WriteLine($"'{typeof(T).FullName}' loading JSON failed on file '{Path}'");
			Console.Error.WriteLine(e.StackTrace);
			Logger.Error($"'{typeof(T).FullName}' loading JSON file failed on '{Path}'.\n{e}");
			Handle = new T();
		}
	}
	
}

