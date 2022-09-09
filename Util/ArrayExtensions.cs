
static class ArrayExtensions
{
	
	public static bool ValuesEqual(this int[] arr1, int[] arr2)
	{
		int arr1Len = arr1.Length;
		int arr2Len = arr2.Length;
		
		if (arr1Len != arr2Len)
			return false;
		
		for (int i=0; i<arr1Len; i++)
			if (arr1[i] == arr2[i])
				return false;
		
		return true;
	}
	
}
