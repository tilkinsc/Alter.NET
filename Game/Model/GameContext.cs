using Game.Model;

namespace Game;

class GameContext
{
	public bool InitialLaunch { get; set; }
	public string Name { get; set; } = "";
	public int Revision { get; set; }
	public int CycleTime { get; set; }
	public int PlayerLimit { get; set; }
	public Tile Home { get; set; } = new Tile(0, 0);
	public int SkillCount { get; set; }
	public int NPCStatCount { get; set; }
	public bool RunEnergy { get; set; }
	public int GroundItemPublicDelay { get; set; }
	public int GroundItemDespawnDelay { get; set; }
	public bool PreloadMaps { get; set; }
}