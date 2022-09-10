namespace Util;

static class StreamExtensions
{
	
	public static long GetRemaining(this Stream buffer)
	{
		long compute = buffer.Length - buffer.Position;
		if (compute < 0)
			return 0;
		return compute;
	}
	
	public static bool IsReadable(this Stream buffer)
	{
		return buffer.GetRemaining() > 0;
	}
	
}