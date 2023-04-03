using System.Collections;

namespace Game.Model.Timer;

class TimerMap
{
	
	public class PersistentTimer
	{
		public int TimeLeft;
		public long CurrentMS;
		public string? Identifier;
		public bool TickOffline;
		
		public PersistentTimer(int timeLeft, long currentMS, string? identifier = null, bool tickOffline = true)
		{
			TimeLeft = timeLeft;
			CurrentMS = currentMS;
			Identifier = identifier;
			TickOffline = tickOffline;
		}
	}
	
	public Dictionary<TimerKey, int> Timers = new Dictionary<TimerKey, int>();

	public int Get(TimerKey key)
	{
		Timers.TryGetValue(key, out int val);
		return val;
	}
	
	public int GetOrDefault(TimerKey key, int def)
	{
		if (Timers.TryGetValue(key, out int val))
			return val;
		return def;
	}
	
	public void Set(TimerKey key, int val) => Timers[key] = val;
	public void Remove(TimerKey key) => Timers.Remove(key);
	public bool Has(TimerKey key) => GetOrDefault(key, 0) > 0;
	public bool Exists(TimerKey key) => Timers.ContainsKey(key);
	public bool IsEmpty() => !Timers.Any();
	public void Clear() => Timers.Clear();
	
	public List<PersistentTimer> ToPersistentTimers()
	{
		List<PersistentTimer> timers = new List<PersistentTimer>();
		long time = Util.Time.CurrentTimeMillis();
		foreach (KeyValuePair<TimerKey, int> timer in Timers)
		{
			if (timer.Key.PersistenceKey == null)
				continue;
			timers.Add(new PersistentTimer(timer.Value, time, timer.Key.PersistenceKey, timer.Key.TickOffline));
		}
		return timers;
	}
	
}
