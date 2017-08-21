using System;

namespace OpenCLNet {
	/// <summary>
	///     Wrapper for an OpenCL sampler
	/// </summary>
	unsafe public class Sampler : IDisposable, InteropTools.IPropertyContainer {
		// Track whether Dispose has been called.
		private Boolean disposed;

		#region Properties
		public IntPtr SamplerID { get; protected set; }
		public Context Context { get; protected set; }

		public UInt32 ReferenceCount {
			get { return InteropTools.ReadUInt(this, (UInt32)SamplerInfo.REFERENCE_COUNT); }
		}

		public AddressingMode AddressingMode {
			get { return (AddressingMode)InteropTools.ReadUInt(this, (UInt32)SamplerInfo.ADDRESSING_MODE); }
		}

		public FilterMode FilterMode {
			get { return (FilterMode)InteropTools.ReadUInt(this, (UInt32)SamplerInfo.FILTER_MODE); }
		}

		public Boolean NormalizedCoords {
			get { return InteropTools.ReadBool(this, (UInt32)SamplerInfo.NORMALIZED_COORDS); }
		}
		#endregion

		#region Construction / Destruction
		internal Sampler(Context context, IntPtr samplerID) {
			this.Context = context;
			this.SamplerID = samplerID;
		}

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~Sampler() {
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			this.Dispose(false);
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
				OpenCL.ReleaseSampler(this.SamplerID);
				this.SamplerID = IntPtr.Zero;

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		#region IPropertyContainer Members
		public IntPtr GetPropertySize(UInt32 key) {
			ErrorCode result;
			IntPtr size;

			result = OpenCL.GetSamplerInfo(this.SamplerID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("clGetSamplerInfo failed with error code " + result, result);

			return size;
		}

		public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			ErrorCode result;
			IntPtr size;

			result = OpenCL.GetSamplerInfo(this.SamplerID, key, keyLength, pBuffer, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("clGetSamplerInfo failed with error code " + result, result);
		}
		#endregion
	}
}