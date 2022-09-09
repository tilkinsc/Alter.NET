namespace Util;

static class MathExtensions
{
	
	public static double ToRadians(this double deg)
	{
		return deg * (Math.PI / 180.0);
	}
	
	public static double ToDegrees(this double rad)
	{
		return rad * (180.0 / Math.PI);
	}
	
}
