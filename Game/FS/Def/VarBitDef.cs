namespace Game.FS.Def;

class VarBitDef : Definition
{
	
	public int Varp;
	public int StartBit;
	public int EndBit;
	
	public VarBitDef(int id)
		: base(id)
	{
		Varp = 0;
		StartBit = 0;
		EndBit = 0;
	}

	public override void Decode(MemoryStream buffer, int opcode)
	{
		if (opcode == 1) {
			BinaryReader stream = new BinaryReader(buffer);
			Varp = stream.ReadUInt16();
			StartBit = stream.ReadByte();
			EndBit = stream.ReadByte();
		}
	}

}
