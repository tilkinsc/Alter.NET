using Game.Model;

namespace Game.Sync.Block;

class UpdateBlockBuffer
{
	
	public bool Teleport = false;
	public int Mask = 0;
	public string ForceChat = "";
	public ChatMessage? PublicChat;
	public int FaceDegrees = 0;
	public int FacePawnIndex = -1;
	public int Animation = 0;
	public int AnimationDelay = 0;
	public int GraphicID = 0;
	public int GraphicHeight = 0;
	public int GraphicDelay = 0;
	public ForcedMovement? ForceMovement;
	
	public List<Hit> Hits = new List<Hit>();
	
	public bool IsDirty() => Mask != 0;
	
	public void Clean()
	{
		Mask = 0;
		Teleport = false;
		Hits.Clear();
	}
	
	public void AddBit(int bit)
	{
		Mask = Mask | bit;
	}
	
	public bool HasBit(int bit)
	{
		return (Mask & bit) > 0;
	}
	
}
