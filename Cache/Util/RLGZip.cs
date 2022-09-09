using System.IO.Compression;

namespace Cache.Util;

class RLGZip
{
	
	public static byte[] Compress(byte[] bytes)
	{
		MemoryStream output = new MemoryStream();
		
		using (GZipStream gzip = new GZipStream(output, CompressionLevel.SmallestSize))
		{
			gzip.Write(bytes);
		}
		
		return output.ToArray();
	}
	
	public static byte[] Decompress(byte[] bytes, int len)
	{
		MemoryStream output = new MemoryStream();
		
		int offset = 0;
		using (GZipStream gzip = new GZipStream(output, CompressionLevel.SmallestSize))
		{
			byte[] buffer = new byte[4096];
			output.Write(buffer, offset, gzip.Read(buffer, offset, len));
		}
		
		return output.ToArray();
	}
	
}