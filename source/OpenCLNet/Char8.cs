using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char8 {
		public SByte S0;
		public SByte S1;
		public SByte S2;
		public SByte S3;
		public SByte S4;
		public SByte S5;
		public SByte S6;
		public SByte S7;
	}
}