using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong8 {
		public UInt64 S0;
		public UInt64 S1;
		public UInt64 S2;
		public UInt64 S3;
		public UInt64 S4;
		public UInt64 S5;
		public UInt64 S6;
		public UInt64 S7;
	}
}