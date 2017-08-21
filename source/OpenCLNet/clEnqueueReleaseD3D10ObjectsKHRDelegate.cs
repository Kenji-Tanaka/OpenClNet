using System;

namespace OpenCLNet {
	public unsafe delegate ErrorCode clEnqueueReleaseD3D10ObjectsKHRDelegate(IntPtr command_queue, UInt32 num_objects, IntPtr* mem_objects, UInt32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event);
}