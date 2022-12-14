namespace Util;

static class BitManipulation
{
	
	public static int GetBit(int packed, int startBit, int endBit)
	{
		int position = DataConstants.BIT_SIZES[endBit - startBit];
		return ((packed >> startBit) & position);
	}
	
	public static int SetBit(int packed, int startBit, int endBit, int value)
	{
		int maxValue = DataConstants.BIT_SIZES[endBit - startBit] << startBit;
		return (packed & ~maxValue) | ((value << startBit) & maxValue);
	}
	
}