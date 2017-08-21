namespace OpenCLNet {
	public class OpenCLNotAvailableException : OpenCLException {
		public OpenCLNotAvailableException()
			: base("OpenCL not available") { }
	}
}