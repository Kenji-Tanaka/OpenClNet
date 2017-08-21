using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Short4 {
		public System.Int16 S0;
		public System.Int16 S1;
		public System.Int16 S2;
		public System.Int16 S3;

		public Short4(System.Int16 s0, System.Int16 s1, System.Int16 s2, System.Int16 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
}