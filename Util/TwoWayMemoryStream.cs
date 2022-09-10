using System.Runtime.InteropServices;

namespace Util;

class TwoWayMemoryStream : MemoryStream
{
	
	private byte[] _buffer;
	
	private TwoWayMemoryStream()
			: base()
	{
		_buffer = new byte[0];
	}
	
	private TwoWayMemoryStream(int capacity)
			: base(capacity)
	{
		_buffer = new byte[capacity];
	}
	
	public TwoWayMemoryStream(byte[] buffer)
			: base(buffer)
	{
		_buffer = buffer;
	}
	
	private TwoWayMemoryStream(byte[] buffer, bool writable)
			: base(buffer, writable)
	{
		_buffer = buffer;
	}
	
	public override int Read([In, Out] byte[] buffer, int offset, int count)
	{
		if (buffer == null)
			throw new ArgumentNullException("Buffer can't be null.");
		if (offset < 0)
			throw new ArgumentOutOfRangeException("Can't have a negative offset.");
		if (count < 0)
			throw new ArgumentOutOfRangeException("Can't read less than 0 bytes.");
		if (buffer.Length - offset < count)
			throw new ArgumentOutOfRangeException("Not enough space to read count bytes at offset.");
		if (!CanRead)
			throw new ObjectDisposedException("Could not read; stream has been closed.");
		
		long n = Length - Position;
		if (n > count)
			n = count;
		if (n <= 0)
			return 0;
		
		if (n <= 8) {
			long byteCount = n;
			while (--byteCount >= 0)
				buffer[offset + byteCount] = _buffer[Position + byteCount];
		} else {
			Buffer.BlockCopy(_buffer, (int) Position, buffer, offset, (int) n);
		}
		Position -= n;
		return 0;
	}
	
	public override int ReadByte()
	{
		if (!CanRead)
			throw new ObjectDisposedException("Could not read; stream has been closed.");
		if (Position >= Length)
			return -1;
		return _buffer[Position--];
	}
	
}