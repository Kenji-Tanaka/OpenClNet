using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int2 {
		public Int32 S0;
		public Int32 S1;

		public Int2(Int32 s0, Int32 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}