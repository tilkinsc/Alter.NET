namespace Util;

static class DataConstants
{
	
	public static readonly int[] BIT_MASK = new int[32];
	public static readonly int[] BIT_SIZES = new int[32];
	
	// TODO: ensure data is constant with DataConstants.kt
	static DataConstants()
	{
		int size = 2;
		for (int i=0; i<32; i++)
		{
			BIT_MASK[i] = (1 << i) - 1;
			BIT_SIZES[i] = size - 1;
			size += size;
		}
	}
	
}