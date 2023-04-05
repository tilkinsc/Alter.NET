using DotNetty.Buffers;
using Exceptions;
using Util.IO;

namespace Net.Packet;

class GamePacketReader
{
	
	private int _bitIndex = 0;
	private IByteBuffer _buffer; // TODO: set in constructor
	private AccessMode _mode = AccessMode.BYTE_ACCESS;
	public int Bit { get => GetBits(1); }
	public int Length { get { CheckByteAccess(); return _buffer.WritableBytes; } }
	public int ReadableBytes { get { CheckByteAccess(); return _buffer.ReadableBytes; } }
	
	public int SignedSmart
	{
		get
		{
			CheckByteAccess();
			byte peek = _buffer.GetByte(_buffer.ReaderIndex);
			if (peek < 128)
			{
				
				return _buffer.ReadByte() - 64;
			}
			else
			{
				return _buffer.ReadShort() - 49152;
			}
		}
	}
	
	public string String { get { CheckByteAccess(); return _buffer.ReadString(); } }
	public string JagString { get { CheckByteAccess(); return _buffer.ReadJagexString(); } }
	
	public int UnsignedSmart {
		get
		{
			CheckByteAccess();
			byte peek = _buffer.GetByte(_buffer.ReaderIndex);
			if (peek < 128)
			{
				return _buffer.ReadByte();
			}
			else
			{
				return _buffer.ReadUnsignedShort() - 32768;
			}
		}
	}
	
	public GamePacket Packet;
	
	public GamePacketReader(GamePacket packet)
	{
		Packet = packet;
		_buffer = packet.Payload;
	}
	
	private void CheckBitAccess()
	{
		if (_mode != AccessMode.BIT_ACCESS)
			throw new IllegalStateException("For bit-based calls to work, the mode must be bit access.");
	}
	
	private void CheckByteAccess()
	{
		if (_mode != AccessMode.BYTE_ACCESS)
			throw new IllegalStateException("For byte-based calls to work, the mode must be byte access.");
	}
	
	
	
	
}
