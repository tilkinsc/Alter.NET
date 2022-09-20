/*
 * Used as a shim for Java's Number class
 */

namespace Util;

enum NumberType
{
	SIGNED,
	UNSIGNED,
	FLOAT,
	DOUBLE
}

class Number
{
	
	public ulong Raw;
	public NumberType Type;
	
	private Number() {}
	private Number(ulong raw, NumberType type)
	{
		Raw = raw;
		Type = type;
	}
	
	public Number(Number value)
	{
		Raw = value.Raw;
	}
	
	public Number(sbyte value) : this((ulong) value, NumberType.SIGNED) {}
	public Number(byte value) : this(value, NumberType.UNSIGNED) {}
	public Number(short value) : this((ulong) value, NumberType.SIGNED) {}
	public Number(ushort value) : this(value, NumberType.UNSIGNED) {}
	public Number(int value) : this((ulong) value, NumberType.SIGNED) {}
	public Number(uint value) : this(value, NumberType.UNSIGNED) {}
	public Number(long value) : this((ulong) value, NumberType.SIGNED) {}
	public Number(ulong value) : this(value, NumberType.UNSIGNED) {}
	public unsafe Number(float value) { Raw = *(uint*) &value; Type = NumberType.FLOAT; }
	public unsafe Number(double value) { Raw = *(ulong*) &value; Type = NumberType.DOUBLE; }
	
	public sbyte SByte => (sbyte) (Raw & 0xFF);
	public byte Byte => (byte) (Raw & 0xFF);
	public short Short => (short) (Raw & 0xFFFF);
	public ushort UShort => (ushort) (Raw & 0xFFFF);
	public int Int => (int) (Raw & 0xFFFFFFFF);
	public uint UInt => (uint) (Raw & 0xFFFFFFFF);
	public long Long => (long) Raw;
	public ulong ULong => Raw;
	public unsafe float Float { get { ulong raw = Raw; return *(float*) &raw; } }
	public unsafe double Double { get { ulong raw = Raw; return *(double*) &raw; } }
	
}

static class NumberExtensions
{
	
	public static Number ToNumber(this sbyte obj) => new Number(obj);
	public static Number ToNumber(this byte obj) => new Number(obj);
	public static Number ToNumber(this short obj) => new Number(obj);
	public static Number ToNumber(this ushort obj) => new Number(obj);
	public static Number ToNumber(this int obj) => new Number(obj);
	public static Number ToNumber(this uint obj) => new Number(obj);
	public static Number ToNumber(this long obj) => new Number(obj);
	public static Number ToNumber(this ulong obj) => new Number(obj);
	public static Number ToNumber(this float obj) => new Number(obj);
	public static Number ToNumber(this double obj) => new Number(obj);
	
}
