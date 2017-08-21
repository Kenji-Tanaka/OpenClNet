using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UInt3 {
		public UInt32 S0;
		public UInt32 S1;
		public UInt32 S2;

		public UInt3(UInt32 s0, UInt32 s1, UInt32 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}
}