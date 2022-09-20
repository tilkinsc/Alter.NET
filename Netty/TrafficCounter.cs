using DotNetty.Common.Concurrency;
using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using static Netty.ObjectUtil;

namespace Netty;

class TrafficCounter
{
	
	private static IInternalLogger logger = InternalLoggerFactory.GetInstance<TrafficCounter>();
	
	public static long MillisecondFromNano()
	{
		return DateTime.UtcNow.Millisecond;
	}
	
	private readonly AtomicReference<Long> CurrentWrittenBytes = new AtomicReference<Long>();
	private readonly AtomicReference<Long> CurrentReadBytes = new AtomicReference<Long>();
	private long WritingTime;
	private long ReadingTime;
	
	private readonly AtomicReference<Long> CumulativeWrittenBytes = new AtomicReference<Long>();
	private readonly AtomicReference<Long> CumulativeReadBytes = new AtomicReference<Long>();
	
	private long LastCumulativeTime;
	private long LastWriteThroughput;
	private long LastReadThroughput;
	
	private readonly AtomicReference<Long> LastTime = new AtomicReference<Long>();
	
	private volatile Long LastWrittenBytes = new Long();
	private volatile Long LastReadBytes = new Long();
	private volatile Long LastWritingTime = new Long();
	private volatile Long LastReadingTime = new Long();
	
	private readonly AtomicReference<Long> RealWrittenBytes = new AtomicReference<Long>();
	private long RealWriteThroughput;
	
	private readonly AtomicReference<Long> CheckInterval = new AtomicReference<Long>(new Long(AbstractTrafficShapingHandler.DEFAULT_CHECK_INTERVAL));
	
	private string Name;
	
	private AbstractTrafficShapingHandler? TrafficShapingHandler;
	
	private IScheduledExecutorService Executor;
	
	private IRunnable? Monitor;
	
	private volatile IScheduledTask? ScheduledFuture;
	private volatile bool MonitorActive;
	
	private class TrafficMonitoringTask : IRunnable
	{
		private TrafficCounter Self;
		
		public TrafficMonitoringTask(TrafficCounter self)
		{
			Self = self;
		}
		
		public void Run()
		{
			if (!Self.MonitorActive)
				return;
			Self.ResetAccounting(MillisecondFromNano());
			if (Self.TrafficShapingHandler != null) {
				Self.TrafficShapingHandler.DoAccounting(Self);
			}
		}
	}
	
	// TODO: this function is synchronized
	public void Start()
	{
		if (!MonitorActive)
			return;
		LastTime.Value.Value = MillisecondFromNano();
		long localCheckInterval = CheckInterval.Value.Value;
		if (localCheckInterval > 0 && Executor != null) {
			MonitorActive = true;
			Monitor = new TrafficMonitoringTask(this);
			ScheduledFuture = Executor.Schedule(Monitor, TimeSpan.FromMilliseconds(localCheckInterval));
		}
	}
	
	// TODO: this function is synchronized
	public void Stop()
	{
		if (!MonitorActive)
			return;
		MonitorActive = false;
		ResetAccounting(MillisecondFromNano());
		if (TrafficShapingHandler != null) {
			TrafficShapingHandler.DoAccounting(this);
		}
		if (ScheduledFuture != null) {
			ScheduledFuture.Cancel();
		}
	}
	
	// TODO: this function is synchronized
	public void ResetAccounting(long newLastTime)
	{
		long interval = newLastTime - LastTime.Value.Value;
		if (interval == 0)
			return;
		
		if (logger.DebugEnabled && interval > (CheckInterval.Value.Value << 1)) {
			logger.Debug($"Acct schedule not ok: {interval} > 2*{CheckInterval.Value.Value} from {Name}");
		}
		LastReadBytes = CurrentReadBytes.Value;
		CurrentReadBytes.Value.Value = 0;
		LastWrittenBytes = CurrentWrittenBytes.Value;
		CurrentWrittenBytes.Value.Value = 0;
		LastReadThroughput = LastReadBytes.Value * 1000 / interval;
		LastWriteThroughput = LastWrittenBytes.Value * 1000 / interval;
		RealWriteThroughput = RealWrittenBytes.Value.Value * 1000 / interval;
		RealWrittenBytes.Value.Value = 0;
		LastWritingTime.Value = Math.Max(LastWritingTime.Value, WritingTime);
		LastReadingTime.Value = Math.Max(LastReadingTime.Value, ReadingTime);
	}
	
	public TrafficCounter(IScheduledExecutorService executor, string name, long checkInterval)
	{
		Name = CheckNotNull(name, "name");
		TrafficShapingHandler = null;
		Executor = executor;
		
		Init(checkInterval);
	}
	
	public TrafficCounter(AbstractTrafficShapingHandler trafficShapingHandler, IScheduledExecutorService executor, string name, long checkInterval)
	{
		Name = CheckNotNull(name, "name");
		TrafficShapingHandler = trafficShapingHandler;
		Executor = executor;
		
		Init(checkInterval);
	}
	
	private void Init(long checkInterval)
	{
		LastCumulativeTime = DateTime.UtcNow.Millisecond;
		WritingTime = MillisecondFromNano();
		ReadingTime = WritingTime;
		LastWritingTime.Value = WritingTime;
		LastReadingTime.Value = WritingTime;
		Configure(checkInterval);
	}
	
	public void Configure(long newCheckInterval)
	{
		long newInterval = newCheckInterval / 10 * 10;
		if (CheckInterval.Value.Value != newInterval) {
			CheckInterval.Value.Value = newInterval;
			if (newInterval <= 0) {
				Stop();
				LastTime.Value.Value = MillisecondFromNano();
			} else {
				Stop();
				Start();
			}
		}
	}
	
	public void BytesRecvFlowControl(long recv)
	{
		CurrentReadBytes.Value.Value += recv;
		CumulativeReadBytes.Value.Value += recv;
	}
	
	public void BytesWriteFlowControl(long write)
	{
		CurrentWrittenBytes.Value.Value += write;
		CumulativeWrittenBytes.Value.Value += write;
	}
	
	public void BytesRealWriteFlowControl(long write)
	{
		RealWrittenBytes.Value.Value += write;
	}
	
	public long GetCheckInterval()
	{
		return CheckInterval.Value.Value;
	}
	
	public long GetLastReadThroughput()
	{
		return LastReadThroughput;
	}
	
	public long GetLastWriteThroughput()
	{
		return LastWriteThroughput;
	}
	
	public long GetLastReadBytes()
	{
		return LastReadBytes.Value;
	}
	
	public long GetLastWrittenBytes()
	{
		return LastWrittenBytes.Value;
	}
	
	public long GetCurrentReadBytes()
	{
		return CurrentReadBytes.Value.Value;
	}
	
	public long GetCurrentWrittenBytes()
	{
		return CurrentWrittenBytes.Value.Value;
	}
	
	public long GetLastTime()
	{
		return LastTime.Value.Value;
	}
	
	public long GetCumulativeReadBytes()
	{
		return CumulativeReadBytes.Value.Value;
	}
	
	public long GetCumulativeWrittenBytes()
	{
		return CumulativeWrittenBytes.Value.Value;
	}
	
	public long GetLastCumulativeTime()
	{
		return LastCumulativeTime;
	}
	
	public AtomicReference<Long> GetRealWrittenBytes()
	{
		return RealWrittenBytes;
	}
	
	public long GetRealWriteThroughput()
	{
		return RealWriteThroughput;
	}
	
	public void ResetCumulativeTime()
	{
		LastCumulativeTime = DateTime.UtcNow.Millisecond;
		CumulativeReadBytes.Value.Value = 0;
		CumulativeWrittenBytes.Value.Value = 0;
	}
	
	public string GetName()
	{
		return Name;
	}
	
	[Obsolete]
	public long ReadTimeToWait(long size, long limitTraffic, long maxTime)
	{
		return ReadTimeToWait(size, limitTraffic, maxTime, MillisecondFromNano());
	}
	
	public long ReadTimeToWait(long size, long limitTraffic, long maxTime, long now)
	{
		BytesRecvFlowControl(size);
		if (size == 0 || limitTraffic == 0) {
			return 0;
		}
		long lastTimeCheck = LastTime.Value.Value;
		long sum = CurrentReadBytes.Value.Value;
		long localReadingTime = ReadingTime;
		long lastRB = LastReadBytes.Value;
		long interval = now - lastTimeCheck;
		long pastDelay = Math.Max(LastReadingTime.Value - lastTimeCheck, 0);
		if (interval > AbstractTrafficShapingHandler.MINIMAL_WAIT) {
			long time0 = sum * 1000 / limitTraffic - interval + pastDelay;
			if (time0 > AbstractTrafficShapingHandler.MINIMAL_WAIT) {
				if (logger.DebugEnabled) {
					logger.Debug($"Time: {time0} : {sum} : {interval} : {pastDelay}");
				}
				if (time0 > maxTime && now + time0 - localReadingTime > maxTime) {
					time0 = maxTime;
				}
				ReadingTime = Math.Max(localReadingTime, now + time0);
				return time0;
			}
			ReadingTime = Math.Max(localReadingTime, now);
			return 0;
		}
		long lastSum = sum + lastRB;
		long lastInterval = interval + CheckInterval.Value.Value;
		long time1 = lastSum * 1000 / limitTraffic - lastInterval + pastDelay;
		if (time1 > AbstractTrafficShapingHandler.MINIMAL_WAIT) {
			if (logger.DebugEnabled) {
				logger.Debug($"Time: {time1} : {lastSum} : {lastInterval} : {pastDelay}");
			}
			if (time1 > maxTime && now + time1 - localReadingTime > maxTime) {
				time1 = maxTime;
			}
			ReadingTime = Math.Max(localReadingTime, now + time1);
			return time1;
		}
		ReadingTime = Math.Max(localReadingTime, now);
		return 0;
	}
	
	[Obsolete]
	public long WriteTimeToWait(long size, long limitTraffic, long maxTime)
	{
		return WriteTimeToWait(size, limitTraffic, maxTime, MillisecondFromNano());
	}
	
	public long WriteTimeToWait(long size, long limitTraffic, long maxTime, long now)
	{
		BytesWriteFlowControl(size);
		if (size == 0 || limitTraffic == 0) {
			return 0;
		}
		long lastTimeCheck = LastTime.Value.Value;
		long sum = CurrentWrittenBytes.Value.Value;
		long lastWB = LastWrittenBytes.Value;
		long localWritingTime = WritingTime;
		long pastDelay = Math.Max(LastWritingTime.Value - lastTimeCheck, 0);
		long interval = now - lastTimeCheck;
		if (interval > AbstractTrafficShapingHandler.MINIMAL_WAIT) {
			long time0 = sum * 1000 / limitTraffic - interval + pastDelay;
			if (time0 > AbstractTrafficShapingHandler.MINIMAL_WAIT) {
				if (logger.DebugEnabled) {
					logger.Debug($"Time: {time0} : {sum} : {interval} : {pastDelay}");
				}
				if (time0 > maxTime && now + time0 - localWritingTime > maxTime) {
					time0 = maxTime;
				}
				WritingTime = Math.Max(localWritingTime, now + time0);
				return time0;
			}
			WritingTime = Math.Max(localWritingTime, now);
			return 0;
		}
		long lastSum = sum + lastWB;
		long lastInterval = interval + CheckInterval.Value.Value;
		long time1 = lastSum * 1000 / limitTraffic - lastInterval + pastDelay;
		if (time1 > AbstractTrafficShapingHandler.MINIMAL_WAIT) {
			if (logger.DebugEnabled) {
				logger.Debug($"Time: {time1} : {lastSum} : {lastInterval} : {pastDelay}");
			}
			if (time1 > maxTime && now + time1 - localWritingTime > maxTime) {
				time1 = maxTime;
			}
			WritingTime = Math.Max(localWritingTime, now + time1);
			return time1;
		}
		WritingTime = Math.Max(localWritingTime, now);
		return 0;
	}
	
	public override string ToString()
	{
		return $"Monitor {Name} Current Speed Read: {LastReadThroughput >> 10} KB/s, Asked Write: {LastWriteThroughput >> 10} KB/s, Real Write: {RealWriteThroughput >> 10} KB/s, Current Read: {CurrentReadBytes.Value.Value >> 10} KB, Current Asked Write: {CurrentWrittenBytes.Value.Value >> 10} KB, Current Real Write: {RealWrittenBytes.Value.Value >> 10} KB";
	}
	
}