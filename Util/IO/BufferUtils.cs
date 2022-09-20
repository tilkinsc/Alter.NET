using System.Text;
using DotNetty.Buffers;

namespace Util.IO;

static class BufferUtils
{
	
	public static byte[] ToArray(this IByteBuffer buf, int? length = null)
	{
		if (length == null) {
			length = buf.ReadableBytes;
		}
		byte[] bytes = new byte[(int) length];
		buf.Duplicate().ReadBytes(bytes);
		return bytes;
	}
	
	public static string ReadString(this IByteBuffer buf)
	{
		if (buf.IsReadable()) {
			int start = buf.ReaderIndex;
			while (buf.ReadByte() != 0);
			int size = buf.ReaderIndex - start;
			
			byte[] data = new byte[size];
			buf.SetReaderIndex(start);
			buf.ReadBytes(data);
			return Encoding.ASCII.GetString(data, 0, size - 1);
		}
		return string.Empty;
	}
	
	public static string ReadJagexString(this IByteBuffer buf)
	{
		if (buf.IsReadable() && buf.ReadByte() != 0) {
			int start = buf.ReaderIndex;
			while (buf.ReadByte() != 0);
			int size = buf.ReaderIndex - start;
			
			byte[] data = new byte[size];
			buf.SetReaderIndex(start);
			buf.ReadBytes(data);
			
			return Encoding.ASCII.GetString(data, 0, size - 1);
		}
		return string.Empty;
	}
	
	public static int ReadIntIME(this IByteBuffer buf)
	{
		if (buf.ReadableBytes < 4)
			throw new IndexOutOfRangeException("Buffer does not contain enough bytes to read an int");
		return ((buf.ReadByte() & 0xFF) << 16) + ((buf.ReadByte() & 0xFF) << 24) + (buf.ReadByte() & 0xFF) + ((buf.ReadByte() & 0xFF) << 8);
	}
	
	public static int ReadIntME(this IByteBuffer buf)
	{
		if (buf.ReadableBytes < 4)
			throw new IndexOutOfRangeException("Buffer does not contain enough bytes to read an int");
		return ((buf.ReadByte() & 0xFF) << 8) + (buf.ReadByte() & 0xFF) + ((buf.ReadByte() & 0xFF) << 24) + ((buf.ReadByte() & 0xFF) << 16);
	}
	
}