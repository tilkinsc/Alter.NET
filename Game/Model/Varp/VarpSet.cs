using Util;

namespace Game.Model.Varp;

class VarpSet
{
	
	public int MaxVarps { get; private set; }
	
	private List<Varp> _varps = new List<Varp>();
	private HashSet<short> _dirty = new HashSet<short>();
	
	public VarpSet(int maxVarps)
	{
		MaxVarps = maxVarps;
		for (int i=0; i<maxVarps; i++)
			_varps[i] = new Varp(i, 0);
	}
	
	public Varp this[int id] => _varps[id];
	
	public int GetState(int id) => _varps[id].State;
	
	public VarpSet SetState(int id, int state)
	{
		_varps[id].State = state;
		_dirty.Add((short) id);
		return this;
	}
	
	public int GetBit(int id, int startBit, int endBit)
	{
		return BitManipulation.GetBit(GetState(id), startBit, endBit);
	}
	
	public VarpSet SetBit(int id, int startBit, int endBit, int value)
	{
		return SetState(id, BitManipulation.SetBit(GetState(id), startBit, endBit, value));
	}
	
	public bool IsDirty(int id) => _dirty.Contains((short) id);
	
	public void clean() => _dirty.Clear();
	
	public List<Varp> GetAll() => _varps;
	
}