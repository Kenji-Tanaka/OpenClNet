using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong2 {
		public UInt64 S0;
		public UInt64 S1;

		public ULong2(UInt64 s0, UInt64 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}