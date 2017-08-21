using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenCLNet {
	/// <summary>
	///     The CommandQueue class wraps an OpenCL command queue reference.
	///     This class contains methods that correspond to all OpenCL functions that take
	///     a command queue as their first parameter. Most notably, all the Enqueue() functions.
	///     In effect, it makes this class into the workhorse of most OpenCL applications.
	/// </summary>
	unsafe public class CommandQueue : IDisposable {
		// Track whether Dispose has been called.
		private Boolean disposed;
		public IntPtr CommandQueueID { get; private set; }
		public Context Context { get; }
		public Device Device { get; }

		public CommandQueueProperties Properties {
			get { return 0; }
		}

		public UInt32 ReferenceCount {
			get { return 0; }
		}

		public static implicit operator IntPtr(CommandQueue cq) {
			return cq.CommandQueueID;
		}

		#region EnqueueBarrier
		public void EnqueueBarrier() {
			OpenCL.EnqueueBarrier(this.CommandQueueID);
		}
		#endregion

		#region EnqueueMarker
		public void EnqueueMarker(out Event _event) {
			IntPtr tmpEvent;

			OpenCL.EnqueueMarker(this.CommandQueueID, &tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
		}
		#endregion

		#region Finish
		public void Finish() {
			OpenCL.Finish(this.CommandQueueID);
		}
		#endregion

		#region Flush
		public void Flush() {
			OpenCL.Flush(this.CommandQueueID);
		}
		#endregion

		#region SetProperty
		[Obsolete("Function deprecated in OpenCL 1.1 due to being inherently unsafe", false)]
		public void SetProperty(CommandQueueProperties properties, Boolean enable, out CommandQueueProperties oldProperties) {
			ErrorCode result;
			UInt64 returnedProperties = 0;
#pragma warning disable 618
			result = OpenCL.SetCommandQueueProperty(this.CommandQueueID,
				(UInt64)properties,
				enable,
				out returnedProperties);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetCommandQueueProperty failed with error code " + result, result);
			oldProperties = (CommandQueueProperties)returnedProperties;
#pragma warning restore 618
		}
		#endregion

		#region Construction / Destruction
		internal CommandQueue(Context context, Device device, IntPtr commandQueueID) {
			this.Context = context;
			this.Device = device;
			this.CommandQueueID = commandQueueID;
		}

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~CommandQueue() {
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
				OpenCL.ReleaseCommandQueue(this.CommandQueueID);
				this.CommandQueueID = IntPtr.Zero;

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		// Enqueue methods follow. Typically, each will have 3 versions.
		// One which takes an event wait list and and event output
		// One which takes an event wait list
		// and one which takes neither
		// There are also overloads which take int, long and IntPtr arguments

		#region EnqueueWriteBuffer
		/// <summary>
		///     Enqueues a command to write data to a buffer object identified by buffer from host memory identified by ptr.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="blockingWrite"></param>
		/// <param name="offset"></param>
		/// <param name="cb"></param>
		/// <param name="ptr"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="InteropTools.ConvertEventsToEventIDs(event_wait_list)"></param>
		/// <param name="_event"></param>
		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, IntPtr offset, IntPtr cb, IntPtr ptr, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr.ToPointer(),
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, Int32 offset, Int32 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, Int64 offset, Int64 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, IntPtr offset, IntPtr cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, Int32 offset, Int32 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, Int64 offset, Int64 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, IntPtr offset, IntPtr cb, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, Int32 offset, Int32 cb, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}

		public void EnqueueWriteBuffer(Mem buffer, Boolean blockingWrite, Int64 offset, Int64 cb, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBuffer failed with error code " + result, result);
		}
		#endregion

		#region EnqueueReadBuffer
		/// <summary>
		///     Enqueues a command to read data from a buffer object identified by buffer to host memory identified by ptr.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="blockingRead"></param>
		/// <param name="offset"></param>
		/// <param name="cb"></param>
		/// <param name="ptr"></param>
		/// <param name="numEventsInWaitList"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, IntPtr offset, IntPtr cb, IntPtr ptr, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr.ToPointer(),
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, Int32 offset, Int32 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, Int64 offset, Int64 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, IntPtr offset, IntPtr cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, Int32 offset, Int32 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, Int64 offset, Int64 cb, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);
			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, IntPtr offset, IntPtr cb, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, Int32 offset, Int32 cb, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}

		public void EnqueueReadBuffer(Mem buffer, Boolean blockingRead, Int64 offset, Int64 cb, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueReadBuffer(this.CommandQueueID,
				buffer,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				offset,
				cb,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBuffer failed with error code " + result, result);
		}
		#endregion

		#region EnqueueCopyBuffer
		/// <summary>
		///     Enqueues a command to copy a buffer object identified by src_buffer to another buffer object identified by dst_buffer.
		/// </summary>
		/// <param name="src_buffer"></param>
		/// <param name="dst_buffer"></param>
		/// <param name="src_offset"></param>
		/// <param name="dst_offset"></param>
		/// <param name="cb"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueCopyBuffer(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				src_offset,
				dst_offset,
				cb,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, Int32 src_offset, Int32 dst_offset, Int32 cb, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			this.EnqueueCopyBuffer(src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb, num_events_in_wait_list, event_wait_list, out _event);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, Int64 src_offset, Int64 dst_offset, Int64 cb, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			this.EnqueueCopyBuffer(src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb, num_events_in_wait_list, event_wait_list, out _event);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyBuffer(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				src_offset,
				dst_offset,
				cb,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, Int32 src_offset, Int32 dst_offset, Int32 cb, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			this.EnqueueCopyBuffer(src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb, num_events_in_wait_list, event_wait_list);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, Int64 src_offset, Int64 dst_offset, Int64 cb, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			this.EnqueueCopyBuffer(src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb, num_events_in_wait_list, event_wait_list);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyBuffer(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				src_offset,
				dst_offset,
				cb,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, Int32 src_offset, Int32 dst_offset, Int32 cb) {
			this.EnqueueCopyBuffer(src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb);
		}

		public void EnqueueCopyBuffer(Mem src_buffer, Mem dst_buffer, Int64 src_offset, Int64 dst_offset, Int64 cb) {
			this.EnqueueCopyBuffer(src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb);
		}
		#endregion

		#region EnqueueReadImage
		public void EnqueueReadImage(Mem image, Boolean blockingRead, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				origin,
				region,
				row_pitch,
				slice_pitch,
				ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, Int32[] origin, Int32[] region, Int32 row_pitch, Int32 slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				row_pitch,
				slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, Int64[] origin, Int64[] region, Int64 row_pitch, Int64 slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				row_pitch,
				slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				origin,
				region,
				row_pitch,
				slice_pitch,
				ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, Int32[] origin, Int32[] region, Int32 row_pitch, Int32 slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				row_pitch,
				slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, Int64[] origin, Int64[] region, Int64 row_pitch, Int64 slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				row_pitch,
				slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				origin,
				region,
				row_pitch,
				slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, Int32[] origin, Int32[] region, Int32 row_pitch, Int32 slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				row_pitch,
				slice_pitch,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}

		public void EnqueueReadImage(Mem image, Boolean blockingRead, Int64[] origin, Int64[] region, Int64 row_pitch, Int64 slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueReadImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingRead ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				row_pitch,
				slice_pitch,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadImage failed with error code " + result, result);
		}
		#endregion

		#region EnqueueWriteImage
		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				origin,
				region,
				input_row_pitch,
				input_slice_pitch,
				ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, Int32[] origin, Int32[] region, Int32 input_row_pitch, Int32 input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				input_row_pitch,
				input_slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, Int64[] origin, Int64[] region, Int64 input_row_pitch, Int64 input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				input_row_pitch,
				input_slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				origin,
				region,
				input_row_pitch,
				input_slice_pitch,
				ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, Int32[] origin, Int32[] region, Int32 input_row_pitch, Int32 input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				input_row_pitch,
				input_slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, Int64[] origin, Int64[] region, Int64 input_row_pitch, Int64 input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				input_row_pitch,
				input_slice_pitch,
				ptr,
				num_events_in_wait_list,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				origin,
				region,
				input_row_pitch,
				input_slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, Int32[] origin, Int32[] region, Int32 input_row_pitch, Int32 input_slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				input_row_pitch,
				input_slice_pitch,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}

		public void EnqueueWriteImage(Mem image, Boolean blockingWrite, Int64[] origin, Int64[] region, Int64 input_row_pitch, Int64 input_slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueWriteImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingWrite ? Bool.TRUE : Bool.FALSE),
				repackedOrigin,
				repackedRegion,
				input_row_pitch,
				input_slice_pitch,
				ptr,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteImage failed with error code " + result, result);
		}
		#endregion

		#region EnqueueCopyImage
		public void EnqueueCopyImage(Mem src_image, Mem dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID,
				src_image.MemID,
				dst_image.MemID,
				src_origin,
				dst_origin,
				region,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, Int32[] src_origin, Int32[] dst_origin, Int32[] region, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID, src_image, dst_image, repackedSrcOrigin, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, &tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, Int64[] src_origin, Int64[] dst_origin, Int64[] region, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID, src_image, dst_image, repackedSrcOrigin, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, &tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID,
				src_image.MemID,
				dst_image.MemID,
				src_origin,
				dst_origin,
				region,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, Int32[] src_origin, Int32[] dst_origin, Int32[] region, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID, src_image, dst_image, repackedSrcOrigin, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, Int64[] src_origin, Int64[] dst_origin, Int64[] region, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID, src_image, dst_image, repackedSrcOrigin, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID,
				src_image.MemID,
				dst_image.MemID,
				src_origin,
				dst_origin,
				region,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, Int32[] src_origin, Int32[] dst_origin, Int32[] region) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID, src_image, dst_image, repackedSrcOrigin, repackedDstOrigin, repackedRegion, 0, null, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}

		public void EnqueueCopyImage(Mem src_image, Mem dst_image, Int64[] src_origin, Int64[] dst_origin, Int64[] region) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueCopyImage(this.CommandQueueID, src_image, dst_image, repackedSrcOrigin, repackedDstOrigin, repackedRegion, 0, null, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImage failed with error code " + result, result);
		}
		#endregion

		#region EnqueueCopyImageToBuffer
		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID,
				src_image.MemID,
				dst_buffer.MemID,
				src_origin,
				region,
				dst_offset,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, Int32[] src_origin, Int32[] region, Int32 dst_offset, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID, src_image, dst_buffer, repackedSrcOrigin, repackedRegion, dst_offset, num_events_in_wait_list, repackedEvents, &tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, Int64[] src_origin, Int64[] region, Int64 dst_offset, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID, src_image, dst_buffer, repackedSrcOrigin, repackedRegion, dst_offset, num_events_in_wait_list, repackedEvents, &tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID,
				src_image.MemID,
				dst_buffer.MemID,
				src_origin,
				region,
				dst_offset,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, Int32[] src_origin, Int32[] region, Int32 dst_offset, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID, src_image, dst_buffer, repackedSrcOrigin, repackedRegion, dst_offset, num_events_in_wait_list, repackedEvents, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, Int64[] src_origin, Int64[] region, Int64 dst_offset, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID, src_image, dst_buffer, repackedSrcOrigin, repackedRegion, dst_offset, num_events_in_wait_list, repackedEvents, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID,
				src_image.MemID,
				dst_buffer.MemID,
				src_origin,
				region,
				dst_offset,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, Int32[] src_origin, Int32[] region, Int32 dst_offset) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID, src_image, dst_buffer, repackedSrcOrigin, repackedRegion, dst_offset, 0, null, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}

		public void EnqueueCopyImageToBuffer(Mem src_image, Mem dst_buffer, Int64[] src_origin, Int64[] region, Int64 dst_offset) {
			ErrorCode result;
			IntPtr* repackedSrcOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(src_origin, repackedSrcOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueCopyImageToBuffer(this.CommandQueueID, src_image, dst_buffer, repackedSrcOrigin, repackedRegion, dst_offset, 0, null, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyImageToBuffer failed with error code " + result, result);
		}
		#endregion

		#region EnqueueCopyBufferToImage
		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID,
				src_buffer.MemID,
				dst_image.MemID,
				src_offset,
				dst_origin,
				region,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, Int32 src_offset, Int32[] dst_origin, Int32[] region, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID, src_buffer, dst_image, src_offset, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, &tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, Int64 src_offset, Int64[] dst_origin, Int64[] region, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID, src_buffer, dst_image, src_offset, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, &tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID,
				src_buffer.MemID,
				dst_image.MemID,
				src_offset,
				dst_origin,
				region,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, Int32 src_offset, Int32[] dst_origin, Int32[] region, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID, src_buffer, dst_image, src_offset, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, Int64 src_offset, Int64[] dst_origin, Int64[] region, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID, src_buffer, dst_image, src_offset, repackedDstOrigin, repackedRegion, num_events_in_wait_list, repackedEvents, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID,
				src_buffer.MemID,
				dst_image.MemID,
				src_offset,
				dst_origin,
				region,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, Int32 src_offset, Int32[] dst_origin, Int32[] region) {
			ErrorCode result;
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID, src_buffer, dst_image, src_offset, repackedDstOrigin, repackedRegion, 0, null, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}

		public void EnqueueCopyBufferToImage(Mem src_buffer, Mem dst_image, Int64 src_offset, Int64[] dst_origin, Int64[] region) {
			ErrorCode result;
			IntPtr* repackedDstOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(dst_origin, repackedDstOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			result = OpenCL.EnqueueCopyBufferToImage(this.CommandQueueID, src_buffer, dst_image, src_offset, repackedDstOrigin, repackedRegion, 0, null, null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferToImage failed with error code " + result, result);
		}
		#endregion

		#region EnqueueMapBuffer
		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, IntPtr offset, IntPtr cb, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr ptr;
			IntPtr tmpEvent;

			ptr = (IntPtr)OpenCL.EnqueueMapBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingMap ? Bool.TRUE : Bool.FALSE),
				(UInt64)map_flags,
				offset,
				cb,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent,
				out result);
			_event = new Event(this.Context, this, tmpEvent);

			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return ptr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, Int32 offset, Int32 cb, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapBuffer(this.CommandQueueID, buffer, blockingMap ? (UInt32)Bool.TRUE : (UInt32)Bool.FALSE, (UInt64)map_flags, offset, cb, num_events_in_wait_list, repackedEvents, &tmpEvent, out result);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);

			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, Int64 offset, Int64 cb, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapBuffer(this.CommandQueueID, buffer, blockingMap ? (UInt32)Bool.TRUE : (UInt32)Bool.FALSE, (UInt64)map_flags, offset, cb, num_events_in_wait_list, repackedEvents, &tmpEvent, out result);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, IntPtr offset, IntPtr cb, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr ptr;

			ptr = (IntPtr)OpenCL.EnqueueMapBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingMap ? Bool.TRUE : Bool.FALSE),
				(UInt64)map_flags,
				offset,
				cb,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null,
				out result);

			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return ptr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, Int32 offset, Int32 cb, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapBuffer(this.CommandQueueID, buffer, blockingMap ? (UInt32)Bool.TRUE : (UInt32)Bool.FALSE, (UInt64)map_flags, offset, cb, num_events_in_wait_list, repackedEvents, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, Int64 offset, Int64 cb, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapBuffer(this.CommandQueueID, buffer, blockingMap ? (UInt32)Bool.TRUE : (UInt32)Bool.FALSE, (UInt64)map_flags, offset, cb, num_events_in_wait_list, repackedEvents, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, IntPtr offset, IntPtr cb) {
			ErrorCode result;
			IntPtr ptr;

			ptr = (IntPtr)OpenCL.EnqueueMapBuffer(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blockingMap ? Bool.TRUE : Bool.FALSE),
				(UInt64)map_flags,
				offset,
				cb,
				0,
				null,
				null,
				out result);

			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return ptr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, Int32 offset, Int32 cb) {
			void* pMappedPtr;
			ErrorCode result;

			pMappedPtr = OpenCL.EnqueueMapBuffer(this.CommandQueueID, buffer, blockingMap ? (UInt32)Bool.TRUE : (UInt32)Bool.FALSE, (UInt64)map_flags, offset, cb, 0, null, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapBuffer(Mem buffer, Boolean blockingMap, MapFlags map_flags, Int64 offset, Int64 cb) {
			void* pMappedPtr;
			ErrorCode result;

			pMappedPtr = OpenCL.EnqueueMapBuffer(this.CommandQueueID, buffer, blockingMap ? (UInt32)Bool.TRUE : (UInt32)Bool.FALSE, (UInt64)map_flags, offset, cb, 0, null, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapBuffer failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}
		#endregion

		#region EnqueueMapImage
		/// <summary>
		///     Map the memory of a Mem object into host memory.
		///     This function must be used before native code accesses an area of memory that's under the control
		///     of OpenCL. This includes Mem objects allocated with MemFlags.USE_HOST_PTR, as results may be cached
		///     in another location. Mapping will ensure caches are synchronizatized.
		/// </summary>
		/// <param name="image">Mem object to map</param>
		/// <param name="blockingMap">Flag that indicates if the operation is synchronous or not</param>
		/// <param name="map_flags">Read/Write flags</param>
		/// <param name="origin">origin contains the x,y,z coordinates indicating the starting point to map</param>
		/// <param name="region">origin contains the width,height,depth coordinates indicating the size of the area to map</param>
		/// <param name="image_row_pitch"></param>
		/// <param name="image_slice_pitch"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			IntPtr ptr;
			IntPtr tmpEvent;
			ErrorCode result;

			ptr = (IntPtr)OpenCL.EnqueueMapImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingMap ? Bool.TRUE : Bool.FALSE),
				(UInt64)map_flags,
				origin,
				region,
				out image_row_pitch,
				out image_slice_pitch,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent,
				out result);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return ptr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, Int32[] origin, Int32[] region, out Int32 image_row_pitch, out Int32 image_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapImage(this.CommandQueueID, image, (UInt32)(blockingMap ? Bool.TRUE : Bool.TRUE), (UInt64)map_flags, repackedOrigin, repackedRegion, out image_row_pitch, out image_slice_pitch, num_events_in_wait_list, repackedEvents, &tmpEvent, out result);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, Int64[] origin, Int64[] region, out Int64 image_row_pitch, out Int64 image_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapImage(this.CommandQueueID, image, (UInt32)(blockingMap ? Bool.TRUE : Bool.TRUE), (UInt64)map_flags, repackedOrigin, repackedRegion, out image_row_pitch, out image_slice_pitch, num_events_in_wait_list, repackedEvents, &tmpEvent, out result);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			IntPtr ptr;
			ErrorCode result;

			ptr = (IntPtr)OpenCL.EnqueueMapImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingMap ? Bool.TRUE : Bool.FALSE),
				(UInt64)map_flags,
				origin,
				region,
				out image_row_pitch,
				out image_slice_pitch,
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null,
				out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return ptr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, Int32[] origin, Int32[] region, out Int32 image_row_pitch, out Int32 image_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			void* pMappedPtr;
			ErrorCode result;

			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapImage(this.CommandQueueID, image, (UInt32)(blockingMap ? Bool.TRUE : Bool.TRUE), (UInt64)map_flags, repackedOrigin, repackedRegion, out image_row_pitch, out image_slice_pitch, num_events_in_wait_list, repackedEvents, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, Int64[] origin, Int64[] region, out Int64 image_row_pitch, out Int64 image_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];
			IntPtr* repackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				repackedEvents = null;
			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, repackedEvents);

			pMappedPtr = OpenCL.EnqueueMapImage(this.CommandQueueID, image, (UInt32)(blockingMap ? Bool.TRUE : Bool.TRUE), (UInt64)map_flags, repackedOrigin, repackedRegion, out image_row_pitch, out image_slice_pitch, num_events_in_wait_list, repackedEvents, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch) {
			IntPtr ptr;
			ErrorCode result;

			ptr = (IntPtr)OpenCL.EnqueueMapImage(this.CommandQueueID,
				image.MemID,
				(UInt32)(blockingMap ? Bool.TRUE : Bool.FALSE),
				(UInt64)map_flags,
				origin,
				region,
				out image_row_pitch,
				out image_slice_pitch,
				0,
				null,
				null,
				out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return ptr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, Int32[] origin, Int32[] region, out Int32 image_row_pitch, out Int32 image_slice_pitch) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			pMappedPtr = OpenCL.EnqueueMapImage(this.CommandQueueID, image, (UInt32)(blockingMap ? Bool.TRUE : Bool.TRUE), (UInt64)map_flags, repackedOrigin, repackedRegion, out image_row_pitch, out image_slice_pitch, 0, null, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}

		public IntPtr EnqueueMapImage(Mem image, Boolean blockingMap, MapFlags map_flags, Int64[] origin, Int64[] region, out Int64 image_row_pitch, out Int64 image_slice_pitch) {
			void* pMappedPtr;
			ErrorCode result;
			IntPtr* repackedOrigin = stackalloc IntPtr[3];
			IntPtr* repackedRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(origin, repackedOrigin);
			InteropTools.A3ToIntPtr3(region, repackedRegion);

			pMappedPtr = OpenCL.EnqueueMapImage(this.CommandQueueID, image, (UInt32)(blockingMap ? Bool.TRUE : Bool.TRUE), (UInt64)map_flags, repackedOrigin, repackedRegion, out image_row_pitch, out image_slice_pitch, 0, null, null, out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueMapImage failed with error code " + result, result);
			return (IntPtr)pMappedPtr;
		}
		#endregion

		#region EnqueueUnmapMemObject
		/// <summary>
		///     Unmap a previously mapped Mem object
		/// </summary>
		/// <param name="memobj"></param>
		/// <param name="mapped_ptr"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueUnmapMemObject(Mem memobj, IntPtr mapped_ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueUnmapMemObject(this.CommandQueueID,
				memobj.MemID,
				mapped_ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueUnmapMemObject failed with error code " + result, result);
		}

		public void EnqueueUnmapMemObject(Mem memobj, IntPtr mapped_ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueUnmapMemObject(this.CommandQueueID,
				memobj.MemID,
				mapped_ptr.ToPointer(),
				(UInt32)num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueUnmapMemObject failed with error code " + result, result);
		}

		public void EnqueueUnmapMemObject(Mem memobj, IntPtr mapped_ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueUnmapMemObject(this.CommandQueueID,
				memobj.MemID,
				mapped_ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueUnmapMemObject failed with error code " + result, result);
		}
		#endregion

		#region EnqueueNDRangeKernel
		/// <summary>
		///     Execute a parallel kernel.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="workDim">The number of dimensions in the workspace(0-2), must correspond to the number of dimensions in the following arrays</param>
		/// <param name="globalWorkOffset">null in OpenCL 1.0, but will allow indices to start at non-0 locations</param>
		/// <param name="globalWorkSize">Index n of this array=the length of the n'th dimension of global work space</param>
		/// <param name="localWorkSize">Index n of this array=the length of the n'th dimension of local work space</param>
		/// <param name="numEventsInWaitList"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueNDRangeKernel(Kernel kernel, UInt32 workDim, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize, UInt32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				globalWorkOffset,
				globalWorkSize,
				localWorkSize,
				numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, Int32 workDim, Int32[] globalWorkOffset, Int32[] globalWorkSize, Int32[] localWorkSize, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			IntPtr* pGlobalWorkOffset = stackalloc IntPtr[workDim];
			IntPtr* pGlobalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* pLocalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* repackedEvents = stackalloc IntPtr[numEventsInWaitList];
			if (globalWorkOffset == null)
				pGlobalWorkOffset = null;
			if (globalWorkSize == null)
				pGlobalWorkSize = null;
			if (localWorkSize == null)
				pLocalWorkSize = null;
			if (event_wait_list == null)
				repackedEvents = null;

			InteropTools.AToIntPtr(workDim, globalWorkOffset, pGlobalWorkOffset);
			InteropTools.AToIntPtr(workDim, globalWorkSize, pGlobalWorkSize);
			InteropTools.AToIntPtr(workDim, localWorkSize, pLocalWorkSize);
			InteropTools.ConvertEventsToEventIDs(numEventsInWaitList, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				pGlobalWorkOffset,
				pGlobalWorkSize,
				pLocalWorkSize,
				numEventsInWaitList,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, Int32 workDim, Int64[] globalWorkOffset, Int64[] globalWorkSize, Int64[] localWorkSize, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			IntPtr* pGlobalWorkOffset = stackalloc IntPtr[workDim];
			IntPtr* pGlobalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* pLocalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* repackedEvents = stackalloc IntPtr[numEventsInWaitList];
			if (globalWorkOffset == null)
				pGlobalWorkOffset = null;
			if (globalWorkSize == null)
				pGlobalWorkSize = null;
			if (localWorkSize == null)
				pLocalWorkSize = null;
			if (event_wait_list == null)
				repackedEvents = null;

			InteropTools.AToIntPtr(workDim, globalWorkOffset, pGlobalWorkOffset);
			InteropTools.AToIntPtr(workDim, globalWorkSize, pGlobalWorkSize);
			InteropTools.AToIntPtr(workDim, localWorkSize, pLocalWorkSize);
			InteropTools.ConvertEventsToEventIDs(numEventsInWaitList, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				pGlobalWorkOffset,
				pGlobalWorkSize,
				pLocalWorkSize,
				numEventsInWaitList,
				repackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, UInt32 workDim, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize, UInt32 numEventsInWaitList, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				globalWorkOffset,
				globalWorkSize,
				localWorkSize,
				numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, Int32 workDim, Int32[] globalWorkOffset, Int32[] globalWorkSize, Int32[] localWorkSize, Int32 numEventsInWaitList, Event[] event_wait_list) {
			ErrorCode result;

			IntPtr* pGlobalWorkOffset = stackalloc IntPtr[workDim];
			IntPtr* pGlobalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* pLocalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* repackedEvents = stackalloc IntPtr[numEventsInWaitList];
			if (globalWorkOffset == null)
				pGlobalWorkOffset = null;
			if (globalWorkSize == null)
				pGlobalWorkSize = null;
			if (localWorkSize == null)
				pLocalWorkSize = null;
			if (event_wait_list == null)
				repackedEvents = null;

			InteropTools.AToIntPtr(workDim, globalWorkOffset, pGlobalWorkOffset);
			InteropTools.AToIntPtr(workDim, globalWorkSize, pGlobalWorkSize);
			InteropTools.AToIntPtr(workDim, localWorkSize, pLocalWorkSize);
			InteropTools.ConvertEventsToEventIDs(numEventsInWaitList, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				pGlobalWorkOffset,
				pGlobalWorkSize,
				pLocalWorkSize,
				numEventsInWaitList,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, Int32 workDim, Int64[] globalWorkOffset, Int64[] globalWorkSize, Int64[] localWorkSize, Int32 numEventsInWaitList, Event[] event_wait_list) {
			ErrorCode result;

			IntPtr* pGlobalWorkOffset = stackalloc IntPtr[workDim];
			IntPtr* pGlobalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* pLocalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* repackedEvents = stackalloc IntPtr[numEventsInWaitList];
			if (globalWorkOffset == null)
				pGlobalWorkOffset = null;
			if (globalWorkSize == null)
				pGlobalWorkSize = null;
			if (localWorkSize == null)
				pLocalWorkSize = null;
			if (event_wait_list == null)
				repackedEvents = null;

			InteropTools.AToIntPtr(workDim, globalWorkOffset, pGlobalWorkOffset);
			InteropTools.AToIntPtr(workDim, globalWorkSize, pGlobalWorkSize);
			InteropTools.AToIntPtr(workDim, localWorkSize, pLocalWorkSize);
			InteropTools.ConvertEventsToEventIDs(numEventsInWaitList, event_wait_list, repackedEvents);

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				pGlobalWorkOffset,
				pGlobalWorkSize,
				pLocalWorkSize,
				numEventsInWaitList,
				repackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, UInt32 workDim, IntPtr[] globalWorkOffset, IntPtr[] globalWorkSize, IntPtr[] localWorkSize) {
			ErrorCode result;

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				globalWorkOffset,
				globalWorkSize,
				localWorkSize,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, Int32 workDim, Int32[] globalWorkOffset, Int32[] globalWorkSize, Int32[] localWorkSize) {
			ErrorCode result;

			IntPtr* pGlobalWorkOffset = stackalloc IntPtr[workDim];
			IntPtr* pGlobalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* pLocalWorkSize = stackalloc IntPtr[workDim];
			if (globalWorkOffset == null)
				pGlobalWorkOffset = null;
			if (globalWorkSize == null)
				pGlobalWorkSize = null;
			if (localWorkSize == null)
				pLocalWorkSize = null;

			InteropTools.AToIntPtr(workDim, globalWorkOffset, pGlobalWorkOffset);
			InteropTools.AToIntPtr(workDim, globalWorkSize, pGlobalWorkSize);
			InteropTools.AToIntPtr(workDim, localWorkSize, pLocalWorkSize);

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				pGlobalWorkOffset,
				pGlobalWorkSize,
				pLocalWorkSize,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}

		public void EnqueueNDRangeKernel(Kernel kernel, Int32 workDim, Int64[] globalWorkOffset, Int64[] globalWorkSize, Int64[] localWorkSize) {
			ErrorCode result;

			IntPtr* pGlobalWorkOffset = stackalloc IntPtr[workDim];
			IntPtr* pGlobalWorkSize = stackalloc IntPtr[workDim];
			IntPtr* pLocalWorkSize = stackalloc IntPtr[workDim];
			if (globalWorkOffset == null)
				pGlobalWorkOffset = null;
			if (globalWorkSize == null)
				pGlobalWorkSize = null;
			if (localWorkSize == null)
				pLocalWorkSize = null;

			InteropTools.AToIntPtr(workDim, globalWorkOffset, pGlobalWorkOffset);
			InteropTools.AToIntPtr(workDim, globalWorkSize, pGlobalWorkSize);
			InteropTools.AToIntPtr(workDim, localWorkSize, pLocalWorkSize);

			result = OpenCL.EnqueueNDRangeKernel(this.CommandQueueID,
				kernel.KernelID,
				workDim,
				pGlobalWorkOffset,
				pGlobalWorkSize,
				pLocalWorkSize,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNDRangeKernel failed with error code " + result, result);
		}
		#endregion

		#region EnqueueTask
		/// <summary>
		///     Execute a simple kernel
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="numEventsInWaitList"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueTask(Kernel kernel, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueTask(this.CommandQueueID,
				kernel.KernelID,
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueTask failed with error code " + result, result);
		}

		/// <summary>
		///     Execute a simple kernel
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="numEventsInWaitList"></param>
		/// <param name="event_wait_list"></param>
		public void EnqueueTask(Kernel kernel, Int32 numEventsInWaitList, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueTask(this.CommandQueueID,
				kernel.KernelID,
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueTask failed with error code " + result, result);
		}

		/// <summary>
		///     Execute a simple kernel
		/// </summary>
		/// <param name="kernel"></param>
		public void EnqueueTask(Kernel kernel) {
			ErrorCode result;

			result = OpenCL.EnqueueTask(this.CommandQueueID,
				kernel.KernelID,
				(UInt32)0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueTask failed with error code " + result, result);
		}
		#endregion

		#region EnqueueNativeKernel
		internal class NativeKernelCallbackData {
			internal Mem[] Buffers;
			internal CommandQueue CQ;
			internal NativeKernel NativeKernel;
			internal Object O;

			internal NativeKernelCallbackData(NativeKernel nk, CommandQueue cq, Object o, Mem[] buffers) {
				this.NativeKernel = nk;
				this.CQ = cq;
				this.O = o;
				this.Buffers = buffers;
			}
		}

		private static readonly Mutex NativeKernelParamsMutex = new Mutex();
		private static Int32 NativeKernelParamsId;
		private static readonly NativeKernelInternal NativeKernelDelegate = CommandQueue.NativeKernelCallback;
		private static readonly Dictionary<Int32, NativeKernelCallbackData> NativeKernelDispatch = new Dictionary<Int32, NativeKernelCallbackData>();

		private static void NativeKernelCallback(void* args) {
			var callbackId = *(Int32*)args;
			var callbackData = CommandQueue.GetCallback(callbackId);
			void*[] buffers;

			if (callbackData.Buffers != null) {
				buffers = new void*[callbackData.Buffers.Length];
				for (var i = 0; i < buffers.Length; i++)
					buffers[i] = callbackData.CQ.EnqueueMapBuffer(callbackData.Buffers[i], true, MapFlags.READ_WRITE, IntPtr.Zero, callbackData.Buffers[i].MemSize).ToPointer();
			}
			else
				buffers = null;

			callbackData.NativeKernel(callbackData.O, buffers);

			if (buffers != null) {
				for (var i = 0; i < buffers.Length; i++)
					callbackData.CQ.EnqueueUnmapMemObject(callbackData.Buffers[i], (IntPtr)buffers[i]);
			}
			CommandQueue.RemoveCallback(callbackId);
		}

		private static Int32 AddNativeKernelParams(NativeKernel nk, CommandQueue cq, Object o, Mem[] buffers) {
			Int32 callbackId;
			var callbackData = new NativeKernelCallbackData(nk, cq, o, buffers);
			var gotMutex = false;
			try {
				gotMutex = CommandQueue.NativeKernelParamsMutex.WaitOne();
				do {
					callbackId = CommandQueue.NativeKernelParamsId++;
				} while (CommandQueue.NativeKernelDispatch.ContainsKey(callbackId));
				CommandQueue.NativeKernelDispatch.Add(callbackId, callbackData);
			}
			finally {
				if (gotMutex)
					CommandQueue.NativeKernelParamsMutex.ReleaseMutex();
			}
			return callbackId;
		}

		private static NativeKernelCallbackData GetCallback(Int32 callbackId) {
			NativeKernelCallbackData callbackData = null;
			var gotMutex = false;
			try {
				gotMutex = CommandQueue.NativeKernelParamsMutex.WaitOne();
				callbackData = CommandQueue.NativeKernelDispatch[callbackId];
			}
			finally {
				if (gotMutex)
					CommandQueue.NativeKernelParamsMutex.ReleaseMutex();
			}
			return callbackData;
		}

		private static void RemoveCallback(Int32 callbackId) {
			var gotMutex = false;
			try {
				gotMutex = CommandQueue.NativeKernelParamsMutex.WaitOne();
				CommandQueue.NativeKernelDispatch.Remove(callbackId);
			}
			finally {
				if (gotMutex)
					CommandQueue.NativeKernelParamsMutex.ReleaseMutex();
			}
		}

		/// <summary>
		///     Enqueue a user function. This function is only supported if
		///     DeviceExecCapabilities.NATIVE_KERNEL set in Device.ExecutionCapabilities
		/// </summary>
		/// <param name="nativeKernel"></param>
		/// <param name="numEventsInWaitList"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueNativeKernel(NativeKernel nativeKernel, Object userObject, Mem[] buffers, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			Int32 callbackId;

			callbackId = CommandQueue.AddNativeKernelParams(nativeKernel, this, userObject, buffers);
			result = OpenCL.EnqueueNativeKernel(this.CommandQueueID,
				CommandQueue.NativeKernelDelegate,
				&callbackId,
				(IntPtr)4,
				0,
				null,
				null,
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNativeKernel failed with error code " + result, result);
			_event = new Event(this.Context, this, tmpEvent);
		}

		/// <summary>
		///     Enquque a user function. This function is only supported if
		///     DeviceExecCapabilities.NATIVE_KERNEL set in Device.ExecutionCapabilities
		/// </summary>
		/// <param name="nativeKernel"></param>
		/// <param name="numEventsInWaitList"></param>
		/// <param name="event_wait_list"></param>
		public void EnqueueNativeKernel(NativeKernel nativeKernel, Object userObject, Mem[] buffers, Int32 numEventsInWaitList, Event[] event_wait_list) {
			ErrorCode result;
			Int32 callbackId;

			callbackId = CommandQueue.AddNativeKernelParams(nativeKernel, this, userObject, buffers);
			result = OpenCL.EnqueueNativeKernel(this.CommandQueueID,
				CommandQueue.NativeKernelDelegate,
				&callbackId,
				(IntPtr)4,
				0,
				null,
				null,
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNativeKernel failed with error code " + result, result);
		}

		/// <summary>
		///     Enquque a user function. This function is only supported if
		///     DeviceExecCapabilities.NATIVE_KERNEL set in Device.ExecutionCapabilities
		/// </summary>
		/// <param name="nativeKernel"></param>
		public void EnqueueNativeKernel(NativeKernel nativeKernel, Object userObject, Mem[] buffers) {
			ErrorCode result;
			Int32 callbackId;

			callbackId = CommandQueue.AddNativeKernelParams(nativeKernel, this, userObject, buffers);
			result = OpenCL.EnqueueNativeKernel(this.CommandQueueID,
				CommandQueue.NativeKernelDelegate,
				&callbackId,
				(IntPtr)4,
				0,
				null,
				null,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueNativeKernel failed with error code " + result, result);
		}
		#endregion

		#region EnqueueAcquireGLObjects
		public void EnqueueAcquireGLObjects(Int32 numObjects, Mem[] memObjects, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			IntPtr tmpEvent;
			ErrorCode result;

			result = OpenCL.EnqueueAcquireGLObjects(this.CommandQueueID,
				(UInt32)numObjects,
				InteropTools.ConvertMemToMemIDs(memObjects),
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueAcquireGLObjects failed with error code " + result, result);
			_event = new Event(this.Context, this, tmpEvent);
		}

		public void EnqueueAcquireGLObjects(Int32 numObjects, Mem[] memObjects, Int32 numEventsInWaitList, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueAcquireGLObjects(this.CommandQueueID,
				(UInt32)numObjects,
				InteropTools.ConvertMemToMemIDs(memObjects),
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueAcquireGLObjects failed with error code " + result, result);
		}

		public void EnqueueAcquireGLObjects(Int32 numObjects, Mem[] memObjects) {
			ErrorCode result;

			result = OpenCL.EnqueueAcquireGLObjects(this.CommandQueueID,
				(UInt32)numObjects,
				InteropTools.ConvertMemToMemIDs(memObjects),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueAcquireGLObjects failed with error code " + result, result);
		}
		#endregion

		#region EnqueueReleaseGLObjects
		public void EnqueueReleaseGLObjects(Int32 numObjects, Mem[] memObjects, Int32 numEventsInWaitList, Event[] event_wait_list, out Event _event) {
			IntPtr tmpEvent;
			ErrorCode result;

			result = OpenCL.EnqueueReleaseGLObjects(this.CommandQueueID,
				(UInt32)numObjects,
				InteropTools.ConvertMemToMemIDs(memObjects),
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReleaseGLObjects failed with error code " + result, result);
			_event = new Event(this.Context, this, tmpEvent);
		}

		public void EnqueueReleaseGLObjects(Int32 numObjects, Mem[] memObjects, Int32 numEventsInWaitList, Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueReleaseGLObjects(this.CommandQueueID,
				(UInt32)numObjects,
				InteropTools.ConvertMemToMemIDs(memObjects),
				(UInt32)numEventsInWaitList,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReleaseGLObjects failed with error code " + result, result);
		}

		public void EnqueueReleaseGLObjects(Int32 numObjects, Mem[] memObjects) {
			ErrorCode result;

			result = OpenCL.EnqueueReleaseGLObjects(this.CommandQueueID,
				(UInt32)numObjects,
				InteropTools.ConvertMemToMemIDs(memObjects),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReleaseGLObjects failed with error code " + result, result);
		}
		#endregion

		#region EnqueueWaitForEvents
		public void EnqueueWaitForEvents(Int32 num_events, Event[] _event_list) {
			OpenCL.EnqueueWaitForEvents(this.CommandQueueID, (UInt32)num_events, InteropTools.ConvertEventsToEventIDs(_event_list));
		}

		public void EnqueueWaitForEvent(Event _event) {
			var waitList = new[] { _event };
			this.EnqueueWaitForEvents(1, waitList);
		}
		#endregion

		#region EnqueueReadBufferRect
		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="command_queue"></param>
		/// <param name="buffer"></param>
		/// <param name="blocking_read"></param>
		/// <param name="buffer_offset"></param>
		/// <param name="host_offset"></param>
		/// <param name="region"></param>
		/// <param name="buffer_row_pitch"></param>
		/// <param name="buffer_slice_pitch"></param>
		/// <param name="host_row_pitch"></param>
		/// <param name="host_slice_pitch"></param>
		/// <param name="ptr"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueReadBufferRect(
			Mem buffer,
			Boolean blocking_read,
			IntPtr[] buffer_offset,
			IntPtr[] host_offset,
			IntPtr[] region,
			IntPtr buffer_row_pitch,
			IntPtr buffer_slice_pitch,
			IntPtr host_row_pitch,
			IntPtr host_slice_pitch,
			IntPtr ptr,
			UInt32 num_events_in_wait_list,
			Event[] event_wait_list,
			out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				buffer_offset,
				host_offset,
				region,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(Mem buffer, Boolean blocking_read, Int32[] buffer_offset, Int32[] host_offset, Int32[] region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(Mem buffer, Boolean blocking_read, Int64[] buffer_offset, Int64[] host_offset, Int64[] region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(
			Mem buffer,
			Boolean blocking_read,
			IntPtr[] buffer_offset,
			IntPtr[] host_offset,
			IntPtr[] region,
			IntPtr buffer_row_pitch,
			IntPtr buffer_slice_pitch,
			IntPtr host_row_pitch,
			IntPtr host_slice_pitch,
			IntPtr ptr,
			UInt32 num_events_in_wait_list,
			Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				buffer_offset,
				host_offset,
				region,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(Mem buffer, Boolean blocking_read, Int32[] buffer_offset, Int32[] host_offset, Int32[] region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(Mem buffer, Boolean blocking_read, Int64[] buffer_offset, Int64[] host_offset, Int64[] region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(
			Mem buffer,
			Boolean blocking_read,
			IntPtr[] buffer_offset,
			IntPtr[] host_offset,
			IntPtr[] region,
			IntPtr buffer_row_pitch,
			IntPtr buffer_slice_pitch,
			IntPtr host_row_pitch,
			IntPtr host_slice_pitch,
			IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				buffer_offset,
				host_offset,
				region,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(Mem buffer, Boolean blocking_read, Int32[] buffer_offset, Int32[] host_offset, Int32[] region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}

		public void EnqueueReadBufferRect(Mem buffer, Boolean blocking_read, Int64[] buffer_offset, Int64[] host_offset, Int64[] region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);

			result = OpenCL.EnqueueReadBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_read ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueReadBufferRect failed with error code " + result, result);
		}
		#endregion

		#region EnqueueWriteBufferRect
		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="blocking_write"></param>
		/// <param name="buffer_offset"></param>
		/// <param name="host_offset"></param>
		/// <param name="region"></param>
		/// <param name="buffer_row_pitch"></param>
		/// <param name="buffer_slice_pitch"></param>
		/// <param name="host_row_pitch"></param>
		/// <param name="host_slice_pitch"></param>
		/// <param name="ptr"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueWriteBufferRect(
			Mem buffer,
			Boolean blocking_write,
			IntPtr[] buffer_offset,
			IntPtr[] host_offset,
			IntPtr[] region,
			IntPtr buffer_row_pitch,
			IntPtr buffer_slice_pitch,
			IntPtr host_row_pitch,
			IntPtr host_slice_pitch,
			IntPtr ptr,
			UInt32 num_events_in_wait_list,
			Event[] event_wait_list,
			out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				blocking_write ? 1u : 0u,
				buffer_offset,
				host_offset,
				region,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(Mem buffer, Boolean blocking_write, Int32[] buffer_offset, Int32[] host_offset, Int32[] region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_write ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(Mem buffer, Boolean blocking_write, Int64[] buffer_offset, Int64[] host_offset, Int64[] region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_write ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(
			Mem buffer,
			Boolean blocking_write,
			IntPtr[] buffer_offset,
			IntPtr[] host_offset,
			IntPtr[] region,
			IntPtr buffer_row_pitch,
			IntPtr buffer_slice_pitch,
			IntPtr host_row_pitch,
			IntPtr host_slice_pitch,
			IntPtr ptr,
			UInt32 num_events_in_wait_list,
			Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				blocking_write ? 1u : 0u,
				buffer_offset,
				host_offset,
				region,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);

			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(Mem buffer, Boolean blocking_write, Int32[] buffer_offset, Int32[] host_offset, Int32[] region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_write ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(Mem buffer, Boolean blocking_write, Int64[] buffer_offset, Int64[] host_offset, Int64[] region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_write ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				num_events_in_wait_list,
				pRepackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(
			Mem buffer,
			Boolean blocking_write,
			IntPtr[] buffer_offset,
			IntPtr[] host_offset,
			IntPtr[] region,
			IntPtr buffer_row_pitch,
			IntPtr buffer_slice_pitch,
			IntPtr host_row_pitch,
			IntPtr host_slice_pitch,
			IntPtr ptr) {
			ErrorCode result;

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				blocking_write ? 1u : 0u,
				buffer_offset,
				host_offset,
				region,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(Mem buffer, Boolean blocking_write, Int32[] buffer_offset, Int32[] host_offset, Int32[] region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_write ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}

		public void EnqueueWriteBufferRect(Mem buffer, Boolean blocking_write, Int64[] buffer_offset, Int64[] host_offset, Int64[] region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, IntPtr ptr) {
			ErrorCode result;
			IntPtr* pBufferOffset = stackalloc IntPtr[3];
			IntPtr* pHostOffset = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(buffer_offset, pBufferOffset);
			InteropTools.A3ToIntPtr3(host_offset, pHostOffset);
			InteropTools.A3ToIntPtr3(region, pRegion);

			result = OpenCL.EnqueueWriteBufferRect(this.CommandQueueID,
				buffer.MemID,
				(UInt32)(blocking_write ? Bool.TRUE : Bool.FALSE),
				pBufferOffset,
				pHostOffset,
				pRegion,
				buffer_row_pitch,
				buffer_slice_pitch,
				host_row_pitch,
				host_slice_pitch,
				ptr.ToPointer(),
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueWriteBufferRect failed with error code " + result, result);
		}
		#endregion

		#region EnqueueCopyBufferRect
		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="src_buffer"></param>
		/// <param name="dst_buffer"></param>
		/// <param name="src_origin"></param>
		/// <param name="dst_origin"></param>
		/// <param name="region"></param>
		/// <param name="src_row_pitch"></param>
		/// <param name="src_slice_pitch"></param>
		/// <param name="dst_row_pitch"></param>
		/// <param name="dst_slice_pitch"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		public void EnqueueCopyBufferRect(
			Mem src_buffer,
			Mem dst_buffer,
			IntPtr[] src_origin,
			IntPtr[] dst_origin,
			IntPtr[] region,
			IntPtr src_row_pitch,
			IntPtr src_slice_pitch,
			IntPtr dst_row_pitch,
			IntPtr dst_slice_pitch,
			UInt32 num_events_in_wait_list,
			Event[] event_wait_list,
			out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				src_origin,
				dst_origin,
				region,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(Mem src_buffer, Mem dst_buffer, Int32[] src_origin, Int32[] dst_origin, Int32[] region, Int32 src_row_pitch, Int32 src_slice_pitch, Int32 dst_row_pitch, Int32 dst_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* pSrcOrigin = stackalloc IntPtr[3];
			IntPtr* pDstOrigin = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, pSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, pDstOrigin);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				pSrcOrigin,
				pDstOrigin,
				pRegion,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				num_events_in_wait_list,
				pRepackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(Mem src_buffer, Mem dst_buffer, Int64[] src_origin, Int64[] dst_origin, Int64[] region, Int64 src_row_pitch, Int64 src_slice_pitch, Int64 dst_row_pitch, Int64 dst_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list, out Event _event) {
			ErrorCode result;
			IntPtr tmpEvent;
			IntPtr* pSrcOrigin = stackalloc IntPtr[3];
			IntPtr* pDstOrigin = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, pSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, pDstOrigin);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				pSrcOrigin,
				pDstOrigin,
				pRegion,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				num_events_in_wait_list,
				pRepackedEvents,
				&tmpEvent);
			_event = new Event(this.Context, this, tmpEvent);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(
			Mem src_buffer,
			Mem dst_buffer,
			IntPtr[] src_origin,
			IntPtr[] dst_origin,
			IntPtr[] region,
			IntPtr src_row_pitch,
			IntPtr src_slice_pitch,
			IntPtr dst_row_pitch,
			IntPtr dst_slice_pitch,
			UInt32 num_events_in_wait_list,
			Event[] event_wait_list) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				src_origin,
				dst_origin,
				region,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				num_events_in_wait_list,
				InteropTools.ConvertEventsToEventIDs(event_wait_list),
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(Mem src_buffer, Mem dst_buffer, Int32[] src_origin, Int32[] dst_origin, Int32[] region, Int32 src_row_pitch, Int32 src_slice_pitch, Int32 dst_row_pitch, Int32 dst_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* pSrcOrigin = stackalloc IntPtr[3];
			IntPtr* pDstOrigin = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, pSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, pDstOrigin);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				pSrcOrigin,
				pDstOrigin,
				pRegion,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				num_events_in_wait_list,
				pRepackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(Mem src_buffer, Mem dst_buffer, Int64[] src_origin, Int64[] dst_origin, Int64[] region, Int64 src_row_pitch, Int64 src_slice_pitch, Int64 dst_row_pitch, Int64 dst_slice_pitch, Int32 num_events_in_wait_list, Event[] event_wait_list) {
			ErrorCode result;
			IntPtr* pSrcOrigin = stackalloc IntPtr[3];
			IntPtr* pDstOrigin = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];
			IntPtr* pRepackedEvents = stackalloc IntPtr[num_events_in_wait_list];

			if (num_events_in_wait_list == 0)
				pRepackedEvents = null;
			InteropTools.A3ToIntPtr3(src_origin, pSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, pDstOrigin);
			InteropTools.A3ToIntPtr3(region, pRegion);
			InteropTools.ConvertEventsToEventIDs(num_events_in_wait_list, event_wait_list, pRepackedEvents);

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				pSrcOrigin,
				pDstOrigin,
				pRegion,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				num_events_in_wait_list,
				pRepackedEvents,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(
			Mem src_buffer,
			Mem dst_buffer,
			IntPtr[] src_origin,
			IntPtr[] dst_origin,
			IntPtr[] region,
			IntPtr src_row_pitch,
			IntPtr src_slice_pitch,
			IntPtr dst_row_pitch,
			IntPtr dst_slice_pitch) {
			ErrorCode result;

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				src_origin,
				dst_origin,
				region,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(Mem src_buffer, Mem dst_buffer, Int32[] src_origin, Int32[] dst_origin, Int32[] region, Int32 src_row_pitch, Int32 src_slice_pitch, Int32 dst_row_pitch, Int32 dst_slice_pitch) {
			ErrorCode result;
			IntPtr* pSrcOrigin = stackalloc IntPtr[3];
			IntPtr* pDstOrigin = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(src_origin, pSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, pDstOrigin);
			InteropTools.A3ToIntPtr3(region, pRegion);

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				pSrcOrigin,
				pDstOrigin,
				pRegion,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}

		public void EnqueueCopyBufferRect(Mem src_buffer, Mem dst_buffer, Int64[] src_origin, Int64[] dst_origin, Int64[] region, Int64 src_row_pitch, Int64 src_slice_pitch, Int64 dst_row_pitch, Int64 dst_slice_pitch) {
			ErrorCode result;
			IntPtr* pSrcOrigin = stackalloc IntPtr[3];
			IntPtr* pDstOrigin = stackalloc IntPtr[3];
			IntPtr* pRegion = stackalloc IntPtr[3];

			InteropTools.A3ToIntPtr3(src_origin, pSrcOrigin);
			InteropTools.A3ToIntPtr3(dst_origin, pDstOrigin);
			InteropTools.A3ToIntPtr3(region, pRegion);

			result = OpenCL.EnqueueCopyBufferRect(this.CommandQueueID,
				src_buffer.MemID,
				dst_buffer.MemID,
				pSrcOrigin,
				pDstOrigin,
				pRegion,
				src_row_pitch,
				src_slice_pitch,
				dst_row_pitch,
				dst_slice_pitch,
				0,
				null,
				null);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("EnqueueCopyBufferRect failed with error code " + result, result);
		}
		#endregion
	}
}