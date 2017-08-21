using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int8 {
		public Int32 S0;
		public Int32 S1;
		public Int32 S2;
		public Int32 S3;
		public Int32 S4;
		public Int32 S5;
		public Int32 S6;
		public Int32 S7;
	}
}