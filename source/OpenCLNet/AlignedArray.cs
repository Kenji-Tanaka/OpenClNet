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
	/// <summary>
	///     Aligned 1D array class for bytes
	/// </summary>
	unsafe public class AlignedArrayByte : AlignedArray<Byte> {
		readonly Byte* pAlignedArray;

		public Byte this[Int64 index] {
			get {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				return this.pAlignedArray[index];
			}
			set {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				this.pAlignedArray[index] = value;
			}
		}

		public AlignedArrayByte(Int64 size, Int64 byteAlignment)
			: base(size, byteAlignment) {
			this.pAlignedArray = (Byte*)this.AlignedMemory.ToPointer();
		}

		public static implicit operator IntPtr(AlignedArrayByte array) {
			return new IntPtr(array.pAlignedArray);
		}

		public void Extract(Int64 index, Byte[] destinationArray, Int64 destinationIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				destinationArray[destinationIndex + i] = this.pAlignedArray[index + i];
		}

		public IntPtr GetPtr(Int64 index) {
			if (index >= this.Length || index < 0)
				throw new IndexOutOfRangeException();

			return new IntPtr(this.pAlignedArray + index);
		}

		public void Insert(Int64 index, Byte[] sourceArray, Int64 sourceIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				this.pAlignedArray[index + i] = sourceArray[sourceIndex + i];
		}
	}
	#endregion

	#region AlignedArrayInt
	/// <summary>
	///     Aligned 1D array class for ints
	/// </summary>
	unsafe public class AlignedArrayInt : AlignedArray<Int32> {
		readonly Int32* pAlignedArray;

		public Int32 this[Int64 index] {
			get {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				return this.pAlignedArray[index];
			}
			set {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				this.pAlignedArray[index] = value;
			}
		}

		public AlignedArrayInt(Int64 size, Int64 byteAlignment)
			: base(size, byteAlignment) {
			this.pAlignedArray = (Int32*)this.AlignedMemory.ToPointer();
		}

		public static implicit operator IntPtr(AlignedArrayInt array) {
			return new IntPtr(array.pAlignedArray);
		}

		public void Extract(Int64 index, Int32[] destinationArray, Int64 destinationIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				destinationArray[destinationIndex + i] = this.pAlignedArray[index + i];
		}

		public IntPtr GetPtr(Int64 index) {
			if (index >= this.Length || index < 0)
				throw new IndexOutOfRangeException();

			return new IntPtr(this.pAlignedArray + index);
		}

		public void Insert(Int64 index, Int32[] sourceArray, Int64 sourceIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				this.pAlignedArray[index + i] = sourceArray[sourceIndex + i];
		}
	}
	#endregion

	#region AlignedArrayLong
	/// <summary>
	///     Aligned 1D array class for longs
	/// </summary>
	unsafe public class AlignedArrayLong : AlignedArray<Int64> {
		readonly Int64* pAlignedArray;

		public Int64 this[Int64 index] {
			get {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				return this.pAlignedArray[index];
			}
			set {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				this.pAlignedArray[index] = value;
			}
		}

		public AlignedArrayLong(Int64 size, Int64 byteAlignment)
			: base(size, byteAlignment) {
			this.pAlignedArray = (Int64*)this.AlignedMemory.ToPointer();
		}

		public static implicit operator IntPtr(AlignedArrayLong array) {
			return new IntPtr(array.pAlignedArray);
		}

		public void Extract(Int64 index, Int64[] destinationArray, Int64 destinationIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				destinationArray[destinationIndex + i] = this.pAlignedArray[index + i];
		}

		public IntPtr GetPtr(Int64 index) {
			if (index >= this.Length || index < 0)
				throw new IndexOutOfRangeException();

			return new IntPtr(this.pAlignedArray + index);
		}

		public void Insert(Int64 index, Int64[] sourceArray, Int64 sourceIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				this.pAlignedArray[index + i] = sourceArray[sourceIndex + i];
		}
	}
	#endregion

	#region AlignedArrayFloat
	/// <summary>
	///     Aligned 1D array class for floats
	/// </summary>
	unsafe public class AlignedArrayFloat : AlignedArray<Single> {
		readonly Single* pAlignedArray;

		public Single this[Int64 index] {
			get {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				return this.pAlignedArray[index];
			}
			set {
				if (index < 0 || index >= this.Length)
					throw new IndexOutOfRangeException();

				this.pAlignedArray[index] = value;
			}
		}

		public AlignedArrayFloat(Int64 size, Int64 byteAlignment)
			: base(size, byteAlignment) {
			this.pAlignedArray = (Single*)this.AlignedMemory.ToPointer();
		}

		public static implicit operator IntPtr(AlignedArrayFloat array) {
			return new IntPtr(array.pAlignedArray);
		}

		public void Extract(Int64 index, Single[] destinationArray, Int64 destinationIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				destinationArray[destinationIndex + i] = this.pAlignedArray[index + i];
		}

		public IntPtr GetPtr(Int64 index) {
			if (index >= this.Length || index < 0)
				throw new IndexOutOfRangeException();

			return new IntPtr(this.pAlignedArray + index);
		}

		public void Insert(Int64 index, Single[] sourceArray, Int64 sourceIndex, Int64 length) {
			if (index + length >= this.Length || index + length < 0)
				throw new IndexOutOfRangeException();

			for (Int64 i = 0; i < length; i++)
				this.pAlignedArray[index + i] = sourceArray[sourceIndex + i];
		}
	}
	#endregion
}