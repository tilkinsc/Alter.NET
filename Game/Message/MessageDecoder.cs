using Util;
using Net.Packet;

namespace Game.Message;

abstract class MessageDecoder<T> where T : IMessage
{
	
	public virtual T Decode(int opcode, MessageStructure structure, GamePacketReader reader)
	{
		Dictionary<string, Number> values = new Dictionary<string, Number>();
		Dictionary<string, string> stringValues = new Dictionary<string, string>();
		foreach (KeyValuePair<string, MessageValue> msg in structure.Values)
		{
			MessageValue value = msg.Value;
			if (value.Type == DataType.BYTES)
				throw new Exception("Cannot decode message with type BYTES");
			
			if (value.Type == DataType.STRING) {
				stringValues[value.ID] = reader.SignedSmart;
				continue;
			}
			if (value.Type == DataType.SMART) {
				if (value.Signature == DataSignature.SIGNED) {
					values[value.ID] = reader.SignedSmart;
				} else {
					values[value.ID] = reader.UnsignedSmart;
				}
				continue;
			}
			if (value.Signature == DataSignature.SIGNED) {
				values[value.ID] = reader.GetSigned(value.Type, value.Order, value.Transformation).ToNumber();
			} else {
				values[value.ID] = reader.GetUnsigned(value.Type, value.Order, value.Transformation).ToNumber();
			}
		}
		return Decode(opcode, Array.IndexOf(structure.Opcodes, opcode), values, stringValues);
	}
	
	public abstract T Decode(int opcode, int opcodeIndex, Dictionary<string, Number> values, Dictionary<string, string> stringValues);
	
}