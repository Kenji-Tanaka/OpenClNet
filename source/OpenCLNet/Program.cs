using System;
using System.Collections.Generic;

namespace OpenCLNet {
	/// <summary>
	///     Wrapper for an OpenCL Program
	/// </summary>
	public unsafe class Program : IDisposable, InteropTools.IPropertyContainer {
		// Track whether Dispose has been called.
		private Boolean disposed;

		internal Program(Context context, IntPtr programID) {
			this.Context = context;
			this.ProgramID = programID;
		}

		public static implicit operator IntPtr(Program p) {
			return p.ProgramID;
		}

		public void Build() {
			this.Build(null, null, IntPtr.Zero);
		}

		public void Build(Device[] devices, ProgramNotify notify, IntPtr userData) {
			this.Build(devices, null, notify, userData);
		}

		public void Build(Device[] devices, String options, ProgramNotify notify, IntPtr userData) {
			var deviceLength = 0;

			if (devices != null)
				deviceLength = devices.Length;

			var deviceIDs = InteropTools.ConvertDevicesToDeviceIDs(devices);
			var result = OpenCL.BuildProgram(this.ProgramID,
				(UInt32)deviceLength,
				deviceIDs,
				options,
				notify,
				userData);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLBuildException(this, result);
		}

		public Kernel CreateKernel(String kernelName) {
			IntPtr kernelID;
			ErrorCode result;

			kernelID = OpenCL.CreateKernel(this.ProgramID, kernelName, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateKernel failed with error code " + result, result);
			return new Kernel(this.Context, this, kernelID);
		}

		/// <summary>
		///     Create all kernels in the program and return them as a Dictionary.
		///     Its keys are the kernel names, its values are the kernels themselves.
		/// </summary>
		/// <returns></returns>
		public Dictionary<String, Kernel> CreateKernelDictionary() {
			var kernels = this.CreateKernels();
			var kernelDictionary = new Dictionary<String, Kernel>();

			foreach (var k in kernels)
				kernelDictionary[k.FunctionName] = k;

			return kernelDictionary;
		}

		/// <summary>
		///     Create all kernels in the program and return them as an array
		/// </summary>
		/// <returns></returns>
		public Kernel[] CreateKernels() {
			UInt32 numKernels;

			var result = OpenCL.CreateKernelsInProgram(this.ProgramID, 0, null, out numKernels);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateKernels failed with error code " + result, result);

			var kernelIDs = new IntPtr[numKernels];
			result = OpenCL.CreateKernelsInProgram(this.ProgramID, numKernels, kernelIDs, out numKernels);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateKernels failed with error code " + result, result);

			var kernels = new Kernel[numKernels];
			for (var i = 0; i < kernels.Length; i++)
				kernels[i] = new Kernel(this.Context, this, kernelIDs[i]);
			return kernels;
		}

		public String GetBuildLog(Device device) {
			var buildInfo = new BuildInfo(this, device);
			return InteropTools.ReadString(buildInfo, (UInt32)ProgramBuildInfo.LOG);
		}

		public String GetBuildOptions(Device device) {
			var buildInfo = new BuildInfo(this, device);
			return InteropTools.ReadString(buildInfo, (UInt32)ProgramBuildInfo.OPTIONS);
		}

		public BuildStatus GetBuildStatus(Device device) {
			var buildInfo = new BuildInfo(this, device);
			return (BuildStatus)InteropTools.ReadInt(buildInfo, (UInt32)ProgramBuildInfo.STATUS);
		}

		~Program() {
			this.Dispose(false);
		}

		sealed class BuildInfo : InteropTools.IPropertyContainer {
			readonly Device Device;
			readonly Program Program;

			public BuildInfo(Program p, Device d) {
				this.Program = p;
				this.Device = d;
			}

			#region IPropertyContainer Members
			public IntPtr GetPropertySize(UInt32 key) {
				ErrorCode result;
				IntPtr size;

				result = OpenCL.GetProgramBuildInfo(this.Program.ProgramID, this.Device.DeviceID, key, IntPtr.Zero, null, out size);
				if (result != ErrorCode.SUCCESS)
					throw new OpenCLException("clGetProgramBuildInfo failed with error code " + result, result);

				return size;
			}

			public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
				ErrorCode result;
				IntPtr size;

				result = OpenCL.GetProgramBuildInfo(this.Program.ProgramID, this.Device.DeviceID, key, keyLength, pBuffer, out size);
				if (result != ErrorCode.SUCCESS)
					throw new OpenCLException("clGetProgramBuildInfo failed with error code " + result, result);
			}
			#endregion
		}

		#region Properties
		public Context Context { get; protected set; }
		public IntPtr ProgramID { get; protected set; }

		public String Source => InteropTools.ReadString(this, (UInt32)ProgramInfo.SOURCE);

		public UInt32 ReferenceCount => InteropTools.ReadUInt(this, (UInt32)ProgramInfo.REFERENCE_COUNT);

		public UInt32 NumDevices => InteropTools.ReadUInt(this, (UInt32)ProgramInfo.NUM_DEVICES);

		public Device[] Devices {
			get {
				var numDevices = this.NumDevices;
				if (numDevices == 0)
					return null;

				var data = InteropTools.ReadBytes(this, (UInt32)ProgramInfo.DEVICES);
				var deviceIDs = new IntPtr[numDevices];
				fixed (Byte* pData = data) {
					var pBS = (void**)pData;
					for (var i = 0; i < numDevices; i++)
						deviceIDs[i] = new IntPtr(pBS[i]);
				}
				return InteropTools.ConvertDeviceIDsToDevices(this.Context.Platform, deviceIDs);
			}
		}

		public IntPtr[] BinarySizes {
			get {
				var numDevices = this.NumDevices;
				if (numDevices == 0)
					return null;

				var data = InteropTools.ReadBytes(this, (UInt32)ProgramInfo.BINARY_SIZES);
				var binarySizes = new IntPtr[numDevices];
				fixed (Byte* pData = data) {
					var pBS = (void**)pData;
					for (var i = 0; i < numDevices; i++)
						binarySizes[i] = new IntPtr(pBS[i]);
				}
				return binarySizes;
			}
		}

		public Byte[][] Binaries {
			get {
				var numDevices = this.NumDevices;
				if (numDevices == 0)
					return null;

				var binarySizes = this.BinarySizes;
				var binaries = new Byte[numDevices][];
				for (var i = 0; i < numDevices; i++)
					binaries[i] = new Byte[binarySizes[i].ToInt64()];

				InteropTools.ReadPreAllocatedBytePtrArray(this, (UInt32)ProgramInfo.BINARIES, binaries);
				return binaries;
			}
		}
		#endregion

		#region IDisposable Members
		// Implement IDisposable.
		// Do not make this method virtual.
		// A derived class should not be able to override this method.
		public void Dispose() {
			this.Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		// Dispose(bool disposing) executes in two distinct scenarios.
		// If disposing equals true, the method has been called directly
		// or indirectly by a user's code. Managed and unmanaged resources
		// can be disposed.
		// If disposing equals false, the method has been called by the
		// runtime from inside the finalizer and you should not reference
		// other objects. Only unmanaged resources can be disposed.
		private void Dispose(Boolean disposing) {
			// Check to see if Dispose has already been called.
			if (!this.disposed) {
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				if (disposing) {
					// Dispose managed resources.
				}

				// Call the appropriate methods to clean up
				// unmanaged resources here.
				// If disposing is false,
				// only the following code is executed.
				var result = OpenCL.ReleaseProgram(this.ProgramID);
				if (result != ErrorCode.SUCCESS)
					throw new OpenCLException("ReleaseProgram failed: " + result, result);

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		#region IPropertyContainer Members
		public IntPtr GetPropertySize(UInt32 key) {
			ErrorCode result;
			IntPtr size;

			result = OpenCL.GetProgramInfo(this.ProgramID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("clGetProgramInfo failed with error code " + result, result);

			return size;
		}

		public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			ErrorCode result;
			IntPtr size;

			result = OpenCL.GetProgramInfo(this.ProgramID, key, keyLength, pBuffer, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("clGetProgramInfo failed with error code " + result, result);
		}
		#endregion
	}
}