namespace Game.Model.Bits;

static class BitStorages
{
	public static readonly BitStorage INFINITE_VARS_STORAGE = new BitStorage("inf_vars");
}

class InfiniteVarsType : StorageBits
{
	public static readonly InfiniteVarsType RUN = new InfiniteVarsType(0);
	public static readonly InfiniteVarsType PRAY = new InfiniteVarsType(1);
	public static readonly InfiniteVarsType HP = new InfiniteVarsType(2);
	
	public InfiniteVarsType(int startBit, int? endBit = null)
			: base(startBit, endBit == null ? startBit : (int) endBit)
	{
	}
}