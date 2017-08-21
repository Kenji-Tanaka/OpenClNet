using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UInt8 {
		public UInt32 S0;
		public UInt32 S1;
		public UInt32 S2;
		public UInt32 S3;
		public UInt32 S4;
		public UInt32 S5;
		public UInt32 S6;
		public UInt32 S7;
	}
}