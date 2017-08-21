using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double4 {
		public Double S0;
		public Double S1;
		public Double S2;
		public Double S3;

		public Double4(Double s0, Double s1, Double s2, Double s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
}