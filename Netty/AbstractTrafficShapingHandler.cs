using DotNetty.Buffers;
using DotNetty.Common.Concurrency;
using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace Netty;

abstract class AbstractTrafficShapingHandler : ChannelDuplexHandler
{
	
	private static IInternalLogger logger = InternalLoggerFactory.GetInstance<AbstractTrafficShapingHandler>();
	
	public static readonly AttributeKey<Bool?> READ_SUSPENDED = AttributeKey<Bool?>.ValueOf(typeof(AbstractTrafficShapingHandler).Name + ".READ_SUSPENDED");
	public static readonly AttributeKey<IRunnable?> REOPEN_TASK = AttributeKey<IRunnable?>.ValueOf(typeof(AbstractTrafficShapingHandler).Name + ".REOPEN_TASK");
	
	public const long CHANNEL_DEFAULT_USER_DEFINED_WRITABILITY_INDEX = 1;
	public const long GLOBAL_DEFAULT_USER_DEFINED_WRITABILITY_INDEX = 2;
	public const long GLOBALCHANNEL_DEFAULT_USER_DEFINED_WRITABILITY_INDEX = 3;
	
	public const long DEFAULT_CHECK_INTERVAL = 1000;
	public const long DEFAULT_MAX_TIME = 15000;
	public const long DEFAULT_MAX_SIZE = 4 * 1024 * 1024;
	public const long MINIMAL_WAIT = 10;
	
	private volatile Long WriteLimit = new Long();
	private volatile Long ReadLimit = new Long();
	private protected volatile Long MaxTime = new Long(DEFAULT_MAX_TIME);
	private protected volatile Long CheckInterval = new Long(DEFAULT_CHECK_INTERVAL);
	private volatile Long MaxWriteDelay = new Long(4 * DEFAULT_CHECK_INTERVAL);
	private volatile Long MaxWriteSize = new Long(DEFAULT_MAX_SIZE);
	
	public readonly long UserDefinedWritabilityIndex;
	public TrafficCounter? TrafficCounter;
	
	private class ReopenReadTimerTask : IRunnable
	{
		private IChannelHandlerContext Context;
		public ReopenReadTimerTask(IChannelHandlerContext ctx)
		{
			Context = ctx;
		}
		
		public void Run()
		{
			IChannel channel = Context.Channel;
			IChannelConfiguration config = channel.Configuration;
			if (!config.AutoRead && IsHandlerActive(Context)) {
				if (logger.DebugEnabled) {
					logger.Debug($"Not unsuspend: {config.AutoRead} : {IsHandlerActive(Context)}");
				}
				channel.GetAttribute(READ_SUSPENDED).Set(new Bool(false));
			} else {
				if (logger.DebugEnabled) {
					if (config.AutoRead && !IsHandlerActive(Context)) {
						if (logger.DebugEnabled) {
							logger.Debug($"Unsuspend: {config.AutoRead} : {IsHandlerActive(Context)}");
						}
					} else {
						if (logger.DebugEnabled) {
							logger.Debug($"Normal unsuspend: {config.AutoRead} : {IsHandlerActive(Context)}");
						}
					}
				}
				channel.GetAttribute(READ_SUSPENDED).Set(new Bool(false));
				config.AutoRead = true;
				channel.Read();
			}
			if (logger.DebugEnabled) {
				logger.Debug($"Unsuspend final status => {config.AutoRead} : {IsHandlerActive(Context)}");
			}
		}
		
	}
	
	private protected virtual long SetUserDefinedWritabilityIndex()
	{
		return CHANNEL_DEFAULT_USER_DEFINED_WRITABILITY_INDEX;
	}

	public AbstractTrafficShapingHandler(long writeLimit, long readLimit, long checkInterval, long maxTime)
	{
		MaxTime.Value = CheckPositive(maxTime, "maxTime");
		UserDefinedWritabilityIndex = SetUserDefinedWritabilityIndex();
		WriteLimit.Value = writeLimit;
		ReadLimit.Value = readLimit;
		CheckInterval.Value = checkInterval;
	}
	
	public AbstractTrafficShapingHandler(long writeLimit, long readLimit, long checkInterval)
			: this(writeLimit, readLimit, checkInterval, DEFAULT_MAX_TIME)
	{
	}
	
	public AbstractTrafficShapingHandler(long writeLimit, long readLimit)
			: this(writeLimit, readLimit, DEFAULT_CHECK_INTERVAL, DEFAULT_MAX_TIME)
	{
	}
	
	public AbstractTrafficShapingHandler()
			: this(0, 0, DEFAULT_CHECK_INTERVAL, DEFAULT_MAX_TIME)
	{
	}
	
	public AbstractTrafficShapingHandler(long checkInterval)
			: this(0, 0, checkInterval, DEFAULT_MAX_TIME)
	{
	}
	
	public void Configure(long newWriteLimit, long newReadLimit)
	{
		WriteLimit.Value = newWriteLimit;
		ReadLimit.Value = newReadLimit;
		if (TrafficCounter != null) {
			TrafficCounter.ResetAccounting(TrafficCounter.MillisecondFromNano());
		}
	}
	
	public void Configure(long newCheckInterval)
	{
		CheckInterval.Value = newCheckInterval;
		if (TrafficCounter != null) {
			TrafficCounter.Configure(CheckInterval.Value);
		}
	}
	
	public void Configure(long newWriteLimit, long newReadLimit, long newCheckInterval)
	{
		Configure(newWriteLimit, newReadLimit);
		Configure(newCheckInterval);
	}
	
	
	public void SetWriteLimit(long writeLimit)
	{
		WriteLimit.Value = writeLimit;
		if (TrafficCounter != null) {
			TrafficCounter.ResetAccounting(TrafficCounter.MillisecondFromNano());
		}
	}
	
	public void SetReadLimit(long readLimit)
	{
		ReadLimit.Value = readLimit;
		if (TrafficCounter != null) {
			TrafficCounter.ResetAccounting(TrafficCounter.MillisecondFromNano());
		}
	}
	
	public void SetCheckInterval(long checkInterval)
	{
		CheckInterval.Value = checkInterval;
		if (TrafficCounter != null) {
			TrafficCounter.Configure(checkInterval);
		}
	}
	
	public void SetMaxTimeWait(long maxTime)
	{
		MaxTime.Value = CheckPositive(maxTime, "maxTime");
	}
	
	public void SetMaxWriteDelay(long maxWriteDelay)
	{
		MaxWriteDelay.Value = CheckPositive(maxWriteDelay, "maxWriteDelay");
	}
	
	public void DoAccounting(TrafficCounter counter)
	{
	}
	
	public void ReleaseReadSuspended(IChannelHandlerContext ctx)
	{
		IChannel channel = ctx.Channel;
		channel.GetAttribute(READ_SUSPENDED).Set(new Bool(false));
		channel.Configuration.AutoRead = true;
	}
	
	public override void ChannelRead(IChannelHandlerContext ctx, object msg)
	{
		long size = CalculateSize(msg);
		long now = TrafficCounter.MillisecondFromNano();
		if (size > 0) {
			long wait = 0;
			if (TrafficCounter != null)
				wait = TrafficCounter.ReadTimeToWait(size, ReadLimit.Value, MaxTime.Value, now);
			wait = CheckWaitReadTime(ctx, wait, now);
			if (wait >= MINIMAL_WAIT) {
				IChannel channel = ctx.Channel;
				IChannelConfiguration config = channel.Configuration;
				if (logger.DebugEnabled) {
					logger.Debug($"Read suspend: {wait} : {config.AutoRead} : {IsHandlerActive(ctx)}");
				}
				if (config.AutoRead && IsHandlerActive(ctx)) {
					config.AutoRead = false;
					channel.GetAttribute(READ_SUSPENDED).Set(new Bool(true));
					IAttribute<IRunnable?> attr = channel.GetAttribute(REOPEN_TASK);
					IRunnable? reopenTask = attr.Get();
					if (reopenTask == null) {
						reopenTask = new ReopenReadTimerTask(ctx);
						attr.Set(reopenTask);
					}
					ctx.Executor.Schedule(reopenTask, TimeSpan.FromMilliseconds(wait));
					if (logger.DebugEnabled) {
						logger.Debug($"Suspend final status => {config.AutoRead} : {IsHandlerActive(ctx)} will be reopened at: {wait}");
					}
				}
			}
		}
		InformReadOperation(ctx, now);
		ctx.FireChannelRead(msg);
	}
	
	public override void HandlerRemoved(IChannelHandlerContext ctx)
	{
		IChannel channel = ctx.Channel;
		if (channel.HasAttribute(REOPEN_TASK)) {
			channel.GetAttribute(REOPEN_TASK).Set(null);
		}
		base.HandlerRemoved(ctx);
	}
	
	public virtual long CheckWaitReadTime(IChannelHandlerContext ctx, long wait, long now)
	{
		return wait;
	}
	
	public virtual void InformReadOperation(IChannelHandlerContext ctx, long now)
	{
	}
	
	public static bool IsHandlerActive(IChannelHandlerContext ctx)
	{
		Bool? suspended = ctx.Channel.GetAttribute(READ_SUSPENDED).Get();
		return suspended == null || suspended.Value == false;
	}
	
	public override void Read(IChannelHandlerContext ctx)
	{
		if (IsHandlerActive(ctx)) {
			ctx.Read();
		}
	}
	
	public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
	{
		long size = CalculateSize(msg);
		long now = TrafficCounter.MillisecondFromNano();
		if (size > 0) {
			long wait = 0;
			if (TrafficCounter != null)
				wait = TrafficCounter.WriteTimeToWait(size, WriteLimit.Value, MaxTime.Value, now);
			if (wait >= MINIMAL_WAIT) {
				if (logger.DebugEnabled) {
					logger.Debug($"Write suspend: {wait} : {ctx.Channel.Configuration.AutoRead} : {IsHandlerActive(ctx)}");
				}
				return SubmitWrite(ctx, msg, size, wait, now);
			}
		}
		return SubmitWrite(ctx, msg, size, 0, now);
	}
	
	[Obsolete]
	public Task SubmitWrite(IChannelHandlerContext ctx, object msg, long delay)
	{
		return SubmitWrite(ctx, msg, CalculateSize(msg), delay, TrafficCounter.MillisecondFromNano());
	}
	
	public abstract Task SubmitWrite(IChannelHandlerContext ctx, object msg, long size, long delay, long now);
	
	public void SetUserDefinedWritability(IChannelHandlerContext ctx, bool writable)
	{
		ChannelOutboundBuffer? cob = ctx.Channel.Unsafe.OutboundBuffer;
		if (cob != null) {
			cob.SetUserDefinedWritability((int) UserDefinedWritabilityIndex, writable);
		}
	}
	
	public override void ChannelRegistered(IChannelHandlerContext ctx)
	{
		SetUserDefinedWritability(ctx, true);
		ChannelRegistered(ctx);
	}
	
	public void CheckWriteSuspend(IChannelHandlerContext ctx, long delay, long queueSize)
	{
		if (queueSize > MaxWriteSize.Value || delay > MaxWriteDelay.Value) {
			SetUserDefinedWritability(ctx, false);
		}
	}
	
	public void ReleaseWriteSuspended(IChannelHandlerContext ctx)
	{
		SetUserDefinedWritability(ctx, true);
	}
	
	public long CalculateSize(object msg)
	{
		if (msg is IByteBuffer) {
			return ((IByteBuffer) msg).ReadableBytes;
		}
		if (msg is IByteBufferHolder) {
			return ((IByteBufferHolder) msg).Content.ReadableBytes;
		}
		if (msg is DefaultFileRegion) {
			return ((DefaultFileRegion) msg).Count;
		}
		return -1;
	}
	
	public override string ToString()
	{
		return $"TrafficShaping with Write Limit: {WriteLimit} Read Limit: {ReadLimit} CheckInterval: {CheckInterval} MaxDelay: {MaxWriteDelay} MaxSize: {MaxWriteSize} and Counter: {TrafficCounter}";
	}
}