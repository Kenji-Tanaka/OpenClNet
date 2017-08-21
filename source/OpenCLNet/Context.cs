using System;
using System.IO;

namespace OpenCLNet {
	public unsafe class Context : IDisposable, InteropTools.IPropertyContainer {
		// Track whether Dispose has been called.
		private Boolean disposed;
		public IntPtr ContextID { get; protected set; }

		/// <summary>
		///     True if there is at least one 64 bit device in the context.
		///     This guarantees that variables such as intptr_t, size_t etc are 64 bit
		/// </summary>
		public Boolean Is64BitContext { get; protected set; }
		public Platform Platform { get; protected set; }

		#region Casts
		public static implicit operator IntPtr(Context c) {
			return c.ContextID;
		}
		#endregion

		#region Create Sampler
		public Sampler CreateSampler(Boolean normalizedCoords, AddressingMode addressingMode, FilterMode filterMode) {
			IntPtr samplerID;
			ErrorCode result;

			samplerID = OpenCL.CreateSampler(this.ContextID, normalizedCoords, (UInt32)addressingMode, (UInt32)filterMode, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateSampler failed with error code " + result, result);
			return new Sampler(this, samplerID);
		}
		#endregion

		#region User Events
		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <returns></returns>
		public Event CreateUserEvent() {
			ErrorCode result;
			IntPtr eventID;

			eventID = OpenCL.CreateUserEvent(this.ContextID, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateUserEvent failed with error code " + result, result);
			return new Event(this, null, eventID);
		}
		#endregion

		#region Construction / Destruction
		internal Context(Platform platform, IntPtr contextID) {
			this.Platform = platform;
			this.ContextID = contextID;
			this.Is64BitContext = this.ContainsA64BitDevice();
		}

		internal Context(Platform platform, ContextProperties[] properties, Device[] devices) {
			IntPtr[] intPtrProperties;
			IntPtr[] deviceIDs;
			ErrorCode result;

			this.Platform = platform;
			deviceIDs = InteropTools.ConvertDevicesToDeviceIDs(devices);

			intPtrProperties = new IntPtr[properties.Length];
			for (var i = 0; i < properties.Length; i++)
				intPtrProperties[i] = new IntPtr((Int64)properties[i]);

			this.ContextID = OpenCL.CreateContext(intPtrProperties,
				(UInt32)devices.Length,
				deviceIDs,
				null,
				IntPtr.Zero,
				out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateContext failed: " + result, result);
			this.Is64BitContext = this.ContainsA64BitDevice();
		}

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~Context() {
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			this.Dispose(false);
		}

		protected Boolean ContainsA64BitDevice() {
			for (var i = 0; i < this.Devices.Length; i++)
				if (this.Devices[i].AddressBits == 64)
					return true;
			return false;
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
				OpenCL.ReleaseContext(this.ContextID);
				this.ContextID = IntPtr.Zero;

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		#region Properties
		public UInt32 ReferenceCount => InteropTools.ReadUInt(this, (UInt32)ContextInfo.REFERENCE_COUNT);

		public Device[] Devices {
			get {
				IntPtr contextDevicesSize;
				ErrorCode result;
				IntPtr[] contextDevices;

				result = OpenCL.GetContextInfo(this.ContextID, (UInt32)ContextInfo.DEVICES, IntPtr.Zero, null, out contextDevicesSize);
				if (result != ErrorCode.SUCCESS)
					throw new OpenCLException("Unable to get context info for context " + this.ContextID + " " + result, result);

				contextDevices = new IntPtr[contextDevicesSize.ToInt64() / sizeof(IntPtr)];
				fixed (IntPtr* pContextDevices = contextDevices) {
					result = OpenCL.GetContextInfo(this.ContextID, (UInt32)ContextInfo.DEVICES, contextDevicesSize, pContextDevices, out contextDevicesSize);
					if (result != ErrorCode.SUCCESS)
						throw new OpenCLException("Unable to get context info for context " + this.ContextID + " " + result, result);
				}
				return InteropTools.ConvertDeviceIDsToDevices(this.Platform, contextDevices);
			}
		}

		public ContextProperties[] Properties => throw new NotImplementedException();
		#endregion

		#region Create Command Queue
		public CommandQueue CreateCommandQueue(Device device) {
			return this.CreateCommandQueue(device, 0);
		}

		public CommandQueue CreateCommandQueue(Device device, CommandQueueProperties properties) {
			IntPtr commandQueueID;
			ErrorCode result;

			commandQueueID = OpenCL.CreateCommandQueue(this.ContextID, device.DeviceID, (UInt64)properties, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateCommandQueue failed with error code " + result, result);
			return new CommandQueue(this, device, commandQueueID);
		}
		#endregion

		#region Create Buffer
		public Mem CreateBuffer(MemFlags flags, Int64 size) {
			return this.CreateBuffer(flags, size, IntPtr.Zero);
		}

		public Mem CreateBuffer(MemFlags flags, Int64 size, IntPtr pHost) {
			return this.CreateBuffer(flags, size, pHost.ToPointer());
		}

		public Mem CreateBuffer(MemFlags flags, Int64 size, void* pHost) {
			IntPtr memID;
			ErrorCode result;

			memID = OpenCL.CreateBuffer(this.ContextID, (UInt64)flags, new IntPtr(size), pHost, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateBuffer failed with error code " + result, result);
			return new Mem(this, memID);
		}
		#endregion

		#region GL Interop
		public Mem CreateFromGLBuffer(MemFlags flags, IntPtr bufobj) {
			IntPtr memID;
			ErrorCode result;

			memID = OpenCL.CreateFromGLBuffer(this.ContextID, (UInt64)flags, (UInt32)bufobj, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateFromGLBuffer failed with error code " + result, result);
			return new Mem(this, memID);
		}

		public Mem CreateFromGLTexture2D(MemFlags flags, Int32 target, Int32 mipLevel, Int32 texture) {
			IntPtr memID;
			ErrorCode result;

			memID = OpenCL.CreateFromGLTexture2D(this.ContextID, (UInt64)flags, target, mipLevel, (UInt32)texture, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateFromGLTexture2D failed with error code " + result, result);
			return new Mem(this, memID);
		}

		public Mem CreateFromGLTexture3D(MemFlags flags, Int32 target, Int32 mipLevel, Int32 texture) {
			IntPtr memID;
			ErrorCode result;

			memID = OpenCL.CreateFromGLTexture3D(this.ContextID, (UInt64)flags, target, mipLevel, (UInt32)texture, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateFromGLTexture3D failed with error code " + result, result);
			return new Mem(this, memID);
		}

		public Mem CreateFromGLRenderbuffer(MemFlags flags, IntPtr renderbuffer) {
			IntPtr memID;
			ErrorCode result;

			memID = OpenCL.CreateFromGLRenderbuffer(this.ContextID, (UInt64)flags, (UInt32)renderbuffer, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateFromGLTexture3D failed with error code " + result, result);
			return new Mem(this, memID);
		}
		#endregion

		#region Create Program
		public Program CreateProgramFromFile(String path) {
			return this.CreateProgramWithSource(File.ReadAllText(path));
		}

		public Program CreateProgramWithSource(String source) {
			return this.CreateProgramWithSource(new[] { source });
		}

		public Program CreateProgramWithSource(String[] source) {
			IntPtr programID;
			ErrorCode result;

			programID = OpenCL.CreateProgramWithSource(this.ContextID, (UInt32)source.Length, source, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateProgramWithSource failed with error code " + result, result);
			return new Program(this, programID);
		}

		public Program CreateProgramWithBinary(Device[] devices, Byte[][] binaries, ErrorCode[] binaryStatus) {
			IntPtr programID;
			ErrorCode result;
			IntPtr[] lengths;
			var binStatus = new Int32[binaryStatus.Length];

			lengths = new IntPtr[devices.Length];
			for (var i = 0; i < lengths.Length; i++)
				lengths[i] = (IntPtr)binaries[i].Length;
			programID = OpenCL.CreateProgramWithBinary(this.ContextID,
				(UInt32)devices.Length,
				InteropTools.ConvertDevicesToDeviceIDs(devices),
				lengths,
				binaries,
				binStatus,
				out result);
			for (var i = 0; i < binaryStatus.Length; i++)
				binaryStatus[i] = (ErrorCode)binStatus[i];
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateProgramWithBinary failed with error code " + result, result);
			return new Program(this, programID);
		}
		#endregion

		#region Image2D
		public Image CreateImage2D(MemFlags flags, ImageFormat imageFormat, Int32 imageWidth, Int32 imageHeight) {
			return this.CreateImage2D(flags, imageFormat, (IntPtr)imageWidth, (IntPtr)imageHeight, IntPtr.Zero, IntPtr.Zero);
		}

		public Image CreateImage2D(MemFlags flags, ImageFormat imageFormat, Int64 imageWidth, Int64 imageHeight) {
			return this.CreateImage2D(flags, imageFormat, (IntPtr)imageWidth, (IntPtr)imageHeight, IntPtr.Zero, IntPtr.Zero);
		}

		public Image CreateImage2D(MemFlags flags, ImageFormat imageFormat, Int32 imageWidth, Int32 imageHeight, Int32 imageRowPitch, IntPtr pHost) {
			return this.CreateImage2D(flags, imageFormat, (IntPtr)imageWidth, (IntPtr)imageHeight, (IntPtr)imageRowPitch, pHost);
		}

		public Image CreateImage2D(MemFlags flags, ImageFormat imageFormat, Int64 imageWidth, Int64 imageHeight, Int64 imageRowPitch, IntPtr pHost) {
			return this.CreateImage2D(flags, imageFormat, (IntPtr)imageWidth, (IntPtr)imageHeight, (IntPtr)imageRowPitch, pHost);
		}

		public Image CreateImage2D(MemFlags flags, ImageFormat imageFormat, IntPtr imageWidth, IntPtr imageHeight, IntPtr imageRowPitch, IntPtr pHost) {
			IntPtr memID;
			ErrorCode result;

			memID = OpenCL.CreateImage2D(this.ContextID, (UInt64)flags, imageFormat, imageWidth, imageHeight, imageRowPitch, pHost.ToPointer(), out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateImage2D failed with error code " + result, result);
			return new Image(this, memID);
		}
		#endregion

		#region Image3D
		public Image CreateImage3D(MemFlags flags, ImageFormat imageFormat, Int32 imageWidth, Int32 imageHeight, Int32 imageDepth, Int32 imageRowPitch, Int32 imageSlicePitch) {
			return this.CreateImage3D(flags, imageFormat, (IntPtr)imageWidth, (IntPtr)imageHeight, (IntPtr)imageDepth, (IntPtr)imageRowPitch, (IntPtr)imageSlicePitch, IntPtr.Zero);
		}

		public Image CreateImage3D(MemFlags flags, ImageFormat imageFormat, Int32 imageWidth, Int32 imageHeight, Int32 imageDepth, Int32 imageRowPitch, Int32 imageSlicePitch, IntPtr pHost) {
			return this.CreateImage3D(flags, imageFormat, (IntPtr)imageWidth, (IntPtr)imageHeight, (IntPtr)imageDepth, (IntPtr)imageRowPitch, (IntPtr)imageSlicePitch, pHost);
		}

		public Image CreateImage3D(MemFlags flags, ImageFormat imageFormat, IntPtr imageWidth, IntPtr imageHeight, IntPtr imageDepth, IntPtr imageRowPitch, IntPtr imageSlicePitch, IntPtr pHost) {
			IntPtr memID;
			ErrorCode result;

			memID = OpenCL.CreateImage3D(this.ContextID, (UInt64)flags, imageFormat, imageWidth, imageHeight, imageDepth, imageRowPitch, imageSlicePitch, pHost.ToPointer(), out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateImage3D failed with error code " + result, result);
			return new Image(this, memID);
		}
		#endregion

		#region Image format queries
		/// <summary>
		///     Query which ImageFormats are supported by this context
		/// </summary>
		/// <param name="flags"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public ImageFormat[] GetSupportedImageFormats(MemFlags flags, MemObjectType type) {
			UInt32 numImageFormats;
			ImageFormat[] imageFormats;
			ErrorCode result;

			result = OpenCL.GetSupportedImageFormats(this.ContextID, (UInt64)flags, (UInt32)type, 0, null, out numImageFormats);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetSupportedImageFormats failed with error code " + result, result);

			imageFormats = new ImageFormat[numImageFormats];

			result = OpenCL.GetSupportedImageFormats(this.ContextID, (UInt64)flags, (UInt32)type, numImageFormats, imageFormats, out numImageFormats);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetSupportedImageFormats failed with error code " + result, result);

			return imageFormats;
		}

		/// <summary>
		///     Convenience function. Checks if a context supports a specific image format
		/// </summary>
		/// <param name="flags"></param>
		/// <param name="type"></param>
		/// <param name="channelOrder"></param>
		/// <param name="channelType"></param>
		/// <returns>true if the image format is supported, false otherwise</returns>
		public Boolean SupportsImageFormat(MemFlags flags, MemObjectType type, ChannelOrder channelOrder, ChannelType channelType) {
			var imageFormats = this.GetSupportedImageFormats(flags, type);
			foreach (var imageFormat in imageFormats) {
				if (imageFormat.ChannelOrder == channelOrder && imageFormat.ChannelType == channelType)
					return true;
			}
			return false;
		}
		#endregion

		#region WaitForEvents
		/// <summary>
		///     Block until the event fires
		/// </summary>
		/// <param name="_event"></param>
		public void WaitForEvent(Event _event) {
			var event_list = new Event[1];

			event_list[0] = _event;
			OpenCL.WaitForEvents(1, InteropTools.ConvertEventsToEventIDs(event_list));
		}

		/// <summary>
		///     Block until all events in the array have fired
		/// </summary>
		/// <param name="num_events"></param>
		/// <param name="event_list"></param>
		public void WaitForEvents(Int32 num_events, Event[] event_list) {
			OpenCL.WaitForEvents((UInt32)num_events, InteropTools.ConvertEventsToEventIDs(event_list));
		}
		#endregion

		#region HasExtension
		public Boolean HasExtension(String extension) {
			foreach (var d in this.Devices)
				if (!d.HasExtension(extension))
					return false;
			return true;
		}

		public Boolean HasExtensions(String[] extensions) {
			foreach (var d in this.Devices)
				if (!d.HasExtensions(extensions))
					return false;
			return true;
		}
		#endregion

		#region IPropertyContainer Members
		public IntPtr GetPropertySize(UInt32 key) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetContextInfo(this.ContextID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetContextInfo failed: " + result, result);
			return size;
		}

		public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetContextInfo(this.ContextID, key, keyLength, pBuffer, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetContextInfo failed: " + result, result);
		}
		#endregion
	}
}