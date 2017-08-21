using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float3 {
		public Single S0;
		public Single S1;
		public Single S2;

		public Float3(Single s0, Single s1, Single s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}
}