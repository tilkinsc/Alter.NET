using System.Diagnostics;
using Game.Model;
using JSON;
using Util;

namespace Game;

class Server
{
	
	public Server()
	{
		
	}
	
	public void StartServer()
	{
		Stopwatch sw = new Stopwatch();
		Logger.Log($"Loading API {GetAPIName()}");
		sw.Start();
		ApiJSON.Instance.Load();
		sw.Stop();
		Logger.Log($"API Loading took {sw.Elapsed}");
	}
	
	public void StartGame()
	{
		Stopwatch sw = new Stopwatch();
		Stopwatch sw2 = new Stopwatch();
		Logger.Log($"Loading Game");
		sw.Start();
		
		sw2.Start();
		GameJSON.Instance.Load();
		GameJSON gm = GameJSON.Instance.Handle;
		GameContext gameContext = new GameContext {
			InitialLaunch = gm.InitialLaunch,
			Name = gm.Name,
			Revision = gm.Revision,
			CycleTime = gm.CycleTime,
			PlayerLimit = gm.PlayerLimit,
			Home = new Tile(gm.Home),
			SkillCount = gm.SkillCount,
			NPCStatCount = gm.NPCStatCount,
			RunEnergy = gm.RunEnergy,
			GroundItemPublicDelay = gm.GroundItemPublicDelay,
			GroundItemDespawnDelay = gm.GroundItemDespawnDelay,
			PreloadMaps = gm.PreloadMaps
		};
		
		
		DevSettingsJSON.Instance.Load();
		DevSettingsJSON dv = DevSettingsJSON.Instance.Handle;
		DevContext devContext = new DevContext {
			DebugExamines = dv.DebugExamines,
			DebugObjects = dv.DebugObjects,
			DebugButtons = dv.DebugButtons,
			DebugItemActions = dv.DebugItemActions,
			DebugMagicSpells = dv.DebugMagicSpells
		};
		
		World world = new World(gameContext, devContext);
		sw2.Stop();
		Logger.Log($"Loaded world settings took {sw2.Elapsed}");
		
		// File Store
		
		
		
	}
	
	public string GetAPIName()
	{
		return ApiJSON.Instance.Handle.Org;
	}
	
}
