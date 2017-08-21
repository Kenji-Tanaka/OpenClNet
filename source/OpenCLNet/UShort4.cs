using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort4 {
		public System.UInt16 S0;
		public System.UInt16 S1;
		public System.UInt16 S2;
		public System.UInt16 S3;

		public UShort4(System.UInt16 s0, System.UInt16 s1, System.UInt16 s2, System.UInt16 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
}