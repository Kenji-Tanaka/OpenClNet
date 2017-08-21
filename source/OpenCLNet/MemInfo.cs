namespace OpenCLNet {
	public enum MemInfo {
		TYPE = 0x1100,
		FLAGS = 0x1101,
		SIZE = 0x1102,
		HOST_PTR = 0x1103,
		MAP_COUNT = 0x1104,
		REFERENCE_COUNT = 0x1105,
		CONTEXT = 0x1106,
		ASSOCIATED_MEMOBJECT = 0x1107,
		OFFSET = 0x1108,

		// D3D10 extension
		D3D10_RESOURCE_KHR = 0x4015
	}
}