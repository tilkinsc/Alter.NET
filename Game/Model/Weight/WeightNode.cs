namespace Game.Model.Weight;

abstract class WeightNode<T>
{
	
	public int Weight { get; private set; }
	
	public WeightNode(int weight)
	{
		Weight = weight;
	}
	
	public abstract T Convert(Random random);
	
}