using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	public class AlignedArray<T> where T : struct {
		// Track whether Dispose has been called.
		private Boolean disposed;
		protected Int64 AlignedArraySize;
		protected IntPtr AlignedMemory;
		protected Int64 ByteAlignment;
		protected Int32 TStride = Marshal.SizeOf(typeof(T));
		protected IntPtr UnmanagedMemory;
		public Int64 ByteLength { get; protected set; }

		public Int64 Length { get; protected set; }

		public AlignedArray(Int64 size, Int64 byteAlignment) {
			Int64 alignmentMask;

			this.Length = size;
			this.ByteLength = size * this.TStride;
			this.AlignedArraySize = size * this.TStride;
			this.ByteAlignment = byteAlignment;
			this.UnmanagedMemory = Marshal.AllocHGlobal(new IntPtr(this.AlignedArraySize + byteAlignment - 1));
			alignmentMask = this.ByteAlignment - 1;
			this.AlignedMemory = new IntPtr((this.UnmanagedMemory.ToInt64() + byteAlignment - 1) & ~alignmentMask);
		}

		~AlignedArray() {
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
				Marshal.FreeHGlobal(this.UnmanagedMemory);

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion
	}

	#region AlignedArrayByte
	#endregion

	#region AlignedArrayInt
	#endregion

	#region AlignedArrayLong
	#endregion

	#region AlignedArrayFloat
	#endregion
}