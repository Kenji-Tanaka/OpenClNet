namespace OpenCLNet {
	public enum DeviceExecCapabilities : ulong {
		KERNEL = (1 << 0),
		NATIVE_KERNEL = (1 << 1)
	}
}