using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int3 {
		public Int32 S0;
		public Int32 S1;
		public Int32 S2;

		public Int3(Int32 s0, Int32 s1, Int32 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}
}