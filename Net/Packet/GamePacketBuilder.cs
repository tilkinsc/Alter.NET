using System.Text;
using DotNetty.Buffers;
using Exceptions;
using Util;

namespace Net.Packet;

class GamePacketBuilder
{
	
	private int BitIndex;
	private IByteBuffer Buffer = Unpooled.Buffer();
	private AccessMode Mode = AccessMode.BYTE_ACCESS;
	private int Opcode = -1;
	private PacketType Type = PacketType.RAW;
	
	public int Length { get { CheckByteAccess(); return Buffer.WriterIndex; } }
	public IByteBuffer ByteBuf { get => Buffer; }
	public int ReadableBytes { get => Buffer.ReadableBytes; }
	
	public GamePacketBuilder()
	{
	}
	
	public GamePacketBuilder(int opcode, PacketType type = PacketType.FIXED)
	{
		Opcode = opcode;
		Type = type;
	}
	
	private void CheckBitAccess()
	{
		if (Mode != AccessMode.BIT_ACCESS)
			throw new IllegalStateException("For bit-based calls to work, bit access needs to be set");
	}
	
	private void CheckByteAccess()
	{
		if (Mode != AccessMode.BYTE_ACCESS)
			throw new IllegalStateException("For byte-based calls to work, byte access needs to be set");
	}
	
	public void Put(DataType type, DataOrder order, DataTransformation transformation, Number value)
	{
		if (type == DataType.SMART)
			throw new IllegalArgumentException("Use PutSmart instead");
		
		CheckByteAccess();
		long longValue = value.Long;
		int length = type.Bytes;
		switch (order)
		{
			case DataOrder.BIG:
				for (int i=length - 1; i>=0; i--)
				{
					if (i == 0 && transformation != DataTransformation.NONE) {
						switch (transformation)
						{
							case DataTransformation.ADD:
								Buffer.WriteByte((byte) (longValue + 128));
								break;
							case DataTransformation.NEGATE:
								Buffer.WriteByte((byte) (-longValue));
								break;
							case DataTransformation.SUBTRACT:
								Buffer.WriteByte((byte) (128 - longValue));
								break;
						}
					} else {
						Buffer.WriteByte((byte) (longValue >> (i * 8)));
					}
				}
				break;
			case DataOrder.LITTLE:
				for (int i=0; i<length; i++)
				{
					if (i == 0 && transformation != DataTransformation.NONE) {
						switch (transformation)
						{
							case DataTransformation.ADD:
								Buffer.WriteByte((byte) (longValue + 128));
								break;
							case DataTransformation.NEGATE:
								Buffer.WriteByte((byte) (-longValue));
								break;
							case DataTransformation.SUBTRACT:
								Buffer.WriteByte((byte) (128 - longValue));
								break;
						}
					} else {
						Buffer.WriteByte((byte) (longValue >> (i * 8)));
					}
				}
				break;
			case DataOrder.MIDDLE:
				if (transformation != DataTransformation.NONE)
					throw new IllegalArgumentException("Middle endian can't be transformed");
				if (type != DataType.INT && type != DataType.MEDIUM)
					throw new IllegalArgumentException("Middle endian can only be used with int and medium values");
				
				if (type == DataType.MEDIUM) {
					Buffer.WriteByte((byte) (longValue >> 16));
					Buffer.WriteByte((byte) (longValue));
					Buffer.WriteByte((byte) (longValue >> 8));
					break;
				} else {
					Buffer.WriteByte((byte) (longValue >> 8));
					Buffer.WriteByte((byte) (longValue));
					Buffer.WriteByte((byte) (longValue >> 24));
					Buffer.WriteByte((byte) (longValue >> 16));
				}
				break;
			case DataOrder.INVERSE_MIDDLE:
				if (transformation != DataTransformation.NONE)
					throw new IllegalArgumentException("Inversed middle endian can't be transformed");
				if (type != DataType.INT && type != DataType.MEDIUM)
					throw new IllegalArgumentException("Inversed middle endian can only be used with integer and medium values");
				
				if (type == DataType.MEDIUM)
				{
					Buffer.WriteByte((byte) (longValue));
					Buffer.WriteByte((byte) (longValue >> 16));
					Buffer.WriteByte((byte) (longValue >> 8));
				}
				else
				{
					Buffer.WriteByte((byte) (longValue >> 16));
					Buffer.WriteByte((byte) (longValue >> 24));
					Buffer.WriteByte((byte) (longValue));
					Buffer.WriteByte((byte) (longValue >> 8));
					
				}
				break;
			default:
				throw new IllegalArgumentException("Unknown order");
		}
	}
	
	public void Put(DataType type, DataOrder order, Number value)
	{
		Put(type, order, DataTransformation.NONE, value);
	}
	
	public void Put(DataType type, DataTransformation transformation, Number value)
	{
		Put(type, DataOrder.BIG, transformation, value);
	}
	
	public void Put(DataType type, Number value)
	{
		Put(type, DataOrder.BIG, DataTransformation.NONE, value);
	}
	
	public void PutBits(int numBits, int value)
	{
		int numberOfBits = numBits;
		if (numberOfBits < 0 || numberOfBits > 32)
			throw new IllegalArgumentException("Number of bits must be between 1 and 32 inclusive");
		
		CheckBitAccess();
		
		int bytePos = BitIndex >> 3;
		int bitOffset = 8 - (BitIndex & 7);
		BitIndex += numberOfBits;
		
		int requiredSpace = bytePos - Buffer.WriterIndex + 1;
		requiredSpace += (numberOfBits + 7) / 8;
		Buffer.EnsureWritable(requiredSpace);
		
		int tmp = 0;
		while (numberOfBits > bitOffset)
		{
			tmp = Buffer.GetByte(bytePos);
			tmp = tmp & ~DataConstants.BIT_MASK[bitOffset];
			tmp = tmp | ((value >> (numberOfBits - bitOffset)) & DataConstants.BIT_MASK[bitOffset]);
			Buffer.SetByte(bytePos++, tmp);
			numberOfBits -= bitOffset;
			bitOffset = 8;
		}
		tmp = Buffer.GetByte(bytePos);
		if (numberOfBits == bitOffset)
		{
			tmp = tmp & ~DataConstants.BIT_MASK[bitOffset];
			tmp = tmp | (value & DataConstants.BIT_MASK[bitOffset]);
			Buffer.SetByte(bytePos, tmp);
		}
		else
		{
			tmp = tmp & ~(DataConstants.BIT_MASK[numberOfBits] << (bitOffset - numberOfBits));
			tmp = tmp | ((value & DataConstants.BIT_MASK[numberOfBits]) << (bitOffset - numberOfBits));
			Buffer.SetByte(bytePos, tmp);
		}
	}
	
	public void PutBit(int value)
	{
		PutBits(1, value);
	}
	
	public void PutBit(bool flag)
	{
		PutBit(flag ? 1 : 0);
	}
	
	public void PutBytes(byte[] bytes, int position, int length)
	{
		for (int i=position; i<position + length; i++)
		{
			Buffer.WriteByte(bytes[i]);
		}
	}
	
	public void PutBytes(byte[] bytes)
	{
		Buffer.WriteBytes(bytes);
	}
	
	public void PutBytes(DataTransformation transformation, byte[] bytes, int position, int length)
	{
		for (int i=position; i<position + length; i++)
		{
			Put(DataType.BYTE, transformation, new Number(bytes[i]));
		}
	}
	
	public void PutBytes(IByteBuffer buffer)
	{
		byte[] bytes = new byte[buffer.ReadableBytes];
		buffer.MarkReaderIndex();
		try
		{
			buffer.ReadBytes(bytes);
		}
		finally
		{
			buffer.ResetReaderIndex();
		}
		PutBytes(bytes);
	}
	
	public void PutBytes(DataTransformation transformation, byte[] bytes)
	{
		if (transformation == DataTransformation.NONE)
		{
			PutBytes(bytes);
		}
		else
		{
			foreach (byte b in bytes)
			{
				Put(DataType.BYTE, transformation, new Number(b));
			}
		}
	}
	
	public void PutBytes(DataTransformation transformation, IByteBuffer buffer)
	{
		byte[] bytes = new byte[buffer.ReadableBytes];
		buffer.MarkReaderIndex();
		try
		{
			buffer.ReadBytes(bytes);
		}
		finally
		{
			buffer.ResetReaderIndex();
		}
		PutBytes(transformation, bytes);
	}
	
	public void PutBytesReverse(byte[] bytes, int length = -1)
	{
		if (length == -1)
			length = bytes.Length;
		CheckByteAccess();
		for (int i=length-1; i>=0; i--)
		{
			Buffer.WriteByte(bytes[i]);
		}
	}
	
	public void PutBytesReverse(IByteBuffer buffer, int length = -1)
	{
		if (length == -1)
			length = buffer.ReadableBytes;
		byte[] bytes = new byte[length];
		buffer.MarkReaderIndex();
		try
		{
			buffer.ReadBytes(bytes);
		}
		finally
		{
			buffer.ResetReaderIndex();
		}
		PutBytesReverse(bytes);
	}
	
	public void PutBytesReverse(DataTransformation transformation, byte[] bytes, int length = -1)
	{
		if (length == -1)
			length = bytes.Length;
		if (transformation == DataTransformation.NONE)
		{
			PutBytesReverse(bytes, length);
		}
		else
		{
			for (int i=length-1; i>=0; i--)
			{
				Put(DataType.BYTE, transformation, new Number(bytes[i]));
			}
		}
	}
	
	public void PutRawBuilder(GamePacketBuilder builder)
	{
		CheckByteAccess();
		if (builder.Type == PacketType.RAW)
			throw new IllegalArgumentException("Builder must be raw.");
		builder.CheckByteAccess();
		PutBytes(builder.Buffer);
	}
	
	public void PutRawBuilderReverse(GamePacketBuilder builder)
	{
		CheckByteAccess();
		if (builder.Type != PacketType.RAW)
			throw new IllegalArgumentException("Builder must be raw.");
		builder.CheckByteAccess();
		PutBytesReverse(builder.Buffer);
	}
	
	public void PutSmart(int value)
	{
		CheckByteAccess();
		if (value >= 0x80)
		{
			Buffer.WriteShort(value + 0x8000);
		}
		else
		{
			Buffer.WriteByte(value);
		}
	}
	
	public void PutString(string str)
	{
		CheckByteAccess();
		byte[] chars = Encoding.ASCII.GetBytes(str);
		for (int i=0; i<chars.Length; i++)
			Buffer.WriteByte(chars[i]);
		Buffer.WriteByte(0);
	}
	
	public void SwitchToBitAccess()
	{
		if (Mode == AccessMode.BIT_ACCESS)
			throw new IllegalStateException("Already in bit access mode.");
		Mode = AccessMode.BIT_ACCESS;
		BitIndex = Buffer.WriterIndex * 8;
	}
	
	public void SwitchToByteAccess()
	{
		if (Mode == AccessMode.BYTE_ACCESS)
			throw new IllegalStateException("Already in byte access mode.");
		Mode = AccessMode.BYTE_ACCESS;
		Buffer.SetWriterIndex((BitIndex + 7) / 8);
	}
	
	public GamePacket ToGamePacket()
	{
		if (Type == PacketType.RAW)
			throw new IllegalStateException("Raw packets cannot be converted to a game packet.");
		if (Mode == AccessMode.BYTE_ACCESS)
			throw new IllegalStateException("Must be in byte access mode to convert to a packet.");
		return new GamePacket(Opcode, Type, Buffer);
	}
	
}