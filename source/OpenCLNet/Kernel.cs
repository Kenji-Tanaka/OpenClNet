using System;

namespace OpenCLNet {
	/// <summary>
	///     The Kernel class wraps an OpenCL kernel handle
	///     The main purposes of this class is to serve as a handle to
	///     a compiled OpenCL function and to set arguments on the function
	///     before enqueueing calls.
	///     Arguments are set using either the overloaded SetArg functions or
	///     explicit Set*Arg functions where * is a type. The most usual types
	///     are supported, but no vectors. If you need to set a parameter that's
	///     more advanced than what's supported here, use the version of SetArg
	///     that takes a pointer and size.
	///     Note that pointer arguments are set by passing their OpenCL memory object,
	///     not native pointers.
	/// </summary>
	unsafe public class Kernel : InteropTools.IPropertyContainer {
		// Track whether Dispose has been called.
		private Boolean disposed;

		public String FunctionName {
			get { return InteropTools.ReadString(this, (UInt32)KernelInfo.FUNCTION_NAME); }
		}

		public UInt32 NumArgs {
			get { return InteropTools.ReadUInt(this, (UInt32)KernelInfo.NUM_ARGS); }
		}

		public UInt32 ReferenceCount {
			get { return InteropTools.ReadUInt(this, (UInt32)KernelInfo.REFERENCE_COUNT); }
		}

		public Context Context { get; protected set; }
		public Program Program { get; protected set; }
		public IntPtr KernelID { get; set; }

		internal Kernel(Context context, Program program, IntPtr kernelID) {
			this.Context = context;
			this.Program = program;
			this.KernelID = kernelID;
		}

		~Kernel() {
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
				OpenCL.ReleaseKernel(this.KernelID);
				this.KernelID = IntPtr.Zero;

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		public void SetArg(Int32 argIndex, IntPtr argSize, IntPtr argValue) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, argSize, argValue.ToPointer());
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		#region SetArg functions
		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, SByte c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(SByte)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, Byte c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Byte)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, System.Int16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(System.Int16)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, System.UInt16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(System.UInt16)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, Int32 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Int32)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, UInt32 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(UInt32)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, Int64 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Int64)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, UInt64 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(UInt64)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, Single c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Single)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, Double c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Double)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to c
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetArg(Int32 argIndex, IntPtr c) {
			ErrorCode result;
			var lc = c;
			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(IntPtr), &lc);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Set argument argIndex to mem
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="mem"></param>
		public void SetArg(Int32 argIndex, Mem mem) {
			this.SetArg(argIndex, mem.MemID);
		}

		/// <summary>
		///     Set argument argIndex to sampler
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="sampler"></param>
		public void SetArg(Int32 argIndex, Sampler sampler) {
			this.SetArg(argIndex, sampler.SamplerID);
		}

		#region Vector Set functions
		#region Vector2
		public void SetArg(Int32 argIndex, Char2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Char2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UChar2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UChar2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Short2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Short2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UShort2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UShort2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Int2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Int2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UInt2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UInt2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Long2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Long2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, ULong2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(ULong2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Float2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Float2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Double2 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Double2), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}
		#endregion

		#region Vector3
		public void SetArg(Int32 argIndex, Char3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Char3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UChar3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UChar3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Short3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Short3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UShort3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UShort3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Int3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Int3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UInt3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UInt3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Long3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Long3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, ULong3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(ULong3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Float3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Float3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Double3 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Double3), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}
		#endregion

		#region Vector4
		public void SetArg(Int32 argIndex, Char4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Char4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UChar4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UChar4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Short4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Short4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UShort4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UShort4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Int4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Int4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UInt4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UInt4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Long4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Long4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, ULong4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(ULong4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Float4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Float4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Double4 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Double4), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}
		#endregion

		#region Vector8
		public void SetArg(Int32 argIndex, Char8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Char8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UChar8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UChar8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Short8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Short8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UShort8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UShort8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Int8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Int8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UInt8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UInt8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Long8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Long8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, ULong8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(ULong8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Float8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Float8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Double8 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Double8), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}
		#endregion

		#region Vector16
		public void SetArg(Int32 argIndex, Char16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Char16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UChar16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UChar16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Short16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Short16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UShort16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UShort16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Int16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Int16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, UInt16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(UInt16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Long16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Long16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, ULong16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(ULong16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Float16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Float16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetArg(Int32 argIndex, Double16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(Double16), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}
		#endregion
		#endregion
		#endregion

		#region SetSizeTArg
		/// <summary>
		///     This function will assign a value to a kernel argument of type size_t.
		///     size_t is 32 bit
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetSizeTArg(Int32 argIndex, IntPtr c) {
			ErrorCode result;
			if (this.Context.Is64BitContext) {
				var l = c.ToInt64();
				result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(8), &l);
			}
			else {
				var i = c.ToInt32();
				result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(4), &i);
			}
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetSizeTArg(Int32 argIndex, Int32 c) {
			ErrorCode result;
			if (this.Context.Is64BitContext) {
				var l = (Int64)c;
				result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(8), &l);
			}
			else {
				var i = c;
				result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(4), &i);
			}
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetSizeTArg(Int32 argIndex, Int64 c) {
			ErrorCode result;
			if (this.Context.Is64BitContext) {
				var l = c;
				result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(8), &l);
			}
			else {
				var i = (Int32)c;
				result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(4), &i);
			}
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}
		#endregion

		#region Setargs with explicit function names(For VB mostly)
		public void SetSByteArg(Int32 argIndex, SByte c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(SByte)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetByteArg(Int32 argIndex, Byte c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Byte)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetShortArg(Int32 argIndex, System.Int16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(System.Int16)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetUShortArg(Int32 argIndex, System.UInt16 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(System.UInt16)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetIntArg(Int32 argIndex, Int32 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Int32)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetUIntArg(Int32 argIndex, UInt32 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(UInt32)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetLongArg(Int32 argIndex, Int64 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Int64)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetULongArg(Int32 argIndex, UInt64 c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(UInt64)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetSingleArg(Int32 argIndex, Single c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Single)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetDoubleArg(Int32 argIndex, Double c) {
			ErrorCode result;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, new IntPtr(sizeof(Double)), &c);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		/// <summary>
		///     Note that this function sets C# IntPtr args that are handles to OpenCL memory objects.
		///     The OpenCL-C datatype intptr_t is not available to use as a kernel argument.
		///     Use the type size_t and SetSizeTArg if you need platform specific integer sizes.
		/// </summary>
		/// <param name="argIndex"></param>
		/// <param name="c"></param>
		public void SetIntPtrArg(Int32 argIndex, IntPtr c) {
			ErrorCode result;
			var lc = c;

			result = OpenCL.SetKernelArg(this.KernelID, (UInt32)argIndex, (IntPtr)sizeof(IntPtr), &lc);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetArg failed with error code " + result, result);
		}

		public void SetMemArg(Int32 argIndex, Mem mem) {
			this.SetIntPtrArg(argIndex, mem.MemID);
		}

		public void SetSamplerArg(Int32 argIndex, Sampler sampler) {
			this.SetIntPtrArg(argIndex, sampler.SamplerID);
		}
		#endregion

#if false // Have to add some endian checking before compiling these into the library

        #region Set Char vectors
        
        public void SetChar2Arg(int argIndex, sbyte s0, sbyte s1)
        {
            sbyte* pBuffer = stackalloc sbyte[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(sbyte) * 2), (IntPtr)pBuffer);
        }

        public void SetChar4Arg(int argIndex, sbyte s0, sbyte s1, sbyte s2, sbyte s3)
        {
            sbyte* pBuffer = stackalloc sbyte[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(sbyte) * 4), (IntPtr)pBuffer);
        }

        #endregion
        
        #region Set UChar vectors

        public void SetUChar2Arg(int argIndex, byte s0, byte s1)
        {
            byte* pBuffer = stackalloc byte[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(byte) * 2), (IntPtr)pBuffer);
        }

        public void SetUChar4Arg(int argIndex, byte s0, byte s1, byte s2, byte s3)
        {
            byte* pBuffer = stackalloc byte[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(byte) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set Int vectors

        public void SetInt2Arg(int argIndex, int s0, int s1)
        {
            int* pBuffer = stackalloc int[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(int) * 2), (IntPtr)pBuffer);
        }

        public void SetInt4Arg(int argIndex, int s0, int s1, int s2, int s3)
        {
            int* pBuffer = stackalloc int[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(int) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set UInt vectors

        public void SetUInt2Arg(int argIndex, uint s0, uint s1)
        {
            uint* pBuffer = stackalloc uint[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(uint) * 2), (IntPtr)pBuffer);
        }

        public void SetUInt4Arg(int argIndex, uint s0, uint s1, uint s2, uint s3)
        {
            uint* pBuffer = stackalloc uint[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(uint) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set Long vectors

        public void SetLong2Arg(int argIndex, long s0, long s1)
        {
            long* pBuffer = stackalloc long[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(long) * 2), (IntPtr)pBuffer);
        }

        public void SetLong4Arg(int argIndex, long s0, long s1, long s2, long s3)
        {
            long* pBuffer = stackalloc long[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(long) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set ULong vectors

        public void SetULong2Arg(int argIndex, ulong s0, ulong s1)
        {
            ulong* pBuffer = stackalloc ulong[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(ulong) * 2), (IntPtr)pBuffer);
        }

        public void SetULong4Arg(int argIndex, ulong s0, ulong s1, ulong s2, ulong s3)
        {
            ulong* pBuffer = stackalloc ulong[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(ulong) * 4), (IntPtr)pBuffer);
        }

        #endregion

        #region Set Float vectors

        public void SetFloat2Arg(int argIndex, float s0, float s1)
        {
            float* pBuffer = stackalloc float[2];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            SetArg(argIndex, (IntPtr)(sizeof(float) * 2), (IntPtr)pBuffer);
        }

        public void SetFloat4Arg(int argIndex, float s0, float s1, float s2, float s3)
        {
            float* pBuffer = stackalloc float[4];
            pBuffer[0] = s0;
            pBuffer[1] = s1;
            pBuffer[2] = s2;
            pBuffer[3] = s3;
            SetArg(argIndex, (IntPtr)(sizeof(float) * 4), (IntPtr)pBuffer);
        }

        #endregion

#endif
		public static implicit operator IntPtr(Kernel k) {
			return k.KernelID;
		}

		#region IPropertyContainer Members
		public IntPtr GetPropertySize(UInt32 key) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetKernelInfo(this.KernelID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("Unable to get kernel info for kernel " + this.KernelID, result);
			return size;
		}

		public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetKernelInfo(this.KernelID, key, keyLength, pBuffer, out size);
			if (result != (Int32)ErrorCode.SUCCESS)
				throw new OpenCLException("Unable to get kernel info for kernel " + this.KernelID, result);
		}
		#endregion
	}
}