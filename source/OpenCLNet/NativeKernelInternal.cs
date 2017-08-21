using System.Runtime.InteropServices;

namespace OpenCLNet {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void NativeKernelInternal(void* pArgs);
}