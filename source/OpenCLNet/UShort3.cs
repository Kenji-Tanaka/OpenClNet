using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort3 {
		public System.UInt16 S0;
		public System.UInt16 S1;
		public System.UInt16 S2;

		public UShort3(System.UInt16 s0, System.UInt16 s1, System.UInt16 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}
}