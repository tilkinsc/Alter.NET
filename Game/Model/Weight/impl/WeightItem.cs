namespace Game.Model.Weight.Impl;

class WeightItem : WeightNode<Item.Item>
{
	
	public int Item { get; private set; }
	
	private int _amount;
	private int _maxAmount;
	
	public WeightItem(int item, int weight, int maxAmount = -1, int amount = 1)
			: base(weight)
	{
		Item = item;
		_amount = amount;
		if (maxAmount == -1)
			maxAmount = amount;
		_maxAmount = maxAmount;
	}
	
	// TODO: I dont know how well Range maps up to Kotlins IntRange
	public WeightItem(int item, Range amount, int weight)
			: this(item, weight, amount.Start.Value, amount.End.Value)
	{
	}
	
	public override Item.Item Convert(Random random)
	{
		return new Item.Item(Item, _amount + random.Next((_maxAmount - _amount) + 1));
	}
	
}