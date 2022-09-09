using Util;

namespace Game.FS.Def;

class NpcDef : Definition
{
	
	public string Name = "";
	public int Category = -1;
	public int Size = 1;
	public int StandAnim = -1;
	public int WalkAnim = -1;
	public int RotateLeftAnim = -1;
	public int RotateRightAnim = -1;
	public int Rotate180Anim = -1;
	public int Rotate90AnimCW = -1;
	public int Rotate90AnimCCW = -1;
	public bool IsMinimapVisible = true;
	public int CombatLevel = -1;
	public int WidthScale = -1;
	public int heightScale = -1;
	public int Length = -1;
	public bool Render = false;
	public int HeadIcon = -1;
	public int Varp = -1;
	public int VarBit = -1;
	public bool Interactable = true;
	public bool Pet = false;
	public List<string?> Options = new List<string?>(new string?[] { "", "", "", "", "" });
	public List<int>? Transforms = null;
	public List<int>? ChatHeadModels = null;
	public Dictionary<int, object> Params = new Dictionary<int, object>();
	public string? Examine = null;
	
	public NpcDef(int id)
			: base(id)
	{
	}
	
	public bool IsAttackable()
	{
		return CombatLevel > 0 && Options.Contains("Attack");
	}
	
	public override void Decode(MemoryStream buffer, int opcode)
	{
		BinaryReader stream = new BinaryReader(buffer);
		switch (opcode)
		{
			case 1:
			{
				int count = stream.ReadByte();
				for (int i=0; i<count; i++)
					stream.ReadByte();
				break;
			}
			case 2:
			{
				Name = stream.ReadString();
				break;
			}
			case 12:
			{
				Size = stream.ReadByte();
				break;
			}
			case 13:
			{
				StandAnim = stream.ReadUInt16();
				break;
			}
			case 14:
			{
				WalkAnim = stream.ReadUInt16();
				break;
			}
			case 15:
			{
				RotateLeftAnim = stream.ReadUInt16();
				break;
			}
			case 16:
			{
				RotateRightAnim = stream.ReadUInt16();
				break;
			}
			case 17:
			{
				WalkAnim = stream.ReadUInt16();
				Rotate180Anim = stream.ReadUInt16();
				Rotate90AnimCW = stream.ReadUInt16();
				Rotate90AnimCCW = stream.ReadUInt16();
				break;
			}
			case 18:
			{
				Category = stream.ReadUInt16();
				break;
			}
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			{
				string read = stream.ReadString();
				if (read.Equals("null") || read.Equals("hidden")) {
					Options[opcode - 30] = null;
					break;
				}
				Options[opcode - 30] = read;
				break;
			}
			case 40:
			case 41:
			{
				int count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					stream.ReadUInt16();
					stream.ReadUInt16();
				}
				break;
			}
			case 60:
			{
				int count = stream.ReadByte();
				ChatHeadModels = new List<int>(count);
				for (int i=0; i<count; i++)
				{
					ChatHeadModels.Add(stream.ReadUInt16());
				}
				break;
			}
			case 93:
			{
				IsMinimapVisible = false;
				break;
			}
			case 95:
			{
				CombatLevel = stream.ReadUInt16();
				break;
			}
			case 97:
			{
				WidthScale = stream.ReadUInt16();
				break;
			}
			case 98:
			{
				heightScale = stream.ReadUInt16();
				break;
			}
			case 99:
			{
				Render = true;
				break;
			}
			case 100:
			case 101:
			{
				stream.ReadByte();
				break;
			}
			case 102:
			{
				HeadIcon = stream.ReadUInt16();
				break;
			}
			case 103:
			{
				stream.ReadUInt16();
				break;
			}
			case 106:
			case 118:
			{
				VarBit = stream.ReadUInt16();
				if (VarBit == 65535) {
					VarBit = -1;
				}
				
				Varp = stream.ReadUInt16();
				if (Varp == 65535) {
					Varp = -1;
				}
				
				int terminatingTransform = -1;
				if (opcode == 118) {
					terminatingTransform = stream.ReadUInt16();
					if (terminatingTransform == 65535) {
						terminatingTransform = -1;
					}
				}
				
				int count = stream.ReadByte();
				int[] trans = new int[count + 2];
				for (int i=0; i<count + 1; i++)
				{
					int transform = stream.ReadUInt16();
					if (transform == 65535) {
						transform = -1;
					}
					trans[i] = transform;
				}
				trans[count+1] = terminatingTransform;
				Transforms = new List<int>(trans);
				break;
			}
			case 107:
			{
				Interactable = false;
				break;
			}
			case 111:
			{
				Pet = true;
				break;
			}
			case 249:
			{
				Params.Merge<int, object>(ReadParams(buffer));
				break;
			}
		}
	}
	
}