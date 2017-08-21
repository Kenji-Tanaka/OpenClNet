using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double8 {
		public Double S0;
		public Double S1;
		public Double S2;
		public Double S3;
		public Double S4;
		public Double S5;
		public Double S6;
		public Double S7;
	}
}