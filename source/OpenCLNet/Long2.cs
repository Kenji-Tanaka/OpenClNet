using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long2 {
		public Int64 S0;
		public Int64 S1;

		public Long2(Int64 s0, Int64 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}