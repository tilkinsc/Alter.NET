using Exceptions;

namespace Game.Model.Weight;

class WeightNodeSet<T>
{
	
	private List<WeightNode<T>> _nodes = new List<WeightNode<T>>();
	
	// TODO: Secure random
	private Random _random = new Random();
	
	public virtual WeightNodeSet<T> Add(WeightNode<T> node)
	{
		_nodes.Add(node);
		return this;
	}
	
	public WeightNode<T> GetRandomNode(Random random = null!)
	{
		if (random == null)
			random = _random;
		
		int totalWeight = 0;
		for (int i=0; i<_nodes.Count; i++)
			totalWeight += _nodes[i].Weight;
		
		int randomWeight = random.Next(totalWeight + 1);
		foreach (WeightNode<T> node in _nodes)
		{
			randomWeight -= node.Weight;
			if (randomWeight <= 0)
				return node;
		}
		
		throw new RuntimeException();
	}
	
	public T GetRandom(Random random = null!)
	{
		if (random == null)
			random = _random;
		return GetRandomNode(random).Convert(random);
	}
	
}