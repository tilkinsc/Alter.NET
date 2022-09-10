namespace Util;

static class MemoryStreamExtensions
{
	
	public static long GetRemaining(this MemoryStream buffer)
	{
		long compute = buffer.Length - buffer.Position;
		if (compute < 0)
			return 0;
		return compute;
	}
	
	public static bool IsReadable(this MemoryStream buffer)
	{
		return buffer.GetRemaining() > 0;
	}
	
}