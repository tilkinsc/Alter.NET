using Exceptions;
using Util;

namespace Cache.Index;


class RLIndexData
{
	
	public int Protocol;
	public int Revision;
	public bool Named;
	public RLArchiveData[]? Archives;
	
	public void Load(byte[] data)
	{
		BinaryReader stream = new BinaryReader(new MemoryStream(data));
		Protocol = (int) stream.ReadByte();
		if (Protocol < 5 || Protocol > 7)
			throw new IllegalArgumentException("Unsupported protocol");
		else if (Protocol >= 6) {
			Revision = stream.ReadInt32();
		}
		
		int hash = (int) stream.ReadByte();
		Named = (1 & hash) != 0;
		if ((hash & ~1) != 0) {
			throw new IllegalArgumentException("Unknown flags");
		}
		// assert (hash & ~3) == 0;
		
		int validArchivesCount = Protocol >= 7 ? stream.ReadBigSmart() : stream.ReadUInt16();
		int lastArchiveId = 0;
		
		Archives = new RLArchiveData[validArchivesCount];
		for (int i=0; i<validArchivesCount; i++)
		{
			int archive = lastArchiveId += Protocol >= 7 ? stream.ReadBigSmart() : stream.ReadUInt16();
			
			RLArchiveData ad = new RLArchiveData();
			ad.ID = archive;
			Archives[i] = ad;
		}
		
		if (Named) {
			for (int i=0; i<validArchivesCount; i++)
			{
				int nameHash = stream.ReadInt32();
				RLArchiveData ad = Archives[i];
				ad.NameHash = nameHash;
			}
		}
		
		for (int i=0; i<validArchivesCount; i++)
		{
			int crc = stream.ReadInt32();
			
			RLArchiveData ad = Archives[i];
			ad.CRC = crc;
		}
		
		for (int i=0; i<validArchivesCount; i++)
		{
			int revision = stream.ReadInt32();
			
			RLArchiveData ad = Archives[i];
			ad.Revision = revision;
		}
		
		int[] numberOfFiles = new int[validArchivesCount];
		for (int i=0; i<validArchivesCount; i++)
		{
			int num = Protocol >= 7 ? stream.ReadBigSmart() : stream.ReadUInt16();
			numberOfFiles[i] = num;
		}
		
		for (int i=0; i<validArchivesCount; i++)
		{
			RLArchiveData ad = Archives[i];
			int num = numberOfFiles[i];
			
			ad.Files = new RLFileData[num];
			
			int last = 0;
			for (int j=0; j<num; j++)
			{
				int fileId = last += Protocol >= 7 ? stream.ReadBigSmart() : stream.ReadUInt16();
				
				RLFileData fd = ad.Files[i] = new RLFileData();
				fd.ID = fileId;
			}
		}
		
		if (Named) {
			for (int i=0; i<validArchivesCount; i++)
			{
				RLArchiveData ad = Archives[i];
				int num = numberOfFiles[i];
				
				for (int j=0; j<num; j++)
				{
					if (ad.Files != null) {
						RLFileData fd = ad.Files[i];
						int name = stream.ReadInt32();
						fd.NameHash = name;
					} else {
						throw new IllegalArgumentException($"ad.Files == null");
					}
				}
			}
		}
			
	}
	
	public byte[]? WriteIndexData()
	{
		if (Archives == null)
			return null;
		MemoryStream mem = new MemoryStream();
		BinaryWriter stream = new BinaryWriter(mem);
		stream.Write((byte) Protocol);
		if (Protocol >= 6) {
			stream.Write(Revision);
		}
		
		stream.Write(Named ? (byte) 1 : (byte) 0);
		if (Protocol >= 7) {
			stream.WriteBigSmart(Archives.Length);
		} else {
			stream.Write((short) Archives.Length);
		}
		
		for (int i=0; i<Archives.Length; i++)
		{
			RLArchiveData a = Archives[i];
			int archive = a.ID;
			
			if (i != 0) {
				RLArchiveData prev = Archives[i - 1];
				archive -= prev.ID;
			}
			
			if (Protocol >= 7) {
				stream.WriteBigSmart(archive);
			} else {
				stream.Write((short) archive);
			}
		}
		
		if (Named) {
			for (int i=0; i<Archives.Length; i++)
			{
				RLArchiveData a = Archives[i];
				stream.Write(a.NameHash);
			}
		}
		
		for (int i=0; i<Archives.Length; i++)
		{
			RLArchiveData a = Archives[i];
			stream.Write(a.CRC);
		}
		
		for (int i=0; i<Archives.Length; i++)
		{
			RLArchiveData a = Archives[i];
			if (a.Files == null)
				throw new IllegalArgumentException();
			
			int len = a.Files.Length;
			if (Protocol >= 7) {
				stream.WriteBigSmart(len);
			} else {
				stream.Write((short) len);
			}
		}
		
		for (int i=0; i<Archives.Length; i++)
		{
			RLArchiveData a = Archives[i];
			if (a.Files == null)
				throw new IllegalArgumentException();
			
			for (int j=0; j<a.Files.Length; j++)
			{
				RLFileData file = a.Files[j];
				int offset = file.ID;
				
				if (j != 0) {
					RLFileData prev = a.Files[j - 1];
					offset -= prev.ID;
				}
				
				if (Protocol >= 7) {
					stream.WriteBigSmart(offset);
				} else {
					stream.Write((short) offset);
				}
			}
		}
		
		if (Named) {
			for (int i=0; i<Archives.Length; i++)
			{
				RLArchiveData a = Archives[i];
				if (a.Files == null)
					throw new IllegalArgumentException();
				
				for (int j=0; j<a.Files.Length; j++)
				{
					RLFileData file = a.Files[j];
					stream.Write(file.NameHash);
				}
			}
		}
		byte[] array = mem.ToArray();
		Array.Reverse(array);
		return array;
	}
	
}