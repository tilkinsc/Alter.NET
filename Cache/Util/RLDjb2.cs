namespace Cache.Util;

static class RLDjb2
{
	
	public static int Hash(string str)
	{
		int hash = 0;
		for (int i=0; i<str.Length; i++)
		{
			hash = str[i] + ((hash << 5) - hash);
		}
		return hash;
	}
	
}
