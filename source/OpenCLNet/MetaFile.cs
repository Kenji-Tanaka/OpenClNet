using System;
using System.ComponentModel;

namespace OpenCLNet {
	[Serializable]
	public class MetaFile {
		[DefaultValue("")]
		public String BinaryName { get; set; }
		[DefaultValue("")]
		public String BuildOptions { get; set; }
		[DefaultValue("")]
		public String Defines { get; set; }
		[DefaultValue("")]
		public String Device { get; set; }
		[DefaultValue("")]
		public String DriverVersion { get; set; }
		[DefaultValue("")]
		public String Platform { get; set; }
		[DefaultValue("")]
		public String Source { get; set; }
		[DefaultValue("")]
		public String SourceName { get; set; }

		public MetaFile() {
			this.Source = "";
			this.SourceName = "";
			this.Platform = "";
			this.Device = "";
			this.DriverVersion = "";
			this.Defines = "";
			this.BuildOptions = "";
			this.BinaryName = "";
		}

		public MetaFile(String source, String sourceName, String platform, String device, String driverVersion, String defines, String buildOptions, String binaryName) {
			if (source != null)
				this.Source = source;
			else
				this.Source = "";

			if (sourceName != null)
				this.SourceName = sourceName;
			else
				this.SourceName = "";

			if (platform != null)
				this.Platform = platform;
			else
				this.Platform = "";

			if (device != null)
				this.Device = device;
			else
				this.Device = "";

			if (driverVersion != null)
				this.DriverVersion = driverVersion;
			else
				this.DriverVersion = "";

			if (defines != null)
				this.Defines = defines;
			else
				this.Defines = "";

			if (buildOptions != null)
				this.BuildOptions = buildOptions;
			else
				this.BuildOptions = "";

			if (binaryName != null)
				this.BinaryName = binaryName;
			else
				this.BinaryName = "";
		}
	}
}