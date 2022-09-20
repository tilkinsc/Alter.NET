namespace Netty;

abstract class GenericFutureListener
{
}

abstract class GenericFutureListener<T> : GenericFutureListener where T : IFuture
{
	
	public Action<Task, object?>? OperationComplete;
	
}