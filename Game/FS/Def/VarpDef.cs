using DotNetty.Buffers;

namespace Game.FS.Def;

class VarpDef : Definition
{
	
	public int ConfigType;
	
	public VarpDef(int id)
		: base(id)
	{
		ConfigType = 0;
	}
	
	public override void Decode(IByteBuffer buf, int opcode)
	{
		if (opcode == 5) {
			ConfigType = buf.ReadUnsignedShort();
		}
	}

}
