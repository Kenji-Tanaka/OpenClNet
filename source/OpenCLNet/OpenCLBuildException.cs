using System;
using System.Collections.Generic;

namespace OpenCLNet {
	public class OpenCLBuildException : OpenCLException {
		public List<String> BuildLogs = new List<String>();

		public OpenCLBuildException(Program program, ErrorCode result)
			: base("Build failed with error code " + result, result) {
			foreach (var d in program.Devices) {
				this.BuildLogs.Add(program.GetBuildLog(d));
			}
		}
	}
}