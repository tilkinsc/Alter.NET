namespace Game.Model.Weight.Impl;

class WeightItemSet : WeightNodeSet<Item.Item>
{
	
	public override WeightItemSet Add(WeightNode<Item.Item> node)
	{
		base.Add(node);
		return this;
	}
	
}