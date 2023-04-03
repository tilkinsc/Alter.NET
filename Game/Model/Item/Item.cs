using Game.FS;
using Game.FS.Def;
using Util;

namespace Game.Model.Item;

class Item
{
	
	public int ID { get; private set; }
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
		CopyAttributes(other);
	}
	
	public Item(Item other, int amount)
			: this(other.ID, other.Amount)
	{
		CopyAttributes(other);
	}
	
	private ItemDef? GetDef(DefinitionSet defs)
	{
		return defs.Get<ItemDef>(ID);
	}
	
	public Item ToNoted(DefinitionSet defs)
	{
		ItemDef? def = GetDef(defs);
		return (def.NoteTemplateId == 0 && def.NoteLinkId > 0) ?
				new Item(def.NoteLinkId, Amount).CopyAttributes(this)
				: new Item(this).CopyAttributes(this);
	}
	
	public Item ToUnnoted(DefinitionSet defs)
	{
		ItemDef? def = GetDef(defs);
		return (def.NoteTemplateId > 0) ?
				new Item(def.NoteLinkId, Amount).CopyAttributes(this)
				: new Item(this).CopyAttributes(this);
	}
	
	public string GetName(DefinitionSet defs)
	{
		return ToUnnoted(defs).GetDef(defs).Name;
	}
	
	public bool HasAnyAttribute() => Attributes.Count > 0;
	
	public int? GetAttribute(ItemAttribute attrib)
	{
		if (!Attributes.ContainsKey(attrib))
			return null;
		return Attributes[attrib];
	}
	
	public Item PutAttribute(ItemAttribute attrib, int value)
	{
		Attributes[attrib] = value;
		return this;
	}
	
	public Item CopyAttributes(Item other)
	{
		if (other.HasAnyAttribute())
			Attributes.Merge(other.Attributes);
		return this;
	}
	
}