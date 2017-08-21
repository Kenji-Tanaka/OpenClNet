using System;
using System.Collections.Generic;

namespace OpenCLNet {
	[Obsolete("SimpleOCLHelper is superseded by OpenCLManager")]
	public class SimpleOCLHelper {
		protected Dictionary<String, Kernel> Kernels;
		/// <summary>
		///     The Context associated with this Helper
		/// </summary>
		public Context Context;
		/// <summary>
		///     Alias for CQs[0]
		/// </summary>
		public CommandQueue CQ;
		/// <summary>
		///     CommandQueue for device with same index
		/// </summary>
		public CommandQueue[] CQs;
		/// <summary>
		///     The devices bound to the Helper
		/// </summary>
		public Device[] Devices;
		public Platform Platform;
		public Program Program;

		public SimpleOCLHelper(Platform platform, DeviceType deviceType, String source) {
			this.Platform = platform;
			this.Initialize(deviceType, source);
		}

		protected virtual void Initialize(DeviceType deviceType, String source) {
			this.Devices = this.Platform.QueryDevices(deviceType);
			if (this.Devices.Length == 0)
				throw new OpenCLException("No devices of type " + deviceType + " present");

			this.Context = this.Platform.CreateContext(null, this.Devices, null, IntPtr.Zero);
			this.CQs = new CommandQueue[this.Devices.Length];
			for (var i = 0; i < this.CQs.Length; i++)
				this.CQs[i] = this.Context.CreateCommandQueue(this.Devices[i], CommandQueueProperties.PROFILING_ENABLE);
			this.CQ = this.CQs[0];
			this.Program = this.Context.CreateProgramWithSource(source);
			this.Program.Build();
			this.Kernels = this.Program.CreateKernelDictionary();
		}

		public Kernel GetKernel(String kernelName) {
			return this.Kernels[kernelName];
		}
	}
}