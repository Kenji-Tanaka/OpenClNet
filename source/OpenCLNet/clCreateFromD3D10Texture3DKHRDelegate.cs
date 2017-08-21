using System;

namespace OpenCLNet {
	public unsafe delegate IntPtr clCreateFromD3D10Texture3DKHRDelegate(IntPtr context, UInt64 flags, IntPtr* resource, UInt32 subresource, out ErrorCode errcode_ret);
}