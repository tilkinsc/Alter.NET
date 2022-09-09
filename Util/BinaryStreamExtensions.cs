using Exceptions;

namespace Util;

static class BinaryReaderExtensions
{
	
	public static int ReadBigSmart(this BinaryReader stream)
	{
		return stream.PeekChar() >= 0
			? (stream.ReadUInt16() & 0xFFFF)
			: (stream.ReadInt32() & int.MaxValue);
	}
	
}

static class BinaryWriterExtensions
{
	
	public static void WriteBigSmart(this BinaryWriter stream, int value)
	{
		if (value < 0)
			throw new IllegalArgumentException();
		
		if (value < 128) {
			stream.Write((byte) value);
		} else {
			stream.Write((short) (0x8000 | (short) value));
		}
	}
	
}