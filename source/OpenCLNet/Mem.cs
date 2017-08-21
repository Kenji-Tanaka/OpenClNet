using System;

namespace OpenCLNet {
	unsafe public class Mem : IDisposable, InteropTools.IPropertyContainer {
		// Track whether Dispose has been called.
		private Boolean disposed;

		private readonly TextureInfo TxInfo;
		public Context Context { get; protected set; }

		public IntPtr HostPtr {
			get { return InteropTools.ReadIntPtr(this, (UInt32)MemInfo.HOST_PTR); }
		}

		public UInt32 MapCount {
			get { return InteropTools.ReadUInt(this, (UInt32)MemInfo.MAP_COUNT); }
		}

		public MemFlags MemFlags {
			get { return (MemFlags)InteropTools.ReadULong(this, (UInt32)MemInfo.FLAGS); }
		}

		public IntPtr MemID { get; protected set; }

		public IntPtr MemSize {
			get { return InteropTools.ReadIntPtr(this, (UInt32)MemInfo.SIZE); }
		}

		public MemObjectType MemType {
			get { return (MemObjectType)InteropTools.ReadUInt(this, (UInt32)MemInfo.TYPE); }
		}

		public Int32 MipMapLevel {
			get { return InteropTools.ReadInt(this.TxInfo, (UInt32)CLGLTextureInfo.MIPMAP_LEVEL); }
		}

		public UInt32 ReferenceCount {
			get { return InteropTools.ReadUInt(this, (UInt32)MemInfo.REFERENCE_COUNT); }
		}

		public UInt32 TextureTarget {
			get { return InteropTools.ReadUInt(this.TxInfo, (UInt32)CLGLTextureInfo.TEXTURE_TARGET); }
		}

		public static implicit operator IntPtr(Mem m) {
			return m.MemID;
		}

		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="flags"></param>
		/// <param name="buffer_create_info"></param>
		/// <param name="errcode_ret"></param>
		/// <returns></returns>
		public Mem CreateSubBuffer(Mem buffer, MemFlags flags, BufferRegion buffer_create_info, out ErrorCode errcode_ret) {
			var memID = OpenCL.CreateSubBuffer(buffer.MemID, flags, buffer_create_info, out errcode_ret);
			return new Mem(buffer.Context, memID);
		}

		public void GetGLObjectInfo(out CLGLObjectType glObjectType, out IntPtr glObjectName) {
			ErrorCode result;
			UInt32 type;
			UInt32 name;

			result = OpenCL.GetGLObjectInfo(this.MemID, out type, out name);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetGLObjectInfo failed: " + result, result);
			glObjectType = (CLGLObjectType)type;
			glObjectName = (IntPtr)name;
		}

		class TextureInfo : InteropTools.IPropertyContainer {
			readonly Mem Mem;

			public TextureInfo(Mem mem) {
				this.Mem = mem;
			}

			#region IPropertyContainer Members
			public IntPtr GetPropertySize(UInt32 key) {
				ErrorCode result;
				IntPtr size;

				result = OpenCL.GetGLTextureInfo(this.Mem.MemID, key, IntPtr.Zero, null, out size);
				if (result != ErrorCode.SUCCESS)
					throw new OpenCLException("GetGLTextureInfo failed with error code " + result, result);

				return size;
			}

			public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
				ErrorCode result;
				IntPtr size;

				result = OpenCL.GetGLTextureInfo(this.Mem.MemID, key, keyLength, pBuffer, out size);
				if (result != ErrorCode.SUCCESS)
					throw new OpenCLException("GetGLTextureInfo failed with error code " + result, result);
			}
			#endregion
		}

		#region Construction / Destruction
		internal Mem(Context context, IntPtr memID) {
			this.Context = context;
			this.MemID = memID;
			this.TxInfo = new TextureInfo(this);
		}

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~Mem() {
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
				OpenCL.ReleaseMemObject(this.MemID);
				this.MemID = IntPtr.Zero;

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		#region Utility functions
		#region Write
		public virtual void Write(CommandQueue cq, Int64 dstOffset, Byte[] srcData, Int32 srcStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstOffset, count);
			var pBlock = (Byte*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = srcData[i + srcStartIndex];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Write(CommandQueue cq, Int64 dstOffset, System.Int16[] srcData, Int32 srcStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstOffset, (Int64)count * sizeof(System.Int16));
			var pBlock = (System.Int16*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = srcData[i + srcStartIndex];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Write(CommandQueue cq, Int64 dstOffset, Int32[] srcData, Int32 srcStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstOffset, (Int64)count * sizeof(Int32));
			var pBlock = (Int32*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = srcData[i + srcStartIndex];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Write(CommandQueue cq, Int64 dstOffset, Single[] srcData, Int32 srcStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstOffset, (Int64)count * sizeof(Single));
			var pBlock = (Single*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = srcData[i + srcStartIndex];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Write(CommandQueue cq, Int64 dstOffset, Double[] srcData, Int32 srcStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstOffset, (Int64)count * sizeof(Double));
			var pBlock = (Double*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = srcData[i + srcStartIndex];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}
		#endregion

		#region Read
		public virtual void Read(CommandQueue cq, Int64 srcOffset, Byte[] dstData, Int32 dstStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.READ, srcOffset, count);
			var pBlock = (Byte*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				dstData[dstStartIndex + i] = pBlock[i];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Read(CommandQueue cq, Int64 srcOffset, System.Int16[] dstData, Int32 dstStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.READ, srcOffset, count * sizeof(System.Int16));
			var pBlock = (System.Int16*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				dstData[dstStartIndex + i] = pBlock[i];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Read(CommandQueue cq, Int64 srcOffset, Int32[] dstData, Int32 dstStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.READ, srcOffset, count * sizeof(Int32));
			var pBlock = (Int32*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				dstData[dstStartIndex + i] = pBlock[i];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Read(CommandQueue cq, Int64 srcOffset, Single[] dstData, Int32 dstStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.READ, srcOffset, count * sizeof(Single));
			var pBlock = (Single*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				dstData[dstStartIndex + i] = pBlock[i];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void Read(CommandQueue cq, Int64 srcOffset, Double[] dstData, Int32 dstStartIndex, Int32 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.READ, srcOffset, count * sizeof(Double));
			var pBlock = (Double*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				dstData[dstStartIndex + i] = pBlock[i];
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}
		#endregion

		#region MemSet
		public virtual void MemSet(CommandQueue cq, Int64 dstByteOffset, Byte value, Int64 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstByteOffset, count);
			var pBlock = (Byte*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = value;
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void MemSet(CommandQueue cq, Int64 dstByteOffset, System.Int16 value, Int64 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstByteOffset, count * sizeof(System.Int16));
			var pBlock = (System.Int16*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = value;
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void MemSet(CommandQueue cq, Int64 dstByteOffset, Int32 value, Int64 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstByteOffset, count * sizeof(Int32));
			var pBlock = (Int32*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = value;
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void MemSet(CommandQueue cq, Int64 dstByteOffset, Single value, Int64 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstByteOffset, count * sizeof(Single));
			var pBlock = (Single*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = value;
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void MemSet(CommandQueue cq, Int64 dstByteOffset, Double value, Int64 count) {
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, dstByteOffset, count * sizeof(Double));
			var pBlock = (Double*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = value;
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}

		public virtual void MemSet(CommandQueue cq, Byte value) {
			Int64 offset = 0;
			var count = this.MemSize.ToInt64();
			var p = cq.EnqueueMapBuffer(this, true, MapFlags.WRITE, offset, count);
			var pBlock = (Byte*)p.ToPointer();
			for (Int64 i = 0; i < count; i++)
				pBlock[i] = value;
			cq.EnqueueUnmapMemObject(this, p);
			cq.Finish();
		}
		#endregion
		#endregion

		#region IPropertyContainer Members
		public virtual IntPtr GetPropertySize(UInt32 key) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetMemObjectInfo(this.MemID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetMemObjectInfo failed: " + result, result);
			return size;
		}

		public virtual void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetMemObjectInfo(this.MemID, key, keyLength, pBuffer, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetMemObjectInfo failed: " + result, result);
		}
		#endregion
	}
}