using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float2 {
		public Single S0;
		public Single S1;

		public Float2(Single s0, Single s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}