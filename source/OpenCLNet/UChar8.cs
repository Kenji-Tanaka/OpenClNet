using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar8 {
		public Byte S0;
		public Byte S1;
		public Byte S2;
		public Byte S3;
		public Byte S4;
		public Byte S5;
		public Byte S6;
		public Byte S7;
	}
}