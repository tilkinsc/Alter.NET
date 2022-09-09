namespace Exceptions;

[System.Serializable]
public class IllegalArgumentException : System.Exception
{
	public IllegalArgumentException() { }
	public IllegalArgumentException(string message) : base(message) { }
	public IllegalArgumentException(string message, System.Exception inner) : base(message, inner) { }
	protected IllegalArgumentException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
