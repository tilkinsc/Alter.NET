using DotNetty.Buffers;
using Util;
using Util.IO;

namespace Game.FS.Def;

class ItemDef : Definition
{
	
	public string Name = "";
	public bool Stacks = false;
	public int Cost = 0;
	public bool Members = false;
	public List<string?> GroundMenu = new List<string?>(new string?[] { null, null, null, null, null });
	public List<string?> InventoryMenu = new List<string?>(new string?[] { null, null, null, null, null });
	public List<string?> EquipmentMenu = new List<string?>(new string?[] { null, null, null, null, null, null, null, null });
	public bool GrandExchangable = false;
	public int TeamCape = 0;
	public int NoteLinkId = 0;
	public int NoteTemplateId = 0;
	public int PlaceholderLink = 0;
	public int PlaceholderTemplate = 0;
	public Dictionary<int, object> Params = new Dictionary<int, object>();
	public int Category = -1;
	public int Zoom2D = 0;
	public string? Examine;
	public bool Tradable = false;
	public double Weight = 0.0;
	public int AttackSpeed = -1;
	public int EquipSlot = -1;
	public int EquipType = 0;
	public int WeaponType = -1;
	public int[]? RenderAnimations;
	public Dictionary<byte, byte>? SkillRequirements;
	public int[]? Bonuses;

	public bool IsStackable { get => Stacks || NoteTemplateId > 0; }
	public bool IsNoted { get => NoteTemplateId > 0; }
	public bool IsPlaceholder { get => PlaceholderTemplate > 0 && PlaceholderLink > 0; }
	
	public ItemDef(int id)
			: base(id)
	{
	}
	
	public override void Decode(IByteBuffer buf, int opcode)
	{
		switch (opcode)
		{
			case 1:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 2:
			{
				Name = buf.ReadString();
				break;
			}
			case 4:
			{
				Zoom2D = buf.ReadUnsignedShort();
				break;
			}
			case 5:
			case 6:
			case 7:
			case 8:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 9:
			{
				buf.ReadString();
				break;
			}
			case 11:
			{
				Stacks = true;
				break;
			}
			case 12:
			{
				Cost = buf.ReadInt();
				break;
			}
			case 16:
			{
				Members = true;
				break;
			}
			case 23:
			{
				buf.ReadUnsignedShort();
				buf.ReadByte();
				break;
			}
			case 24:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 25:
			{
				buf.ReadUnsignedShort();
				buf.ReadByte();
				break;
			}
			case 26:
			{
				buf.ReadUnsignedShort();
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
					GroundMenu[opcode - 30] = null;
					break;
				}
				GroundMenu[opcode - 30] = read;
				break;
			}
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			{
				InventoryMenu[opcode - 35] = buf.ReadString();
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
			case 42:
			{
				buf.ReadByte();
				break;
			}
			case 65:
			{
				GrandExchangable = true;
				break;
			}
			case 78:
			case 79:
			case 90:
			case 91:
			case 92:
			case 93:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 94:
			{
				Category = buf.ReadUnsignedShort();
				break;
			}
			case 95:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 97:
			{
				NoteLinkId = buf.ReadUnsignedShort();
				break;
			}
			case 98:
			{
				NoteTemplateId = buf.ReadUnsignedShort();
				break;
			}
			case 100:
			case 101:
			case 102:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
			case 108:
			case 109:
			{
				buf.ReadUnsignedShort();
				buf.ReadUnsignedShort();
				break;
			}
			case 110:
			case 111:
			case 112:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 113:
			case 114:
			{
				buf.ReadByte();
				break;
			}
			case 115:
			{
				TeamCape = buf.ReadByte();
				break;
			}
			case 139:
			case 140:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 148:
			{
				PlaceholderLink = buf.ReadUnsignedShort();
				break;
			}
			case 149:
			{
				PlaceholderTemplate = buf.ReadUnsignedShort();
				break;
			}
			case 249:
			{
				Params.Merge<int, object>(ReadParams(buf));
				
				for (int i=0; i<8; i++)
				{
					int paramId = 451 + i;
					string? option = null;
					if (Params.ContainsKey(paramId)) {
						option = Params[paramId] as string;
						if (option == null)
							continue;
					}
					EquipmentMenu[i] = option;
				}
				break;
			}
		}
	}
	
}