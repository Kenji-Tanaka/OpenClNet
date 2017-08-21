using System;

namespace OpenCLNet {
	[Flags]
	public enum CommandQueueProperties : ulong {
		NONE = 0,
		OUT_OF_ORDER_EXEC_MODE_ENABLE = 1 << 0,
		PROFILING_ENABLE = 1 << 1
	}
}