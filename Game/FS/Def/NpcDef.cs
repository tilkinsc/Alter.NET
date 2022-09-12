using DotNetty.Buffers;
using Util;
using Util.IO;

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
	
	public override void Decode(IByteBuffer buf, int opcode)
	{
		switch (opcode)
		{
			case 1:
			{
				int count = buf.ReadByte();
				for (int i=0; i<count; i++)
					buf.ReadUnsignedShort();
				break;
			}
			case 2:
			{
				Name = buf.ReadString();
				break;
			}
			case 12:
			{
				Size = buf.ReadByte();
				break;
			}
			case 13:
			{
				StandAnim = buf.ReadUnsignedShort();
				break;
			}
			case 14:
			{
				WalkAnim = buf.ReadUnsignedShort();
				break;
			}
			case 15:
			{
				RotateLeftAnim = buf.ReadUnsignedShort();
				break;
			}
			case 16:
			{
				RotateRightAnim = buf.ReadUnsignedShort();
				break;
			}
			case 17:
			{
				WalkAnim = buf.ReadUnsignedShort();
				Rotate180Anim = buf.ReadUnsignedShort();
				Rotate90AnimCW = buf.ReadUnsignedShort();
				Rotate90AnimCCW = buf.ReadUnsignedShort();
				break;
			}
			case 18:
			{
				Category = buf.ReadUnsignedShort();
				break;
			}
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			{
				string read = buf.ReadString();
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
				int count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadUnsignedShort();
					buf.ReadUnsignedShort();
				}
				break;
			}
			case 60:
			{
				int count = buf.ReadByte();
				ChatHeadModels = new List<int>(count);
				for (int i=0; i<count; i++)
				{
					ChatHeadModels.Add(buf.ReadUnsignedShort());
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
				CombatLevel = buf.ReadUnsignedShort();
				break;
			}
			case 97:
			{
				WidthScale = buf.ReadUnsignedShort();
				break;
			}
			case 98:
			{
				heightScale = buf.ReadUnsignedShort();
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
				buf.ReadByte();
				break;
			}
			case 102:
			{
				HeadIcon = buf.ReadUnsignedShort();
				break;
			}
			case 103:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 106:
			case 118:
			{
				VarBit = buf.ReadUnsignedShort();
				if (VarBit == 65535) {
					VarBit = -1;
				}
				
				Varp = buf.ReadUnsignedShort();
				if (Varp == 65535) {
					Varp = -1;
				}
				
				int terminatingTransform = -1;
				if (opcode == 118) {
					terminatingTransform = buf.ReadUnsignedShort();
					if (terminatingTransform == 65535) {
						terminatingTransform = -1;
					}
				}
				
				int count = buf.ReadByte();
				int[] trans = new int[count + 2];
				for (int i=0; i<count + 1; i++)
				{
					int transform = buf.ReadUnsignedShort();
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
				Params.Merge<int, object>(ReadParams(buf));
				break;
			}
		}
	}
	
}