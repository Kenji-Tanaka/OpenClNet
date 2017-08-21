using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar2 {
		public Byte S0;
		public Byte S1;

		public UChar2(Byte s0, Byte s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}