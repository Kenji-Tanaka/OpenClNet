using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort2 {
		public System.UInt16 S0;
		public System.UInt16 S1;

		public UShort2(System.UInt16 s0, System.UInt16 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}