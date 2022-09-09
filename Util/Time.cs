namespace Util;

static class Time
{
	
	public static long CurrentTimeMillis()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
	
}
