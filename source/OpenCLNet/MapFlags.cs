namespace OpenCLNet {
	public enum MapFlags : ulong {
		READ = (1 << 0),
		WRITE = (1 << 1),
		READ_WRITE = (MapFlags.READ + MapFlags.WRITE)
	}
}