using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long4 {
		public Int64 S0;
		public Int64 S1;
		public Int64 S2;
		public Int64 S3;

		public Long4(Int64 s0, Int64 s1, Int64 s2, Int64 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
}