namespace OpenCLNet {
	public enum ContextProperties : ulong {
		PLATFORM = 0x1084,

		// Additional cl_context_properties for GL support
		GL_CONTEXT_KHR = 0x2008,
		EGL_DISPLAY_KHR = 0x2009,
		GLX_DISPLAY_KHR = 0x200A,
		WGL_HDC_KHR = 0x200B,
		CGL_SHAREGROUP_KHR = 0x200C
	}
}