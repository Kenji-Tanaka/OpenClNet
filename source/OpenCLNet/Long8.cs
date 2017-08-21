using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long8 {
		public Int64 S0;
		public Int64 S1;
		public Int64 S2;
		public Int64 S3;
		public Int64 S4;
		public Int64 S5;
		public Int64 S6;
		public Int64 S7;
	}
}