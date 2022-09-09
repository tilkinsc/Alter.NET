using Cache.FS.Jagex;
using Cache.Util;
using Exceptions;

namespace Cache.FS;

class RLContainer
{
	
	public int Compression;
	public int Revision;
	public int CRC;
	public byte[]? Data;
	
	public RLContainer(int compression, int revision)
	{
		Compression = compression;
		Revision = revision;
	}
	
	private byte[] Concat(params byte[][] arr)
	{
		int count = 0;
		for (int i=0; i<arr.Length; i++)
			count += arr[i].Length;
		byte[] array = new byte[count];
		int cursor = 0;
		for (int i=0; i<arr.Length; i++)
		{
			Array.Copy(arr[i], 0, array, cursor, arr[i].Length);
			cursor += arr[i].Length;
		}
		return array;
	}
	
	public void Compress(byte[] data, int[]? keys)
	{
		MemoryStream mem = new MemoryStream();
		BinaryWriter stream = new BinaryWriter(mem);
		
		byte[] compressedData;
		int length;
		switch (Compression)
		{
			case RLCompressionType.NONE:
				compressedData = data;
				length = compressedData.Length;
				break;
			case RLCompressionType.BZ2:
				compressedData = Concat(BitConverter.GetBytes(data.Length), RLBZip2.Compress(data));
				length = compressedData.Length - 4;
				break;
			case RLCompressionType.GZ:
				compressedData = Concat(BitConverter.GetBytes(data.Length), RLGZip.Compress(data));
				length = compressedData.Length - 4;
				break;
			default:
				throw new RuntimeException("Unknown compression type");
		}
		
		compressedData = Encrypt(compressedData, compressedData.Length, keys);
		
		stream.Write((byte) Compression);
		stream.Write((int) length);
		
		stream.Write(compressedData);
		if (Revision != -1) {
			stream.Write((short) Revision);
		}
		
		byte[] array = mem.ToArray();
		Array.Reverse(array);
		Data = array;
	}
	
	public static RLContainer? Decompress(byte[] b, int[]? keys)
	{
		MemoryStream mem = new MemoryStream(b);
		BinaryReader stream = new BinaryReader(mem);
		
		int compression = (byte) stream.Read();
		int compressedLength = stream.ReadInt32();
		if (compressedLength < 0 || compressedLength > 1000000) {
			throw new RuntimeException("Invalid data");
		}
		
		RLCRC32 crc32 = new RLCRC32();
		crc32.Update(b, 0, 5); // compression + length
		
		byte[] data;
		int revision = -1;
		switch (compression)
		{
			case RLCompressionType.NONE:
			{
				byte[] encryptedData = stream.ReadBytes(compressedLength);
				
				crc32.Update(encryptedData, 0, compressedLength);
				
				byte[] decryptedData = Decrypt(encryptedData, encryptedData.Length, keys);
				
				if (mem.Length >= 2) {
					revision = (int) stream.ReadUInt16();
					// assert revision != -1;
				}
				
				data = decryptedData;
				
				break;
			}
			case RLCompressionType.BZ2:
			{
				byte[] encryptedData = stream.ReadBytes(compressedLength + 4);
				
				crc32.Update(encryptedData, 0, encryptedData.Length);
				byte[] decryptedData = Decrypt(encryptedData, encryptedData.Length, keys);
				
				if (mem.Length >= 2) {
					revision = stream.ReadUInt16();
					// assert revision != -1;
				}
				
				MemoryStream mem2 = new MemoryStream(decryptedData);
				stream = new BinaryReader(mem2);
				
				int decompressedLength = stream.ReadInt32();
				data = RLGZip.Decompress(mem2.ToArray(), compressedLength);
				
				if (data == null)
					return null;
				
				// assert data.Length == decompressedLength;
				
				break;
			}
			default:
				throw new RuntimeException("Unknown decompression type");
		}
		
		RLContainer container = new RLContainer(compression, revision);
		container.Data = data;
		container.CRC = crc32.Finalize();
		return container;
	}
	
	private static byte[] Decrypt(byte[] data, int length, int[]? keys)
	{
		if (keys == null)
			return data;
		
		RLXtea xtea = new RLXtea(keys);
		return xtea.Decrypt(data, length);
	}
	
	private static byte[] Encrypt(byte[] data, int length, int[]? keys)
	{
		if (keys == null)
			return data;
		
		RLXtea xtea = new RLXtea(keys);
		return xtea.Encrypt(data, length);
	}
	
}
