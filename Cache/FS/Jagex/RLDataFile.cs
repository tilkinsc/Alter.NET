namespace Cache.FS.Jagex;


class RLDataFile : IDisposable
{
	
	public const int SECTOR_SIZE = 520;
	
	public FileStream DatFile;
	
	private object _Lock = new object();
	
	public RLDataFile(FileStream datFile)
	{
		DatFile = datFile;
	}
	
	public void Clear()
	{
		DatFile.SetLength(0);
	}
	
	public byte[]? Read(int indexId, int archiveId, int sector, int size)
	{
		lock (_Lock)
		{
			if (sector <= 0L || (DatFile.Length / SECTOR_SIZE) < (long) sector) {
				return null;
			}
			
			byte[] readBuffer = new byte[SECTOR_SIZE];
			MemoryStream mem = new MemoryStream();
			
			for (int part=0, readBytesCount=0, nextSector; size > readBytesCount; sector = nextSector)
			{
				if (sector == 0) {
					return null;
				}
				
				DatFile.Seek(SECTOR_SIZE * sector, SeekOrigin.Begin);
				
				int dataBlockSize = size - readBytesCount;
				byte headerSize;
				int currentIndex;
				int currentPart;
				int currentArchive;
				if (archiveId > 0xFFFF) {
					headerSize = 10;
					if (dataBlockSize > SECTOR_SIZE - headerSize) {
						dataBlockSize = SECTOR_SIZE - headerSize;
					}
					
					int i = DatFile.Read(readBuffer, 0, headerSize + dataBlockSize);
					if (i != headerSize + dataBlockSize) {
						return null;
					}
					
					currentArchive = ((readBuffer[0] & 0xFF) << 24)
							| ((readBuffer[1] & 0xFF) << 16)
							| ((readBuffer[2] & 0xFF) << 8)
							| ((readBuffer[3] & 0xFF));
					currentPart = ((readBuffer[4] & 0xFF) << 8) + (readBuffer[5] & 0xFF);
					nextSector = ((readBuffer[6] & 0xFF) << 16)
							| ((readBuffer[7] & 0xFF) << 8)
							| ((readBuffer[8] & 0xFF));
					currentIndex = readBuffer[9] & 0xFF;
				} else {
					headerSize = 8;
					if (dataBlockSize > SECTOR_SIZE - headerSize) {
						dataBlockSize = SECTOR_SIZE - headerSize;
					}
					
					int i = DatFile.Read(readBuffer, 0, headerSize + dataBlockSize);
					if (i != headerSize + dataBlockSize) {
						return null;
					}
					
					currentArchive = ((readBuffer[0] & 0xFF) << 8)
							| (readBuffer[1] & 0xFF);
					currentPart = ((readBuffer[2] & 0xFF) << 8)
							| (readBuffer[3] & 0xFF);
					nextSector = ((readBuffer[4] & 0xFF) << 16)
							| ((readBuffer[5] & 0xFF) << 8)
							| ((readBuffer[6] & 0xFF));
					currentIndex = readBuffer[7] & 0xFF;
				}
				
				if (archiveId != currentArchive || part != currentPart || indexId != currentIndex) {
					return null;
				}
				
				if (nextSector < 0 || DatFile.Length / SECTOR_SIZE < (long) nextSector) {
					return null;
				}
				
				mem.Write(readBuffer, headerSize, dataBlockSize);
				readBytesCount += dataBlockSize;
				part++;
			}
			
			// TODO: check the other way of reversing bytes
			return mem.ToArray().Reverse().ToArray();
		}
	}
	
	public RLDataFileWriteResult Write(int indexId, int archiveId, byte[] compressedData)
	{
		lock (_Lock)
		{
			int sector;
			int startSector;
			
			byte[] writeBuffer = new byte[SECTOR_SIZE];
			MemoryStream mem = new MemoryStream(compressedData);
			
			sector = (int) ((DatFile.Length + (long) (SECTOR_SIZE - 1)) / (long) SECTOR_SIZE);
			if (sector == 0)
				sector = 1;
			startSector = sector;
			
			for (int part=0; mem.Length > 0; part++)
			{
				int nextSector = sector + 1;
				int dataToWrite;
				
				if (archiveId > 0xFFFF) {
					if (mem.Length <= 510) {
						nextSector = 0;
					}
					
					writeBuffer[0] = (byte) (archiveId >> 24);
					writeBuffer[1] = (byte) (archiveId >> 16);
					writeBuffer[2] = (byte) (archiveId >> 8);
					writeBuffer[3] = (byte) (archiveId);
					writeBuffer[4] = (byte) (part >> 8);
					writeBuffer[5] = (byte) (part);
					writeBuffer[6] = (byte) (nextSector >> 16);
					writeBuffer[7] = (byte) (nextSector >> 8);
					writeBuffer[8] = (byte) (nextSector);
					writeBuffer[9] = (byte) (indexId);
					DatFile.Seek(SECTOR_SIZE * sector, SeekOrigin.Begin);
					DatFile.Write(writeBuffer, 0, 10);
					
					dataToWrite = (int) mem.Length;
					if (dataToWrite > 510) {
						dataToWrite = 510;
					}
				} else {
					if (mem.Length <= 512) {
						nextSector = 0;
					}
					
					writeBuffer[0] = (byte) (archiveId >> 8);
					writeBuffer[1] = (byte) (archiveId);
					writeBuffer[2] = (byte) (part >> 8);
					writeBuffer[3] = (byte) (part);
					writeBuffer[4] = (byte) (nextSector >> 16);
					writeBuffer[5] = (byte) (nextSector >> 8);
					writeBuffer[6] = (byte) (nextSector);
					writeBuffer[7] = (byte) (indexId);
					DatFile.Seek(SECTOR_SIZE * sector, SeekOrigin.Begin);
					DatFile.Write(writeBuffer, 0, 8);
					
					dataToWrite = (int) mem.Length;
					if (dataToWrite > 512) {
						dataToWrite = 512;
					}
				}
				
				mem.Write(writeBuffer, 0, dataToWrite);
				DatFile.Write(writeBuffer, 0, dataToWrite);
				sector = nextSector;
			}
			
			RLDataFileWriteResult res = new RLDataFileWriteResult();
			res.Sector = startSector;
			res.CompressedLength = compressedData.Length;
			return res;
		}
	}
	
	public void Dispose()
	{
		DatFile.Close();
	}
	
}

