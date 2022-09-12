using Util.IO;
using DotNetty.Buffers;

namespace Game.FS;

abstract class Definition
{
	
	public int ID;
	
	public Definition(int id)
	{
		ID = id;
	}
	
	public void Decode(IByteBuffer buf)
	{
		while (true)
		{
			int opcode = buf.ReadByte();
			if (opcode == 0)
				break;
			Decode(buf, opcode);
		}
	}
	
	public virtual void Decode(IByteBuffer buffer, int opcode) {}
	
	public Dictionary<int, object> ReadParams(IByteBuffer buf)
	{
		Dictionary<int, object> map = new Dictionary<int, object>();
		int length = (int) buf.ReadByte();
		for (int i=0; i<length; i++)
		{
			bool isString = buf.ReadByte() == 1;
			int id = buf.ReadUnsignedMedium();
			if (isString) {
				map[id] = buf.ReadString();
			} else {
				map[id] = buf.ReadInt();
			}
		}
		return map;
	}
	
}
