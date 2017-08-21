using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenCLNet {
	public class Event : IDisposable, InteropTools.IPropertyContainer {
		private static readonly Dictionary<Int32, CallbackData> CallbackDispatch = new Dictionary<Int32, CallbackData>();
		private static Int32 CallbackId;
		private static readonly Mutex CallbackMutex = new Mutex();
		private static readonly EventNotifyInternal CallbackDelegate = Event.EventCallback;

		// Track whether Dispose has been called.
		private Boolean disposed;

		private static Int32 AddCallback(Event _event, EventNotify userMethod, Object userData) {
			Int32 callbackId;
			var callbackData = new CallbackData(_event, userMethod, userData);
			var gotMutex = false;

			try {
				gotMutex = Event.CallbackMutex.WaitOne();
				do {
					callbackId = Event.CallbackId++;
				} while (Event.CallbackDispatch.ContainsKey(callbackId));
				Event.CallbackDispatch.Add(callbackId, callbackData);
			}
			finally {
				if (gotMutex)
					Event.CallbackMutex.ReleaseMutex();
			}
			return callbackId;
		}

		private static void EventCallback(IntPtr eventId, Int32 executionStatus, IntPtr userData) {
			var callbackId = userData.ToInt32();
			var callbackData = Event.GetCallback(callbackId);
			callbackData.UserMethod(callbackData.EventObject, (ExecutionStatus)executionStatus, callbackData.UserData);
			Event.RemoveCallback(callbackId);
		}

		private static CallbackData GetCallback(Int32 callbackId) {
			CallbackData callbackData = null;
			var gotMutex = false;
			try {
				gotMutex = Event.CallbackMutex.WaitOne();
				callbackData = Event.CallbackDispatch[callbackId];
			}
			finally {
				if (gotMutex)
					Event.CallbackMutex.ReleaseMutex();
			}
			return callbackData;
		}

		private static void RemoveCallback(Int32 callbackId) {
			var gotMutex = false;
			try {
				gotMutex = Event.CallbackMutex.WaitOne();
				Event.CallbackDispatch.Remove(callbackId);
			}
			finally {
				if (gotMutex)
					Event.CallbackMutex.ReleaseMutex();
			}
		}

		public static implicit operator IntPtr(Event _event) {
			return _event.EventID;
		}

		/// <summary>
		///     Returns the specified profiling counter
		/// </summary>
		/// <param name="paramName"></param>
		/// <param name="paramValue"></param>
		public unsafe void GetEventProfilingInfo(ProfilingInfo paramName, out UInt64 paramValue) {
			IntPtr paramValueSizeRet;
			UInt64 v;
			ErrorCode errorCode;

			errorCode = OpenCL.GetEventProfilingInfo(this.EventID, paramName, (IntPtr)sizeof(UInt64), &v, out paramValueSizeRet);
			if (errorCode != ErrorCode.SUCCESS)
				throw new OpenCLException("GetEventProfilingInfo failed with error code " + errorCode, errorCode);
			paramValue = v;
		}

		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="command_exec_callback_type"></param>
		/// <param name="pfn_notify"></param>
		/// <param name="user_data"></param>
		public void SetCallback(ExecutionStatus command_exec_callback_type, EventNotify pfn_notify, Object user_data) {
			ErrorCode result;
			var callbackId = Event.AddCallback(this, pfn_notify, user_data);

			result = OpenCL.SetEventCallback(this.EventID, (Int32)command_exec_callback_type, Event.CallbackDelegate, (IntPtr)callbackId);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetEventCallback failed with error code " + result, result);
		}

		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="_event"></param>
		/// <param name="execution_status"></param>
		public void SetUserEventStatus(ExecutionStatus execution_status) {
			ErrorCode result;

			result = OpenCL.SetUserEventStatus(this.EventID, execution_status);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("SetUserEventStatus failed with error code " + result, result);
		}

		/// <summary>
		///     Block the current thread until this event is completed
		/// </summary>
		public void Wait() {
			this.Context.WaitForEvent(this);
		}

		internal class CallbackData {
			public Event EventObject;
			public Object UserData;
			public EventNotify UserMethod;

			internal CallbackData(Event _event, EventNotify userMethod, Object userData) {
				this.EventObject = _event;
				this.UserMethod = userMethod;
				this.UserData = userData;
			}
		}

		#region Properties
		public IntPtr EventID { get; protected set; }
		public Context Context { get; protected set; }
		public CommandQueue CommandQueue { get; protected set; }

		public ExecutionStatus ExecutionStatus => (ExecutionStatus)InteropTools.ReadUInt(this, (UInt32)EventInfo.COMMAND_EXECUTION_STATUS);

		public CommandType CommandType => (CommandType)InteropTools.ReadUInt(this, (UInt32)EventInfo.COMMAND_TYPE);
		#endregion

		#region Construction / Destruction
		internal Event(Context context, CommandQueue cq, IntPtr eventID) {
			this.Context = context;
			this.CommandQueue = cq;
			this.EventID = eventID;
		}

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~Event() {
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
				OpenCL.ReleaseEvent(this.EventID);
				this.EventID = IntPtr.Zero;

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion

		#region IPropertyContainer Members
		public unsafe IntPtr GetPropertySize(UInt32 key) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetEventInfo(this.EventID, key, IntPtr.Zero, null, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetEventInfo failed; " + result, result);
			return size;
		}

		public unsafe void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			IntPtr size;
			ErrorCode result;

			result = OpenCL.GetEventInfo(this.EventID, key, keyLength, pBuffer, out size);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetEventInfo failed; " + result, result);
		}
		#endregion
	}
}