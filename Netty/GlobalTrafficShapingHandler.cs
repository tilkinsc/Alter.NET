using DotNetty.Buffers;
using DotNetty.Common.Concurrency;
using DotNetty.Common.Internal;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace Netty;

class GlobalTrafficShapingHandler : AbstractTrafficShapingHandler
{
	
	// TODO: missing promise
	private sealed class ToSend
	{
		public readonly long RelativeTimeAction;
		public readonly object Data;
		public readonly long Size;
		
		public ToSend(long delay, object data, long size)
		{
			RelativeTimeAction = delay;
			Data = data;
			Size = size;
		}
	}
	
	private sealed class PerChannel
	{
		public Deque<ToSend> MessagesQueue = new Deque<ToSend>();
		public long QueueSize;
		public long LastWriteTimestamp;
		public long LastReadTimestamp;
	}
	
	private readonly IDictionary<int, PerChannel> ChannelQueues = PlatformDependent.NewConcurrentHashMap<int, PerChannel>();
	private AtomicReference<Long> QueuesSize = new AtomicReference<Long>();
	
	private long MaxGlobalWriteSize = DEFAULT_MAX_SIZE * 100;
	
	public void CreateGlobalTrafficCounter(IScheduledExecutorService executor)
	{
		TrafficCounter tc = new TrafficCounter(this, ObjectUtil.CheckNotNull(executor, "executor"), "GlobalTC", CheckInterval.Value);
		TrafficCounter = tc;
		tc.Start();
	}
	
	public GlobalTrafficShapingHandler(IScheduledExecutorService executor, long writeLimit, long readLimit, long checkInterval, long maxTime)
			: base(writeLimit, readLimit, checkInterval, maxTime)
	{
		CreateGlobalTrafficCounter(executor);
	}
	
	public GlobalTrafficShapingHandler(IScheduledExecutorService executor, long writeLimit, long readLimit, long checkInterval)
			: base(writeLimit, readLimit, checkInterval)
	{
		CreateGlobalTrafficCounter(executor);
	}
	
	public GlobalTrafficShapingHandler(IScheduledExecutorService executor, long writeLimit, long readLimit)
			: base(writeLimit, readLimit)
	{
		CreateGlobalTrafficCounter(executor);
	}
	
	public GlobalTrafficShapingHandler(IScheduledExecutorService executor, long checkInterval)
			: base(checkInterval)
	{
		CreateGlobalTrafficCounter(executor);
	}
	
	public GlobalTrafficShapingHandler(IEventExecutor executor)
	{
		CreateGlobalTrafficCounter(executor);
	}
	
	private protected override long SetUserDefinedWritabilityIndex()
	{
		return AbstractTrafficShapingHandler.GLOBAL_DEFAULT_USER_DEFINED_WRITABILITY_INDEX;
	}
	
	public long GetMaxGlobalWriteSize()
	{
		return MaxGlobalWriteSize;
	}
	
	public void SetMaxGlobalWriteSize(long maxGlobalWriteSize)
	{
		MaxGlobalWriteSize = maxGlobalWriteSize;
	}
	
	public long GetQueuesSize()
	{
		return QueuesSize.Value.Value;
	}
	
	public void Release()
	{
		TrafficCounter!.Stop();
	}
	
	private PerChannel GetOrSetPerChannel(IChannelHandlerContext ctx)
	{
		IChannel channel = ctx.Channel;
		int key = channel.GetHashCode();
		ChannelQueues.TryGetValue(key, out PerChannel? perChannel);
		if (perChannel == null) {
			perChannel = new PerChannel();
			perChannel.MessagesQueue = new Deque<ToSend>();
			perChannel.QueueSize = 0;
			perChannel.LastReadTimestamp = TrafficCounter.MillisecondFromNano();
			perChannel.LastWriteTimestamp = perChannel.LastReadTimestamp;
			ChannelQueues.Add(key, perChannel);
		}
		return perChannel;
	}
	
	public override void HandlerAdded(IChannelHandlerContext ctx)
	{
		GetOrSetPerChannel(ctx);
		base.HandlerAdded(ctx);
	}
	
	public override void HandlerRemoved(IChannelHandlerContext ctx)
	{
		IChannel channel = ctx.Channel;
		int key = channel.GetHashCode();
		if (ChannelQueues.TryGetValue(key, out PerChannel? perChannel))
		{
			lock (perChannel)
			{
				if (channel.Active) {
					while (perChannel.MessagesQueue.HasNext)
					{
						ToSend toSend = perChannel.MessagesQueue.DequeueFirst()!;
						long size = CalculateSize(toSend.Data);
						TrafficCounter!.BytesRealWriteFlowControl(size);
						perChannel.QueueSize -= size;
						QueuesSize.Value.Value -= size;
						ctx.WriteAsync(toSend.Data);
					}
				} else {
					QueuesSize.Value.Value -= perChannel.QueueSize;
					while (perChannel.MessagesQueue.HasNext)
					{
						ToSend toSend = perChannel.MessagesQueue.DequeueFirst()!;
						if (toSend.Data is IByteBuffer) {
							((IByteBuffer) toSend.Data).Release();
						}
					}
				}
				perChannel.MessagesQueue.Clear();
			}
		}
		ReleaseWriteSuspended(ctx);
		ReleaseReadSuspended(ctx);
		base.HandlerRemoved(ctx);
	}
	
	public override long CheckWaitReadTime(IChannelHandlerContext ctx, long wait, long now)
	{
		int key = ctx.Channel.GetHashCode();
		ChannelQueues.TryGetValue(key, out PerChannel? perChannel);
		if (perChannel != null) {
			if (wait > MaxTime.Value && now + wait - perChannel.LastReadTimestamp > MaxTime.Value) {
				wait = MaxTime.Value;
			}
		}
		return wait;
	}
	
	public override void InformReadOperation(IChannelHandlerContext ctx, long now)
	{
		int key = ctx.Channel.GetHashCode();
		ChannelQueues.TryGetValue(key, out PerChannel? perChannel);
		if (perChannel != null) {
			perChannel.LastReadTimestamp = now;
		}
	}
	
	private void SendAllValid(IChannelHandlerContext ctx, PerChannel perChannel, long now)
	{
		lock (perChannel)
		{
			while (perChannel.MessagesQueue.HasNext)
			{
				ToSend newToSend = perChannel.MessagesQueue.DequeueFirst()!;
				if (newToSend.RelativeTimeAction <= now) {
					long size = newToSend.Size;
					TrafficCounter!.BytesRealWriteFlowControl(size);
					perChannel.QueueSize -= size;
					QueuesSize.Value.Value -= size;
					ctx.WriteAsync(newToSend.Data);
					perChannel.LastWriteTimestamp = now;
				} else {
					perChannel.MessagesQueue.EnqueueLast(newToSend);
					break;
				}
			}
			if (perChannel.MessagesQueue.HasNext) {
				ReleaseWriteSuspended(ctx);
			}
		}
		ctx.Flush();
	}
	
	public override Task SubmitWrite(IChannelHandlerContext ctx, object msg, long size, long writeDelay, long now)
	{
		IChannel channel = ctx.Channel;
		int key = channel.GetHashCode();
		ChannelQueues.TryGetValue(key, out PerChannel? perChannel);
		if (perChannel == null) {
			perChannel = GetOrSetPerChannel(ctx);
		}
		ToSend? newToSend;
		long delay = writeDelay;
		bool globalSizeExceeded = false;
		lock (perChannel)
		{
			if (writeDelay == 0 && perChannel.MessagesQueue.HasNext) {
				TrafficCounter!.BytesRealWriteFlowControl(size);
				perChannel.LastWriteTimestamp = now;
				return ctx.WriteAsync(msg);
			}
			if (delay > MaxTime.Value && now + delay - perChannel.LastWriteTimestamp > MaxTime.Value) {
				delay = MaxTime.Value;
			}
			newToSend = new ToSend(delay + now, msg, size);
			perChannel.MessagesQueue.EnqueueLast(newToSend);
			perChannel.QueueSize += size;
			QueuesSize.Value.Value += size;
			CheckWriteSuspend(ctx, delay, perChannel.QueueSize);
			if (QueuesSize.Value.Value > MaxGlobalWriteSize) {
				globalSizeExceeded = true;
			}
		}
		if (globalSizeExceeded) {
			SetUserDefinedWritability(ctx, false);
		}
		long futureNow = newToSend.RelativeTimeAction;
		PerChannel forSchedule = perChannel;
		ctx.Executor.Schedule(() => {
			SendAllValid(ctx, forSchedule, futureNow);
		}, TimeSpan.FromMilliseconds(delay));
	}
	
}