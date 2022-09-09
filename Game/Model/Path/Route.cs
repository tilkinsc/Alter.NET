namespace Game.Model.Path;

class Route
{
	public Queue<Tile> Path;
	public bool Success;
	public Tile Tail;
	
	public Route(Queue<Tile> path, bool success, Tile tail)
	{
		Path = path;
		Success = success;
		Tail = tail;
	}
}