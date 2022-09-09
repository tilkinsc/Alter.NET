using Util;

namespace Game.FS.Def;

class StructDef : Definition
{
	
	public Dictionary<int, object> Params = new Dictionary<int, object>();
	
	public StructDef(int id)
		: base(id)
	{
	}

	public override void Decode(MemoryStream buffer, int opcode)
	{
		if (opcode == 249) {
			Params.Merge<int, object>(ReadParams(buffer));
		}
	}

}
