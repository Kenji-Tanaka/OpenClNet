using System;

namespace OpenCLNet {
	/// <summary>
	///     Aligned 1D array class for bytes
	/// </summary>
	public unsafe class AlignedArrayByte : AlignedArray<Byte> {
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
}