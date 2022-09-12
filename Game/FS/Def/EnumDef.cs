using DotNetty.Buffers;
using Util.IO;

namespace Game.FS.Def;

class EnumDef : Definition
{
	
	public int KeyType = 0;
	public int ValueType = 0;
	public int DefaultInt = 0;
	public string DefaultString = "";
	
	public Dictionary<int, object> Values = new Dictionary<int, object>();
	
	public EnumDef(int id)
			: base(id)
	{
	}
	
	public override void Decode(IByteBuffer buf, int opcode)
	{
		switch (opcode)
		{
			case 1:
			{
				KeyType = buf.ReadByte();
				break;
			}
			case 2:
			{
				ValueType = buf.ReadByte();
				break;
			}
			case 3:
			{
				DefaultString = buf.ReadString();
				break;
			}
			case 4:
			{
				DefaultInt = buf.ReadInt();
				break;
			}
			case 5:
			case 6:
			{
				int count = buf.ReadUnsignedShort();
				for (int i=0; i<count; i++)
				{
					int key = buf.ReadInt();
					if (opcode == 5) {
						Values[key] = buf.ReadString();
					} else {
						Values[key] = buf.ReadInt();
					}
				}
				break;
			}
		}
	}
	
	public int GetInt(int key) => Values.ContainsKey(key) ? (int) Values[key] : DefaultInt;
	public string GetString(int key) => Values.ContainsKey(key) ? (string) Values[key] : DefaultString;
	
}