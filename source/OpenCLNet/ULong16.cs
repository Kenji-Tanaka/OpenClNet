using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong16 {
		public UInt64 S0;
		public UInt64 S1;
		public UInt64 S2;
		public UInt64 S3;
		public UInt64 S4;
		public UInt64 S5;
		public UInt64 S6;
		public UInt64 S7;
		public UInt64 S8;
		public UInt64 S9;
		public UInt64 S10;
		public UInt64 S11;
		public UInt64 S12;
		public UInt64 S13;
		public UInt64 S14;
		public UInt64 S15;
	}
}