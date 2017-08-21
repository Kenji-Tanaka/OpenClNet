using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Short2 {
		public System.Int16 S0;
		public System.Int16 S1;

		public Short2(System.Int16 s0, System.Int16 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
}