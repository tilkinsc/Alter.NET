using Cache;
using Game.Model;

namespace Game.Service.Xtea;

class XTeaKeyService : IService
{
	
	public static readonly int[] EMPTY_KEYS = new int[] { 0, 0, 0, 0 };
	
	public Dictionary<int, int[]> Keys = new Dictionary<int, int[]>();
	
	public int[] ValidRegions { get => Keys.Keys.ToArray(); }
	
	private void LoadKeys(World world)
	{
		int maxRegions = short.MaxValue;
		int totalRegions = 0;
		// List<int> missingKeys = new List<int>();

		Cache.FS.RLIndex regionIndex = world.FileStore.GetIndex(RLIndexType.MAPS)!;
		for (int regionId=0; regionId<maxRegions; regionId++)
		{
			int x = regionId >> 8;
			int z = regionId & 0xFF;
			
			if (regionIndex.FindArchiveByName($"m{x}_{z}") == null)
				continue;
			if (regionIndex.FindArchiveByName($"l{x}_{z}") == null)
				continue;
			
			totalRegions++;
			
			// if (Get(regionId).ValuesEqual(EMPTY_KEYS)) {
			// 	missingKeys.Add(regionId);
			// }
		}
		
		world.XTeaKeyService = this;
		
		// int validKeys = totalRegions - missingKeys.Count;
		
	}
	
	private void LoadSingleFile(string path)
	{
		// not enough information of where we are reading xteas from
	}
	
	private void LoadDirectory(string path)
	{
		// not enough information of where wea re reading directories from
	}
	
	public void Init(Server server, World world)
	{
		
	}
	
	public void PostLoad(Server server, World world)
	{
	}
	
	public void BindNet(Server server, World world)
	{
	}
	
	public void Terminate(Server server, World world)
	{
	}
	
	public int[] Get(int region)
	{
		if (!Keys.ContainsKey(region)) {
			Keys[region] = EMPTY_KEYS;
		}
		return Keys[region];
	}
	
	private class XTeaFile : IEquatable<XTeaFile>
	{
		
		public int MapSquare;
		public int[] Key;
		
		public XTeaFile(int mapSquare, int[] key)
		{
			MapSquare = mapSquare;
			Key = key;
		}
		
		public override bool Equals(object? obj)
		{
			return Equals(obj as XTeaFile);
		}
		
		public bool Equals(XTeaFile? obj)
		{
			if (obj == null)
				return false;
			if (this == obj)
				return true;
			
			if (MapSquare != obj.MapSquare)
				return false;
			if (!Key.ValuesEqual(obj.Key))
				return false;
			
			return true;
		}
		
		public override int GetHashCode()
		{
			int hash = MapSquare;
			hash = 31 * hash + Key.GetHashCode();
			return hash;
		}
		
	}
	
}