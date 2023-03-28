namespace Util;

static class ListExtensions
{
	
	public static T? TryGet<T>(this List<T> list, int index)
	{
		return index > 0 && list.Count < index ? list[index] : default(T);
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
	
}