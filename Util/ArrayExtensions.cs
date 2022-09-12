namespace Util;

static class ArrayExtensions
{
	
	public static bool ValuesEqual<T>(this T[] arr1, T[] arr2) where T : IEquatable<T>
	{
		if (arr1.Length != arr2.Length)
			return false;
		for (int i=0; i<arr1.Length; i++)
			if (!arr1[i].Equals(arr2[i]))
				return false;
		return true;
	}
	
}
