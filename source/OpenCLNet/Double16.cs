using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double16 {
		public Double S0;
		public Double S1;
		public Double S2;
		public Double S3;
		public Double S4;
		public Double S5;
		public Double S6;
		public Double S7;
		public Double S8;
		public Double S9;
		public Double S10;
		public Double S11;
		public Double S12;
		public Double S13;
		public Double S14;
		public Double S15;
	}
}