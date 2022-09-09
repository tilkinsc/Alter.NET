namespace Util;

static class DictionaryExtensions
{
	
	#nullable disable
	
	// merges one dictionary into caller dict
	public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> dict, params Dictionary<TKey, TValue>[] dicts)
	{
		for (int i=0; i<dicts.Length; i++)
		{
			foreach (KeyValuePair<TKey, TValue> pair in dicts[i])
			{
				dict[pair.Key] = pair.Value;
			}
		}
	}
	
	// Returns if value was added at key inside of list
	public static bool AddIfNotExist<TValue>(this List<TValue> list, TValue val)
	{
		if (!list.Contains(val)) {
			list.Add(val);
			return true;
		}
		return false;
	}
	
	// Removes an element or does nothing if it doesn't exist
	public static bool RemoveIfNotExist<TKey, TDataType>(this Dictionary<TKey, TDataType> dict, TKey key)
	{
		if (dict.ContainsKey(key)) {
			dict.Remove(key);
			return true;
		}
		return false;
	}
	
	#nullable enable
	
}
