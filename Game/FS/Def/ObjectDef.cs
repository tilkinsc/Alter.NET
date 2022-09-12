using System.Text;
using Util;
using Game.Model.Entity;
using DotNetty.Buffers;
using Util.IO;

namespace Game.FS.Def;

class ObjectDef : Definition
{
	
	public String Name = "";
	public int Width = 1;
	public int Length = 1;
	public bool IsSolid = true;
	public bool IsImpenetrable = true;
	public bool IsInteractive = false;
	public bool IsObstructive = false;
	public int ClipMask = 0;
	public int VarBit = -1;
	public int Varp = -1;
	public int Animation = -1;
	public bool IsRotated = false;
	public int Category = -1;
	public List<string?> Options = new List<string?>(new string[5] { "", "", "", "", "" });
	public List<int>? Transforms;
	public Dictionary<int, object> Parameters = new Dictionary<int, object>();
	public string? Examine;
	
	public ObjectDef(int id)
		: base(id)
	{
	}
	
	public int GetRotatedWidth(GameObject obj)
	{
		if ((obj.Rotation & 0x1) == 1) {
			return Length;
		}
		return Width;
	}
	
	public int GetRotatedLength(GameObject obj)
	{
		if ((obj.Rotation & 0x1) == 1) {
			return Width;
		}
		return Length;
	}
	
	public override void Decode(IByteBuffer buf, int opcode)
	{
		switch (opcode)
		{
			case 1:
			{
				byte count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadUnsignedShort();
					buf.ReadByte();
				}
				break;
			}
			case 2:
			{
				Name = buf.ReadString();
				break;
			}
			case 5:
			{
				byte count = buf.ReadByte();
				for (int i=0; i<count; i++)
					buf.ReadUnsignedShort();
				break;
			}
			case 14:
			{
				Width = buf.ReadByte();
				break;
			}
			case 15:
			{
				Length = buf.ReadByte();
				break;
			}
			case 17:
			{
				IsSolid = false;
				break;
			}
			case 18:
			{
				IsImpenetrable = false;
				break;
			}
			case 19:
			{
				IsInteractive = buf.ReadByte() == 1;
				break;
			}
			case 24:
			{
				Animation = buf.ReadUnsignedShort();
				break;
			}
			case 27:
			{
				break;
			}
			case 28:
			{
				buf.ReadByte();
				break;
			}
			case 29:
			{
				buf.ReadByte();
				break;
			}
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			{
				string str = buf.ReadString();
				Options[opcode - 30] = str;
				if (str.Equals("null") || str.Equals("hidden")) {
					Options[opcode - 30] = null;
				}
				break;
			}
			case 39:
			{
				buf.ReadByte();
				break;
			}
			case 40:
			{
				byte count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadUnsignedShort();
					buf.ReadUnsignedShort();
				}
				break;
			}
			case 41:
			{
				byte count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadUnsignedShort();
					buf.ReadUnsignedShort();
				}
				break;
			}
			case 61:
			{
				Category = buf.ReadUnsignedShort();
				break;
			}
			case 62:
			{
				IsRotated = true;
				break;
			}
			case 65:
			case 66:
			case 67:
			case 68:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 69:
			{
				ClipMask = buf.ReadByte();
				break;
			}
			case 70:
			case 71:
			case 72:
			{
				buf.ReadShort();
				break;
			}
			case 73:
			{
				IsObstructive = true;
				break;
			}
			case 75:
			{
				buf.ReadByte();
				break;
			}
			case 77:
			case 92:
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
				if (opcode == 92) {
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
					if (transform == 65535)
						transform = -1;
					trans[i] = transform;
				}
				trans[count+1] = terminatingTransform;
				Transforms = new List<int>(trans);
				break;
			}
			case 78:
			{
				buf.ReadUnsignedShort();
				buf.ReadByte();
				break;
			}
			case 79:
			{
				buf.ReadUnsignedShort();
				buf.ReadUnsignedShort();
				buf.ReadByte();
				int count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadUnsignedShort();
				}
				break;
			}
			case 81:
			{
				buf.ReadByte();
				break;
			}
			case 82:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 249:
			{
					Parameters.Merge(ReadParams(buf));
				break;
			}
		}
	}
}
