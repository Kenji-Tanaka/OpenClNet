using System;

namespace OpenCLNet {
	public class OpenCLException : Exception {
		public ErrorCode ErrorCode = ErrorCode.SUCCESS;

		public OpenCLException() { }

		public OpenCLException(ErrorCode errorCode) {
			this.ErrorCode = errorCode;
		}

		public OpenCLException(String errorMessage)
			: base(errorMessage) { }

		public OpenCLException(String errorMessage, ErrorCode errorCode)
			: base(errorMessage) {
			this.ErrorCode = errorCode;
		}

		public OpenCLException(String message, Exception innerException)
			: base(message, innerException) { }

		public OpenCLException(String message, ErrorCode errorCode, Exception innerException)
			: base(message, innerException) {
			this.ErrorCode = errorCode;
		}
	}
}