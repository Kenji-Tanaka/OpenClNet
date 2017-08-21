using System;

namespace OpenCLNet {
	/// <summary>
	///     Aligned 1D array class for longs
	/// </summary>
	public unsafe class AlignedArrayLong : AlignedArray<Int64> {
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
}