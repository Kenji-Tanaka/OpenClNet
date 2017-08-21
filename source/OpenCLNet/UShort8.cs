using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort8 {
		public System.UInt16 S0;
		public System.UInt16 S1;
		public System.UInt16 S2;
		public System.UInt16 S3;
		public System.UInt16 S4;
		public System.UInt16 S5;
		public System.UInt16 S6;
		public System.UInt16 S7;
	}
}