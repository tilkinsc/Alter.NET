namespace Game.Model.Varp;

class Varp
{
	public int ID { get; private set; }
	public int State;

	public Varp(int id, int state)
	{
		ID = id;
		State = state;
	}
}