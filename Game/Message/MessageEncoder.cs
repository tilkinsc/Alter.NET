using Util;

namespace Game.Message;

abstract class MessageEncoder<T> where T : Message
{
	
	public void Encode(T message, GamePacketBuilder builder, MessageStructure structure)
	{
		foreach (var? value in structure.Values)
		{
			if (value.Type != DataType.BYTES) {
				builder.Put(value.Type, value.Order, value.Transformation, Extract(message, value.ID));
			} else {
				builder.PutBytes(ExtractBytes(message, value.ID));
			}
		}
	}
	
	public abstract Number Extract(T message, string key);
	public abstract byte[] ExtractBytes(T message, string key);
	
}