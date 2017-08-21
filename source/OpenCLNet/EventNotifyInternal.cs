using System;

namespace OpenCLNet {
	public delegate void EventNotifyInternal(IntPtr _event, Int32 eventCommandExecStatus, IntPtr userData);
}