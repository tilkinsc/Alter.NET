using Game.Model;

namespace Game;

class GameContext
{
	public bool InitialLaunch;
	public string Name;
	public int Revision;
	public int CycleTime;
	public int PlayerLimit;
	public Tile Home;
	public int SkillCount;
	public int NPCStatCount;
	public bool RunEnergy;
	public int GroundItemPublicDelay;
	public int GroundItemDespawnDelay;
	public bool PreloadMaps;
	
	public GameContext(bool initialLaunch, string name, int revision, int cycleTime,
			int playerLimit, Tile home, int skillCount, int npcStatCount,
			bool runEnergy, int groundItemPublicDelay, int groundItemDespawnDelay, bool preloadMaps)
	{
		InitialLaunch = initialLaunch;
		Name = name;
		Revision = revision;
		CycleTime = cycleTime;
		PlayerLimit = playerLimit;
		Home = home;
		SkillCount = skillCount;
		NPCStatCount = npcStatCount;
		RunEnergy = runEnergy;
		GroundItemPublicDelay = groundItemPublicDelay;
		GroundItemDespawnDelay = groundItemDespawnDelay;
		PreloadMaps = preloadMaps;
	}
}