using System.Collections;
using Exceptions;

namespace Game.Model.Priv;

class PrivilegeSet : IEnumerable<Privilege>
{
	
	public List<Privilege> Values { get; private set; }
	
	public PrivilegeSet()
	{
	}
	
	// TODO: public void load(ServerProperties properties) {}
	
	public Privilege? Get(int id) => Values.FirstOrDefault(priv => priv?.ID == id, null);
	public Privilege? Get(string name) => Values.FirstOrDefault(priv => priv?.Name == name.ToLower(), null);
	
	public bool IsEligible(Privilege from, string to) => from.Powers.Contains(to.ToLower());
	
	public IEnumerator<Privilege> GetEnumerator()
	{
		return Values?.GetEnumerator() ?? throw new IllegalStateException("PrivilegeSet iterated before loaded!");
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	
}