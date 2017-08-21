using System;

namespace OpenCLNet {
	public unsafe delegate ErrorCode clGetDeviceIDsFromD3D10KHRDelegate(IntPtr platform, UInt32 d3d_device_source, void* d3d_object, UInt32 d3d_device_set, UInt32 num_entries, IntPtr* devices, UInt32* num_devices);
}