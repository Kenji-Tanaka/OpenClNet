using System;

namespace OpenCLNet {
	public unsafe delegate IntPtr clCreateFromD3D10BufferKHRDelegate(IntPtr context, UInt64 flags, IntPtr* resource, out ErrorCode errcode_ret);
}