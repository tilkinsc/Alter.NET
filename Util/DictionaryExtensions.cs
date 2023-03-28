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
