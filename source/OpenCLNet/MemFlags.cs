namespace OpenCLNet {
	public enum MemFlags : ulong {
		READ_WRITE = (1 << 0),
		WRITE_ONLY = (1 << 1),
		READ_ONLY = (1 << 2),
		USE_HOST_PTR = (1 << 3),
		ALLOC_HOST_PTR = (1 << 4),
		COPY_HOST_PTR = (1 << 5)
	}
}