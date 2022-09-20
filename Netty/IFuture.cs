namespace Netty;

interface IFuture
{
	public bool IsSuccess();
	public bool IsCancellable();
	public Exception Cause();
	
	public bool Await(TimeSpan timeout);
	public bool Await(long timeoutMillis);
	public bool AwaitUinterruptibly(TimeSpan timeout);
	
	public bool Cancel(bool runningInterrupts);
}

interface IFuture<T> : IFuture
{
	
	public IFuture<T> AddListener(GenericFutureListener listener);
	public IFuture<T> AddListeners(params GenericFutureListener[] listeners);
	public IFuture<T> RemoveListener(GenericFutureListener listener);
	public IFuture<T> RemoveListeners(GenericFutureListener listeners);
	
	public IFuture<T> Sync();
	public IFuture<T> SyncUninterruptibly();
	public IFuture<T> Await();
	public IFuture<T> AwaitUninterruptibly();
	
	public T GetNow();
	
}