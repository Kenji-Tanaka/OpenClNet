using System;

namespace OpenCLNet {
	[Flags]
	public enum DeviceType : ulong {
		DEFAULT = 1 << 0,
		CPU = 1 << 1,
		GPU = 1 << 2,
		ACCELERATOR = 1 << 3,
		ALL = 0xFFFFFFFF
	}
}