using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char4 {
		public SByte S0;
		public SByte S1;
		public SByte S2;
		public SByte S3;

		public Char4(SByte s0, SByte s1, SByte s2, SByte s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
}