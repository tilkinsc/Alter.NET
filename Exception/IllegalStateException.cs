namespace Exceptions;

[System.Serializable]
public class IllegalStateException : System.Exception
{
	public IllegalStateException() { }
	public IllegalStateException(string message) : base(message) { }
	public IllegalStateException(string message, System.Exception inner) : base(message, inner) { }
	protected IllegalStateException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
