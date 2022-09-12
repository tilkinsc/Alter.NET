using DotNetty.Buffers;

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

	public override void Decode(IByteBuffer buf, int opcode)
	{
		if (opcode == 1) {
			Varp = buf.ReadUnsignedShort();
			StartBit = buf.ReadByte();
			EndBit = buf.ReadByte();
		}
	}

}
