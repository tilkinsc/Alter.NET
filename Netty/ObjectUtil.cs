namespace Netty;

static class ObjectUtil
{
	
	public static T CheckNotNull<T>(T? arg, string text)
	{
		if (arg == null)
			throw new NullReferenceException(text);
		if (arg is string && string.IsNullOrEmpty(arg as string))
			throw new NullReferenceException(text);
		return arg;
	}
	
	public static int CheckPositive(int arg, string text)
	{
		if (arg < 0)
			throw new ArgumentOutOfRangeException(text);
		return arg;
	}
	
	public static long CheckPositive(long arg, string text)
	{
		if (arg < 0)
			throw new ArgumentOutOfRangeException(text);
		return arg;
	}
	
}