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
					Buffer.WriteByte((byte) ());
					Buffer.WriteByte((byte) ());
					Buffer.WriteByte((byte) ());
				}
				break;
			case DataOrder.INVERSE_MIDDLE:
				
				break;
			default:
				throw new IllegalArgumentException("Unknown order");
		}
	}
	
}