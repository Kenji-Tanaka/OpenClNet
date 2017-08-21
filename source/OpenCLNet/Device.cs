using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenCLNet {
	public unsafe class Device : IDisposable, InteropTools.IPropertyContainer {
		// Track whether Dispose has been called.
		private Boolean disposed;
		private Boolean IsSubDevice;
		protected HashSet<String> ExtensionHashSet = new HashSet<String>();

		internal Device(Platform platform, IntPtr deviceID) {
			this.Platform = platform;
			this.DeviceID = deviceID;

			this.InitializeExtensionHashSet();
		}

		public static implicit operator IntPtr(Device d) {
			return d.DeviceID;
		}

		#region ToString
		public override String ToString() {
			var sb = new StringBuilder();

			sb.AppendLine("Name: " + this.Name);
			sb.AppendLine("Vendor: " + this.Vendor);
			sb.AppendLine("VendorID: " + this.VendorID);
			sb.AppendLine("DriverVersion: " + this.DriverVersion);
			sb.AppendLine("Profile: " + this.Profile);
			sb.AppendLine("Version: " + this.Version);
			sb.AppendLine("Extensions: " + this.Extensions);
			sb.AppendLine("DeviceType: " + this.DeviceType);
			sb.AppendLine("MaxComputeUnits: " + this.MaxComputeUnits);
			sb.AppendLine("MaxWorkItemDimensions: " + this.MaxWorkItemDimensions);
			sb.Append("MaxWorkItemSizes:");
			for (var i = 0; i < this.MaxWorkItemSizes.Length; i++)
				sb.Append(" " + i + "=" + (Int32)this.MaxWorkItemSizes[i]);
			sb.AppendLine("");
			sb.AppendLine("MaxWorkGroupSize: " + this.MaxWorkGroupSize);
			sb.AppendLine("PreferredVectorWidthChar: " + this.PreferredVectorWidthChar);
			sb.AppendLine("PreferredVectorWidthShort: " + this.PreferredVectorWidthShort);
			sb.AppendLine("PreferredVectorWidthInt: " + this.PreferredVectorWidthInt);
			sb.AppendLine("PreferredVectorWidthLong: " + this.PreferredVectorWidthLong);
			sb.AppendLine("PreferredVectorWidthFloat: " + this.PreferredVectorWidthFloat);
			sb.AppendLine("PreferredVectorWidthDouble: " + this.PreferredVectorWidthDouble);
			sb.AppendLine("NativeVectorWidthChar: " + this.NativeVectorWidthChar);
			sb.AppendLine("NativeVectorWidthShort: " + this.NativeVectorWidthShort);
			sb.AppendLine("NativeVectorWidthInt: " + this.NativeVectorWidthInt);
			sb.AppendLine("NativeVectorWidthLong: " + this.NativeVectorWidthLong);
			sb.AppendLine("NativeVectorWidthFloat: " + this.NativeVectorWidthFloat);
			sb.AppendLine("NativeVectorWidthDouble: " + this.NativeVectorWidthDouble);
			sb.AppendLine("MaxClockFrequency: " + this.MaxClockFrequency);
			sb.AppendLine("AddressBits: " + this.AddressBits);
			sb.AppendLine("MaxMemAllocSize: " + this.MaxMemAllocSize);
			sb.AppendLine("ImageSupport: " + this.ImageSupport);
			sb.AppendLine("MaxReadImageArgs: " + this.MaxReadImageArgs);
			sb.AppendLine("MaxWriteImageArgs: " + this.MaxWriteImageArgs);
			sb.AppendLine("Image2DMaxWidth: " + this.Image2DMaxWidth);
			sb.AppendLine("Image2DMaxHeight: " + this.Image2DMaxHeight);
			sb.AppendLine("Image3DMaxWidth: " + this.Image3DMaxWidth);
			sb.AppendLine("Image3DMaxHeight: " + this.Image3DMaxHeight);
			sb.AppendLine("Image3DMaxDepth: " + this.Image3DMaxDepth);
			sb.AppendLine("MaxSamplers: " + this.MaxSamplers);
			sb.AppendLine("MaxParameterSize: " + this.MaxParameterSize);
			sb.AppendLine("MemBaseAddrAlign: " + this.MemBaseAddrAlign);
			sb.AppendLine("MinDataTypeAlignSize: " + this.MinDataTypeAlignSize);
			sb.AppendLine("SingleFPConfig: " + this.SingleFPConfig);
			sb.AppendLine("GlobalMemCacheType: " + this.GlobalMemCacheType);
			sb.AppendLine("GlobalMemCacheLineSize: " + this.GlobalMemCacheLineSize);
			sb.AppendLine("GlobalMemCacheSize: " + this.GlobalMemCacheSize);
			sb.AppendLine("GlobalMemSize: " + this.GlobalMemSize);
			sb.AppendLine("MaxConstantBufferSize: " + this.MaxConstantBufferSize);
			sb.AppendLine("MaxConstantArgs: " + this.MaxConstantArgs);
			sb.AppendLine("LocalMemType: " + this.LocalMemType);
			sb.AppendLine("LocalMemSize: " + this.LocalMemSize);
			sb.AppendLine("ErrorCorrectionSupport: " + this.ErrorCorrectionSupport);
			sb.AppendLine("ProfilingTimerResolution: " + this.ProfilingTimerResolution);
			sb.AppendLine("EndianLittle: " + this.EndianLittle);
			sb.AppendLine("Available: " + this.Available);
			sb.AppendLine("CompilerAvailable: " + this.CompilerAvailable);
			sb.AppendLine("ExecutionCapabilities: " + this.ExecutionCapabilities);
			sb.AppendLine("QueueProperties: " + this.QueueProperties);
			return sb.ToString();
		}
		#endregion

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~Device() {
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			this.Dispose(false);
		}

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
				if (this.IsSubDevice)
					OpenCL.ReleaseDeviceEXT(this.DeviceID);

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		#region Device Fission API (Extension)
		public void ReleaseDeviceEXT() {
			ErrorCode result;

			result = OpenCL.ReleaseDeviceEXT(this.DeviceID);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("ReleaseDeviceEXT failed with error code: " + result, result);
		}

		public void RetainDeviceEXT() {
			ErrorCode result;

			result = OpenCL.RetainDeviceEXT(this.DeviceID);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("RetainDeviceEXT failed with error code: " + result, result);
		}

		/// <summary>
		///     CreateSubDevicesEXT uses a slightly modified API,
		///     due to the overall messiness of creating a
		///     cl_device_partition_property_ext in managed C#.
		///     The object list properties is a linear list of partition properties and arguments
		///     add the DevicePartition property IDs  and ListTerminators as ulongs and the argument lists as ints
		///     CreateSubDevicesEXT will use that info to construct a binary block
		/// </summary>
		/// <param name="properties"></param>
		public Device[] CreateSubDevicesEXT(List<Object> properties) {
			ErrorCode result;
			var ms = new MemoryStream();
			var bw = new BinaryWriter(ms);

			for (var i = 0; i < properties.Count; i++) {
				if (properties[i] is UInt64)
					bw.Write((UInt64)properties[i]);
				else if (properties[i] is Int32)
					bw.Write((Int32)properties[i]);
				else
					throw new ArgumentException("CreateSubDevicesEXT: property lists only accepts ulongs and ints");
			}
			bw.Flush();
			var propertyArray = ms.ToArray();
			UInt32 numDevices;
			result = OpenCL.CreateSubDevicesEXT(this.DeviceID, propertyArray, 0, null, &numDevices);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateSubDevicesEXT failed with error code: " + result, result);

			var subDeviceIDs = new IntPtr[(Int32)numDevices];
			result = OpenCL.CreateSubDevicesEXT(this.DeviceID, propertyArray, numDevices, subDeviceIDs, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateSubDevicesEXT failed with error code: " + result, result);

			var subDevices = new Device[(Int32)numDevices];
			for (var i = 0; i < (Int32)numDevices; i++) {
				var d = new Device(this.Platform, subDeviceIDs[i]);
				d.IsSubDevice = true;
				subDevices[i] = d;
			}
			return subDevices;
		}
		#endregion

		#region Properties
		public IntPtr DeviceID { get; protected set; }

		public DeviceType DeviceType => (DeviceType)InteropTools.ReadULong(this, (UInt32)DeviceInfo.TYPE);

		/// <summary>
		///     A unique device vendor identifier. An example of a unique device identifier could be the PCIe ID.
		/// </summary>
		public UInt32 VendorID => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.VENDOR_ID);

		/// <summary>
		///     The number of parallel compute cores on the OpenCL device. The minimum value is 1.
		/// </summary>
		public UInt32 MaxComputeUnits => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MAX_COMPUTE_UNITS);

		/// <summary>
		///     Maximum dimensions that specify the global and local work-item IDs used by the data parallel execution model.
		///     (Refer to clEnqueueNDRangeKernel). The minimum value is 3.
		/// </summary>
		public UInt32 MaxWorkItemDimensions => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MAX_WORK_ITEM_DIMENSIONS);

		/// <summary>
		///     Maximum number of work-items that can be specified in each dimension of
		///     the work-group to clEnqueueNDRangeKernel.
		///     Returns n size_t entries, where n is the value returned by the query for
		///     CL_DEVICE_MAX_WORK_ITEM_DIMENSIONS.
		///     The minimum value is (1, 1, 1).
		/// </summary>
		public IntPtr[] MaxWorkItemSizes => InteropTools.ReadIntPtrArray(this, (UInt32)DeviceInfo.MAX_WORK_ITEM_SIZES);

		/// <summary>
		///     Maximum number of work-items in a work-group executing a kernel using the data parallel execution model.
		///     (Refer to clEnqueueNDRangeKernel).
		///     The minimum value is 1.
		/// </summary>
		public Int64 MaxWorkGroupSize => InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.MAX_WORK_GROUP_SIZE).ToInt64();

		public UInt32 PreferredVectorWidthChar => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.PREFERRED_VECTOR_WIDTH_CHAR);

		public UInt32 PreferredVectorWidthShort => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.PREFERRED_VECTOR_WIDTH_SHORT);

		public UInt32 PreferredVectorWidthInt => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.PREFERRED_VECTOR_WIDTH_INT);

		public UInt32 PreferredVectorWidthLong => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.PREFERRED_VECTOR_WIDTH_LONG);

		public UInt32 PreferredVectorWidthFloat => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.PREFERRED_VECTOR_WIDTH_FLOAT);

		public UInt32 PreferredVectorWidthDouble => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.PREFERRED_VECTOR_WIDTH_DOUBLE);

		public UInt32 PreferredVectorWidthHalf => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.PREFERRED_VECTOR_WIDTH_HALF);

		public UInt32 NativeVectorWidthChar => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.NATIVE_VECTOR_WIDTH_CHAR);

		public UInt32 NativeVectorWidthShort => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.NATIVE_VECTOR_WIDTH_SHORT);

		public UInt32 NativeVectorWidthInt => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.NATIVE_VECTOR_WIDTH_INT);

		public UInt32 NativeVectorWidthLong => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.NATIVE_VECTOR_WIDTH_LONG);

		public UInt32 NativeVectorWidthFloat => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.NATIVE_VECTOR_WIDTH_FLOAT);

		public UInt32 NativeVectorWidthDouble => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.NATIVE_VECTOR_WIDTH_DOUBLE);

		public UInt32 NativeVectorWidthHalf => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.NATIVE_VECTOR_WIDTH_HALF);

		/// <summary>
		///     Maximum configured clock frequency of the device in MHz.
		/// </summary>
		public UInt32 MaxClockFrequency => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MAX_CLOCK_FREQUENCY);

		/// <summary>
		///     The default compute device address space size specified as an unsigned
		///     integer value in bits. Currently supported values are 32 or 64 bits.
		/// </summary>
		public UInt32 AddressBits => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.ADDRESS_BITS);

		/// <summary>
		///     Max size of memory object allocation in bytes. The minimum value is max
		///     (1/4th of CL_DEVICE_GLOBAL_MEM_SIZE, 128*1024*1024)
		/// </summary>
		public UInt64 MaxMemAllocSize => InteropTools.ReadULong(this, (UInt32)DeviceInfo.MAX_MEM_ALLOC_SIZE);

		/// <summary>
		///     Is true if images are supported by the OpenCL device and CL_FALSE otherwise.
		/// </summary>
		public Boolean ImageSupport => InteropTools.ReadBool(this, (UInt32)DeviceInfo.IMAGE_SUPPORT);

		/// <summary>
		///     Max number of simultaneous image objects that can be read by a kernel.
		///     The minimum value is 128 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public UInt32 MaxReadImageArgs => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MAX_READ_IMAGE_ARGS);

		/// <summary>
		///     Max number of simultaneous image objects that can be written to by a
		///     kernel. The minimum value is 8 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public UInt32 MaxWriteImageArgs => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MAX_WRITE_IMAGE_ARGS);

		/// <summary>
		///     Max width of 2D image in pixels. The minimum value is 8192 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public Int64 Image2DMaxWidth => InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.IMAGE2D_MAX_WIDTH).ToInt64();

		/// <summary>
		///     Max height of 2D image in pixels. The minimum value is 8192 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public Int64 Image2DMaxHeight => InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.IMAGE2D_MAX_HEIGHT).ToInt64();

		/// <summary>
		///     Max width of 3D image in pixels. The minimum value is 2048 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public Int64 Image3DMaxWidth => InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.IMAGE3D_MAX_WIDTH).ToInt64();

		/// <summary>
		///     Max height of 3D image in pixels. The minimum value is 2048 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public Int64 Image3DMaxHeight => InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.IMAGE3D_MAX_HEIGHT).ToInt64();

		/// <summary>
		///     Max depth of 3D image in pixels. The minimum value is 2048 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public Int64 Image3DMaxDepth => InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.IMAGE3D_MAX_DEPTH).ToInt64();

		/// <summary>
		///     Maximum number of samplers that can be used in a kernel. Refer to section 6.11.8 for a detailed
		///     description on samplers. The minimum value is 16 if CL_DEVICE_IMAGE_SUPPORT is true.
		/// </summary>
		public UInt32 MaxSamplers => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MAX_SAMPLERS);

		/// <summary>
		///     Max size in bytes of the arguments that can be passed to a kernel. The minimum value is 256.
		/// </summary>
		public Int64 MaxParameterSize => InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.MAX_PARAMETER_SIZE).ToInt64();

		/// <summary>
		///     Describes the alignment in bits of the base address of any allocated memory object.
		/// </summary>
		public UInt32 MemBaseAddrAlign => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MEM_BASE_ADDR_ALIGN);

		/// <summary>
		///     The smallest alignment in bytes which can be used for any data type.
		/// </summary>
		public UInt32 MinDataTypeAlignSize => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MIN_DATA_TYPE_ALIGN_SIZE);

		public UInt64 SingleFPConfig => InteropTools.ReadULong(this, (UInt32)DeviceInfo.SINGLE_FP_CONFIG);

		/// <summary>
		///     Type of global memory cache supported. Valid values are: CL_NONE, CL_READ_ONLY_CACHE and CL_READ_WRITE_CACHE.
		/// </summary>
		public DeviceMemCacheType GlobalMemCacheType => (DeviceMemCacheType)InteropTools.ReadUInt(this, (UInt32)DeviceInfo.GLOBAL_MEM_CACHE_TYPE);

		/// <summary>
		///     Size of global memory cache line in bytes.
		/// </summary>
		public UInt32 GlobalMemCacheLineSize => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.GLOBAL_MEM_CACHELINE_SIZE);

		/// <summary>
		///     Size of global memory cache in bytes.
		/// </summary>
		public UInt64 GlobalMemCacheSize => InteropTools.ReadULong(this, (UInt32)DeviceInfo.GLOBAL_MEM_CACHE_SIZE);

		/// <summary>
		///     Size of global device memory in bytes.
		/// </summary>
		public UInt64 GlobalMemSize => InteropTools.ReadULong(this, (UInt32)DeviceInfo.GLOBAL_MEM_SIZE);

		/// <summary>
		///     Max size in bytes of a constant buffer allocation. The minimum value is 64 KB.
		/// </summary>
		public UInt64 MaxConstantBufferSize => InteropTools.ReadULong(this, (UInt32)DeviceInfo.MAX_CONSTANT_BUFFER_SIZE);

		/// <summary>
		///     Max number of arguments declared with the __constant qualifier in a kernel. The minimum value is 8.
		/// </summary>
		public UInt32 MaxConstantArgs => InteropTools.ReadUInt(this, (UInt32)DeviceInfo.MAX_CONSTANT_ARGS);

		/// <summary>
		///     Type of local memory supported. This can be set to CL_LOCAL implying dedicated local memory storage such as SRAM, or CL_GLOBAL.
		/// </summary>
		public DeviceLocalMemType LocalMemType => (DeviceLocalMemType)InteropTools.ReadUInt(this, (UInt32)DeviceInfo.LOCAL_MEM_TYPE);

		/// <summary>
		///     Size of local memory arena in bytes. The minimum value is 16 KB.
		/// </summary>
		public UInt64 LocalMemSize => InteropTools.ReadULong(this, (UInt32)DeviceInfo.LOCAL_MEM_SIZE);

		/// <summary>
		///     Is CL_TRUE if the device implements error correction for the memories,
		///     caches, registers etc. in the device. Is CL_FALSE if the device does not
		///     implement error correction. This can be a requirement for certain clients of OpenCL.
		/// </summary>
		public Boolean ErrorCorrectionSupport => InteropTools.ReadBool(this, (UInt32)DeviceInfo.ERROR_CORRECTION_SUPPORT);

		/// <summary>
		///     Is CL_TRUE if the device and the host have a unified memory subsystem
		///     and is CL_FALSE otherwise.
		/// </summary>
		public Boolean HostUnifiedMemory => InteropTools.ReadBool(this, (UInt32)DeviceInfo.HOST_UNIFIED_MEMORY);

		/// <summary>
		///     Describes the resolution of device timer. This is measured in nanoseconds. Refer to section 5.9 for details.
		/// </summary>
		public UInt64 ProfilingTimerResolution => (UInt64)InteropTools.ReadIntPtr(this, (UInt32)DeviceInfo.PROFILING_TIMER_RESOLUTION).ToInt64();

		/// <summary>
		///     Is CL_TRUE if the OpenCL device is a little endian device and CL_FALSE otherwise.
		/// </summary>
		public Boolean EndianLittle => InteropTools.ReadBool(this, (UInt32)DeviceInfo.ENDIAN_LITTLE);

		/// <summary>
		///     Is CL_TRUE if the device is available and CL_FALSE if the device is not available.
		/// </summary>
		public Boolean Available => InteropTools.ReadBool(this, (UInt32)DeviceInfo.AVAILABLE);

		/// <summary>
		///     Is CL_FALSE if the implementation does not have a compiler available to compile the program source.
		///     Is CL_TRUE if the compiler is available.
		///     This can be CL_FALSE for the embededed platform profile only.
		/// </summary>
		public Boolean CompilerAvailable => InteropTools.ReadBool(this, (UInt32)DeviceInfo.COMPILER_AVAILABLE);

		/// <summary>
		///     Describes the execution capabilities of the device. This is a bit-field that describes one or more of the following values:
		///     CL_EXEC_KERNEL – The OpenCL device can execute OpenCL kernels.
		///     CL_EXEC_NATIVE_KERNEL – The OpenCL device can execute native kernels.
		///     The mandated minimum capability is: CL_EXEC_KERNEL.
		/// </summary>
		public UInt64 ExecutionCapabilities => InteropTools.ReadULong(this, (UInt32)DeviceInfo.EXECUTION_CAPABILITIES);

		/// <summary>
		///     Describes the command-queue properties supported by the device.
		///     This is a bit-field that describes one or more of the following values:
		///     CL_QUEUE_OUT_OF_ORDER_EXEC_MODE_ENABLE
		///     CL_QUEUE_PROFILING_ENABLE
		///     These properties are described in table 5.1.
		///     The mandated minimum capability is:
		///     CL_QUEUE_PROFILING_ENABLE.
		/// </summary>
		public UInt64 QueueProperties => InteropTools.ReadULong(this, (UInt32)DeviceInfo.QUEUE_PROPERTIES);

		/// <summary>
		///     The platform associated with this device.
		/// </summary>
		public Platform Platform { get; protected set; }

		/// <summary>
		///     Device name string.
		/// </summary>
		public String Name => InteropTools.ReadString(this, (UInt32)DeviceInfo.NAME);

		/// <summary>
		///     Vendor name string.
		/// </summary>
		public String Vendor => InteropTools.ReadString(this, (UInt32)DeviceInfo.VENDOR);

		/// <summary>
		///     OpenCL software driver version string in the form major_number.minor_number.
		/// </summary>
		public String DriverVersion => InteropTools.ReadString(this, (UInt32)DeviceInfo.DRIVER_VERSION);

		/// <summary>
		///     OpenCL profile string. Returns the profile name supported by the device.
		///     The profile name returned can be one of the following strings:
		///     FULL_PROFILE – if the device supports the OpenCL specification (functionality defined as part of the
		///     core specification and does not require any extensions to be supported).
		///     EMBEDDED_PROFILE - if the device supports the OpenCL embedded profile.
		/// </summary>
		public String Profile => InteropTools.ReadString(this, (UInt32)DeviceInfo.PROFILE);

		/// <summary>
		///     OpenCL version string. Returns the OpenCL version supported by the device. This version string has the
		///     following format:
		///     OpenCL&lt;space&gt;&lt;major_version.minor_version&gt;&lt;space&gt;&lt;vendor-specificinformation&gt;
		/// </summary>
		public String Version => InteropTools.ReadString(this, (UInt32)DeviceInfo.VERSION);

		/// <summary>
		///     OpenCL C version string. Returns the highest OpenCL C version supported
		///     by the compiler for this device. This version string has the following format:
		///     OpenCL&lt;space&gt;C&lt;space&gt;&lt;major_version.minor_version&gt;&lt;space&gt;&lt;vendor-specific information&gt;
		/// </summary>
		public String OpenCL_C_Version => InteropTools.ReadString(this, (UInt32)DeviceInfo.OPENCL_C_VERSION);

		/// <summary>
		///     Returns a space separated list of extension names
		///     (the extension names themselves do not contain any spaces).
		///     The list of extension names returned currently can include one or more of
		///     the following approved extension names:
		///     cl_khr_fp64
		///     cl_khr_select_fprounding_mode
		///     cl_khr_global_int32_base_atomics
		///     cl_khr_global_int32_extended_atomics
		///     cl_khr_local_int32_base_atomics
		///     cl_khr_local_int32_extended_atomics
		///     cl_khr_int64_base_atomics
		///     cl_khr_int64_extended_atomics
		///     cl_khr_3d_image_writes
		///     cl_khr_byte_addressable_store
		///     cl_khr_fp16
		///     cl_khr_gl_sharing
		///     Please refer to the OpenCL specification for a detailed
		///     description of these extensions.
		/// </summary>
		public String Extensions => InteropTools.ReadString(this, (UInt32)DeviceInfo.EXTENSIONS);
		#endregion

		#region IPropertyContainer Members
		public IntPtr GetPropertySize(UInt32 key) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetDeviceInfo(this.DeviceID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("Unable to get device info for device " + this.DeviceID, result);
			return size;
		}

		public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetDeviceInfo(this.DeviceID, key, keyLength, pBuffer, out size);
			if (result != (Int32)ErrorCode.SUCCESS)
				throw new OpenCLException("Unable to get device info for device " + this.DeviceID, result);
		}
		#endregion

		#region HasExtension
		protected void InitializeExtensionHashSet() {
			var ext = this.Extensions.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var s in ext)
				this.ExtensionHashSet.Add(s);
		}

		public Boolean HasExtension(String extension) {
			return this.ExtensionHashSet.Contains(extension);
		}

		public Boolean HasExtensions(String[] extensions) {
			foreach (var s in extensions)
				if (!this.ExtensionHashSet.Contains(s))
					return false;
			return true;
		}
		#endregion
	}
}