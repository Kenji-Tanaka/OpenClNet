namespace OpenCLNet {
	public enum CommandType {
		NDRANGE_KERNEL = 0x11F0,
		TASK = 0x11F1,
		NATIVE_KERNEL = 0x11F2,
		READ_BUFFER = 0x11F3,
		WRITE_BUFFER = 0x11F4,
		COPY_BUFFER = 0x11F5,
		READ_IMAGE = 0x11F6,
		WRITE_IMAGE = 0x11F7,
		COPY_IMAGE = 0x11F8,
		COPY_IMAGE_TO_BUFFER = 0x11F9,
		COPY_BUFFER_TO_IMAGE = 0x11FA,
		MAP_BUFFER = 0x11FB,
		MAP_IMAGE = 0x11FC,
		UNMAP_MEM_OBJECT = 0x11FD,
		MARKER = 0x11FE,
		ACQUIRE_GL_OBJECTS = 0x11FF,
		RELEASE_GL_OBJECTS = 0x1200,
		READ_BUFFER_RECT = 0x1201,
		WRITE_BUFFER_RECT = 0x1202,
		COPY_BUFFER_RECT = 0x1203,
		USER = 0x1204,

		// D3D10 extension
		ACQUIRE_D3D10_OBJECTS_KHR = 0x4017,
		RELEASE_D3D10_OBJECTS_KHR = 0x4018
	}
}