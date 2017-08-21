using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong4 {
		public UInt64 S0;
		public UInt64 S1;
		public UInt64 S2;
		public UInt64 S3;

		public ULong4(UInt64 s0, UInt64 s1, UInt64 s2, UInt64 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
}