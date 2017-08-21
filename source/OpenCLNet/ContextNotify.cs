using System;

namespace OpenCLNet {
	public delegate void ContextNotify(String errInfo, Byte[] data, IntPtr cb, IntPtr userData);
}