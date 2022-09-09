using System.Runtime.Intrinsics.X86;

namespace Cache.Util;

class RLCRC32
{
	
	public ulong CRC;
	
	public RLCRC32()
	{
		CRC = 0;
	}
	
	public void Update(byte[] data, int start, int length)
	{
		while (length >= 8)
		{
			CRC = Sse42.X64.Crc32(CRC, BitConverter.ToUInt64(data, start));
			start += 8;
			length -= 8;
		}
		while (length > 0)
		{
			CRC = Sse42.X64.Crc32(CRC, data[start]);
			start++;
			length--;
		}
	}
	
	public int Finalize()
	{
		return (int) ~CRC;
	}
	
}