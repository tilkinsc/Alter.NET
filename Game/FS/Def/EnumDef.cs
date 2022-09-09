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
	
	public override void Decode(MemoryStream buffer, int opcode)
	{
		BinaryReader stream = new BinaryReader(buffer);
		switch (opcode)
		{
			case 1:
			{
				KeyType = stream.ReadByte();
				break;
			}
			case 2:
			{
				ValueType = stream.ReadByte();
				break;
			}
			case 3:
			{
				DefaultString = stream.ReadString();
				break;
			}
			case 4:
			{
				DefaultInt = stream.ReadInt32();
				break;
			}
			case 5:
			case 6:
			{
				int count = stream.ReadUInt16();
				for (int i=0; i<count; i++)
				{
					int key = stream.ReadInt32();
					if (opcode == 5) {
						Values[key] = stream.ReadString();
					} else {
						Values[key] = stream.ReadInt32();
					}
				}
				break;
			}
		}
	}
	
}