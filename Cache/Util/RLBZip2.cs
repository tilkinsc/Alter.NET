
namespace Cache.Util;

class RLBZip2
{
	
	private static byte[] BZIP_HEADER = new byte[]
	{
		(byte) 'B', (byte) 'Z', // magic
		(byte) 'h',      // 'h' for Bzip2 ('H'uffman coding)
		(byte) '1'
	};
	
	public static byte[] Compress(byte[] bytes)
	{
		MemoryStream input = new MemoryStream(bytes);
		MemoryStream output = new MemoryStream();
		
		ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(input, output, false, 9);
		
		return output.ToArray().Skip(BZIP_HEADER.Length).ToArray();
	}
	
	public static byte[] Decompress(byte[] bytes, int len)
	{
		MemoryStream input = new MemoryStream();
		input.Write(BZIP_HEADER);
		input.Write(bytes);
		
		MemoryStream output = new MemoryStream();
		
		ICSharpCode.SharpZipLib.BZip2.BZip2.Decompress(input, output, false);
		
		return output.ToArray();
	}
	
}