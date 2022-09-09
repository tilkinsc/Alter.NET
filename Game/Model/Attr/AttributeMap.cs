namespace Game.Model.Attr;

class AttributeMap
{
	
	private Dictionary<AttributeKey, object> Attributes = new Dictionary<AttributeKey, object>();
	
	public T? Get<T>(AttributeKey<T> key)
	{
		Attributes.TryGetValue(key, out object? val);
		return (T?) val;
	}
	
	public T? GetOrDefault<T>(AttributeKey<T> key, T def)
	{
		T? val = Get<T>(key);
		if (val == null)
			return def;
		return val;
	}
	
	public void Set<T>(AttributeKey<T> key, T val) where T : notnull => Attributes[key] = (object) val;
	public void Remove<T>(AttributeKey<T> key) => Attributes.Remove(key);
	public bool Has<T>(AttributeKey<T> key) => Attributes.ContainsKey(key);
	public bool IsEmpty() => !Attributes.Any();
	public void Clear() => Attributes.Clear();
	
	public Dictionary<string, AttributeKey> ToPersistentMap()
	{
		Dictionary<string, AttributeKey> map = new Dictionary<string, AttributeKey>();
		foreach (KeyValuePair<AttributeKey, object> attrib in Attributes)
		{
			if (attrib.Key.PersistenceKey == null || attrib.Key.Temporary)
				continue;
			map.Add(attrib.Key.PersistenceKey, attrib.Key);
		}
		return map;
	}
	
}