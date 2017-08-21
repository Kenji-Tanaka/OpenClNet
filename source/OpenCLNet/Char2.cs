using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char2 {
		public SByte S0;
		public SByte S1;

		public Char2(SByte s0, SByte s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}