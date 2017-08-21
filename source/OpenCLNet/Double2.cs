using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double2 {
		public Double S0;
		public Double S1;

		public Double2(Double s0, Double s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}