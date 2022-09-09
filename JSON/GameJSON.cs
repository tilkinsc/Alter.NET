namespace JSON;

using System.Text.Json.Serialization;
using Game.Model;

class GameJSON
{
	
	private static GameJSON instance = new GameJSON();
	private static JSONSerializable<GameJSON> Data;
	public static JSONSerializable<GameJSON> Instance { get => Data; }
	public static string Path { get; set; } = "./Data/game.json";
	
	[JsonInclude]
	public bool InitialLaunch;
	[JsonInclude]
	public string Name;
	[JsonInclude]
	public int Revision;
	[JsonInclude]
	public int CycleTime;
	[JsonInclude]
	public int PlayerLimit;
	[JsonInclude]
	public int Home;
	[JsonInclude]
	public int SkillCount;
	[JsonInclude]
	public int NPCStatCount;
	[JsonInclude]
	public bool RunEnergy;
	[JsonInclude]
	public int GroundItemPublicDelay;
	[JsonInclude]
	public int GroundItemDespawnDelay;
	[JsonInclude]
	public bool PreloadMaps;
	
	static GameJSON() {
		Data = new JSONSerializable<GameJSON>(instance, Path);
	}
	
	public GameJSON()
	{
		InitialLaunch = true;
		Name = "";
		Revision = 0;
		CycleTime = 600;
		PlayerLimit = 2048;
		Home = 0;
		SkillCount = 0; // TODO: Skillset.DEFAULT_SKILL_COUNT
		NPCStatCount = 0; // TODO: Npc.Stats.DEFAULT_NPC_STAT_COUNT
		RunEnergy = true;
		GroundItemPublicDelay = 0; // TODO: GroundItem.DEFAULT_PUBLIC_SPAWN_CYCLES
		GroundItemDespawnDelay = 0; // TODO: GroundItem.DEFAULT_DESPAWN_CYCLES
	}
	
}
