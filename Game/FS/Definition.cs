using System.Text;

namespace Game.FS;

static class BinaryReaderExtensions
{
	
	// TODO: check for accuracy
	
	public static int ReadUnsignedMedium(this BinaryReader stream)
	{
		ushort firstTwo = stream.ReadUInt16();
		byte last = stream.ReadByte();
		return (last << 16) | firstTwo;
	}
	
}

abstract class Definition
{
	
	public int ID;
	
	public Definition(int id)
	{
		ID = id;
	}
	
	public void Decode(MemoryStream buffer)
	{
		BinaryReader stream = new BinaryReader(buffer);
		
		while (true)
		{
			int opcode = ((int) stream.ReadByte());
			if (opcode == 0)
				break;
			Decode(buffer, opcode);
		}
	}
	
	public virtual void Decode(MemoryStream buffer, int opcode) {}
	
	// TODO: check for accuracy
	public Dictionary<int, object> ReadParams(MemoryStream buffer)
	{
		Dictionary<int, object> map = new Dictionary<int, object>();
		
		BinaryReader stream = new BinaryReader(buffer, Encoding.ASCII, true);
		
		int length = (int) stream.ReadByte();
		for (int i=0; i<length; i++)
		{
			bool isString = ((int) stream.ReadByte()) == 1;
			int id = stream.ReadUnsignedMedium();
			if (isString) {
				map[id] = stream.ReadString();
			} else {
				map[id] = stream.ReadInt32();
			}
		}
		
		return map;
	}
	
}
