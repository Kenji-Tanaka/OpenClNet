using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BufferRegion {
		public IntPtr Origin;
		public IntPtr Size;
	}
}