using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	public unsafe delegate ErrorCode clCreateSubDevicesEXTDelegate(
		IntPtr in_device,
		[In] Byte[] properties,
		UInt32 num_entries,
		[In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] IntPtr[] out_devices,
		[Out] UInt32* num_devices);
}