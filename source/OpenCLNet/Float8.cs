using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float8 {
		public Single S0;
		public Single S1;
		public Single S2;
		public Single S3;
		public Single S4;
		public Single S5;
		public Single S6;
		public Single S7;
	}
}