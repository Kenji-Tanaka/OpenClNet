using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	public unsafe class Image : Mem {
		internal Image(Context context, IntPtr memID)
			: base(context, memID) { }

		#region Properties
		public ImageFormat ImageFormat {
			get {
				var size = this.GetPropertySize((UInt32)ImageInfo.FORMAT);
				Byte* pBuffer = stackalloc Byte[(Int32)size];

				this.ReadProperty((UInt32)ImageInfo.FORMAT, size, pBuffer);
				return (ImageFormat)Marshal.PtrToStructure((IntPtr)pBuffer, typeof(ImageFormat));
			}
		}

		public IntPtr ElementSize {
			get { return InteropTools.ReadIntPtr(this, (UInt32)ImageInfo.ELEMENT_SIZE); }
		}

		public IntPtr RowPitch {
			get { return InteropTools.ReadIntPtr(this, (UInt32)ImageInfo.ROW_PITCH); }
		}

		public IntPtr SlicePitch {
			get { return InteropTools.ReadIntPtr(this, (UInt32)ImageInfo.SLICE_PITCH); }
		}

		public IntPtr Width {
			get { return InteropTools.ReadIntPtr(this, (UInt32)ImageInfo.WIDTH); }
		}

		public IntPtr Height {
			get { return InteropTools.ReadIntPtr(this, (UInt32)ImageInfo.HEIGHT); }
		}

		public IntPtr Depth {
			get { return InteropTools.ReadIntPtr(this, (UInt32)ImageInfo.DEPTH); }
		}
		#endregion

		// Override the IPropertyContainer interface of the Mem class.

		#region IPropertyContainer Members
		public override IntPtr GetPropertySize(UInt32 key) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetImageInfo(this.MemID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS) {
				size = base.GetPropertySize(key);
			}
			return size;
		}

		public override void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetImageInfo(this.MemID, key, keyLength, pBuffer, out size);
			if (result != ErrorCode.SUCCESS) {
				base.ReadProperty(key, keyLength, pBuffer);
			}
		}
		#endregion
	}
}