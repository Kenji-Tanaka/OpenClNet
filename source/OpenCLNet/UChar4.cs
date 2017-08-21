using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar4 {
		public Byte S0;
		public Byte S1;
		public Byte S2;
		public Byte S3;

		public UChar4(Byte s0, Byte s1, Byte s2, Byte s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
}