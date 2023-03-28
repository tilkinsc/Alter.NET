using Game.FS;
using Game.FS.Def;
using Util;

namespace Game.Model.Item;

class Item
{
	
	public int ID;
	public int Amount;
	
	public Dictionary<ItemAttribute, int> Attributes = new Dictionary<ItemAttribute, int>();
	
	public Item(int id, int amount = 1)
	{
		ID = id;
		Amount = amount;
	}
	
	public Item(Item other)
			: this(other.ID, other.Amount)
	{
		CopyAttr(other);
	}
	
	public Item(Item other, int amount)
			: this(other.ID, other.Amount)
	{
		CopyAttr(other);
	}
	
	private ItemDef? GetDef(DefinitionSet defs)
	{
		return defs.Get<ItemDef>(ID);
	}
	
	public Item ToNoted(DefinitionSet defs)
	{
		ItemDef? def = GetDef(defs);
		return (def.NoteTemplateId == 0 && def.NoteLinkId > 0) ?
				new Item(def.NoteLinkId, Amount).CopyAttr(this)
				: new Item(this).CopyAttr(this);
	}
	
	public Item ToUnnoted(DefinitionSet defs)
	{
		ItemDef? def = GetDef(defs);
		return (def.NoteTemplateId > 0) ?
				new Item(def.NoteLinkId, Amount).CopyAttr(this)
				: new Item(this).CopyAttr(this);
	}
	
	public string GetName(DefinitionSet defs)
	{
		return ToUnnoted(defs).GetDef(defs).Name;
	}
	
	public bool HasAttributes() => Attributes.Count > 0;
	
	public int? GetAttribute(ItemAttribute attrib) => Attributes[attrib];
	
	public Item PutAttribute(ItemAttribute attrib, int value)
	{
		Attributes[attrib] = value;
		return this;
	}
	
	public Item CopyAttributes(Item other)
	{
		if (other.HasAttributes()) {
			Attributes.Merge(other.Attributes);
		}
		return this;
	}
	
}