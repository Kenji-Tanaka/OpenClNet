namespace OpenCLNet {
	public enum ImageInfo {
		FORMAT = 0x1110,
		ELEMENT_SIZE = 0x1111,
		ROW_PITCH = 0x1112,
		SLICE_PITCH = 0x1113,
		WIDTH = 0x1114,
		HEIGHT = 0x1115,
		DEPTH = 0x1116,

		// D3D10 extension
		D3D10_SUBRESOURCE_KHR = 0x4016
	}
}