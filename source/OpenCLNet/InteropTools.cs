using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenCLNet {
	public class InteropTools {
		public static unsafe void A3ToIntPtr3(Int32[] a, IntPtr* b) {
			if (a == null || b == null)
				return;

			b[0] = (IntPtr)a[0];
			b[1] = (IntPtr)a[1];
			b[2] = (IntPtr)a[2];
		}

		public static unsafe void A3ToIntPtr3(Int64[] a, IntPtr* b) {
			if (a == null || b == null)
				return;

			b[0] = (IntPtr)a[0];
			b[1] = (IntPtr)a[1];
			b[2] = (IntPtr)a[2];
		}

		public static unsafe void AToIntPtr(Int32 count, Int32[] a, IntPtr* b) {
			if (a == null || b == null)
				return;

			for (var i = 0; i < count; i++)
				b[i] = (IntPtr)a[i];
		}

		public static unsafe void AToIntPtr(Int32 count, Int64[] a, IntPtr* b) {
			if (a == null || b == null)
				return;

			for (var i = 0; i < count; i++)
				b[i] = (IntPtr)a[i];
		}

		public static Device[] ConvertDeviceIDsToDevices(Platform platform, IntPtr[] deviceIDs) {
			Device[] devices;

			if (deviceIDs == null)
				return null;

			devices = new Device[deviceIDs.Length];
			for (var i = 0; i < deviceIDs.Length; i++)
				devices[i] = platform.GetDevice(deviceIDs[i]);
			return devices;
		}

		public static IntPtr[] ConvertDevicesToDeviceIDs(Device[] devices) {
			IntPtr[] deviceIDs;

			if (devices == null)
				return null;

			deviceIDs = new IntPtr[devices.Length];
			for (var i = 0; i < devices.Length; i++)
				deviceIDs[i] = devices[i];
			return deviceIDs;
		}

		public static IntPtr[] ConvertEventsToEventIDs(Event[] events) {
			IntPtr[] eventIDs;

			if (events == null)
				return null;

			eventIDs = new IntPtr[events.Length];
			for (var i = 0; i < events.Length; i++)
				eventIDs[i] = events[i];
			return eventIDs;
		}

		public unsafe static void ConvertEventsToEventIDs(Int32 num, Event[] events, IntPtr* pHandles) {
			if (events == null)
				return;
			if (num > events.Length)
				throw new ArgumentOutOfRangeException();

			for (var i = 0; i < num; i++)
				pHandles[i] = events[i].EventID;
		}

		public static IntPtr[] ConvertMemToMemIDs(Mem[] mems) {
			IntPtr[] memIDs;

			if (mems == null)
				return null;

			memIDs = new IntPtr[mems.Length];
			for (var i = 0; i < mems.Length; i++)
				memIDs[i] = mems[i].MemID;
			return memIDs;
		}

		public static Byte[] CreateNullTerminatedString(String s) {
			Int32 len;
			Byte[] data;

			len = Encoding.UTF8.GetByteCount(s);
			data = new Byte[len + 1];
			Encoding.UTF8.GetBytes(s, 0, s.Length, data, 0);
			data[len] = 0;
			return data;
		}

		public static void HexDump(String filename, Byte[] array, Int32 width) {
			var sw = new StreamWriter(filename);

			InteropTools.HexDump(sw, array, width);
			sw.Close();
		}

		public static void HexDump(TextWriter tw, Byte[] array, Int32 width) {
			var hexWidth = width * 3;
			var charWidth = width;
			var bytePtr = 0;
			Int32 linePos;

			while (bytePtr < array.Length) {
				var dataLen = Math.Min(width, array.Length - bytePtr);

				tw.Write("{0:x8} ", bytePtr);
				// Output hex
				for (linePos = 0; linePos < dataLen; linePos++) {
					var b = array[bytePtr + linePos];

					tw.Write("{0:x2} ", (Int32)b);
				}
				for (; linePos < width; linePos++)
					tw.Write("   ");

				// Output characters
				for (linePos = 0; linePos < dataLen; linePos++) {
					var b = array[bytePtr + linePos];
					var c = (Char)b;
					if (Char.IsControl(c))
						c = '.';
					tw.Write(c);
				}
				for (; linePos < width; linePos++)
					tw.Write(" ");

				tw.WriteLine();
				bytePtr += dataLen;
			}
		}

		public static unsafe void IntPtrToIntPtr(Int32 num, IntPtr[] a, IntPtr* b) {
			if (a == null)
				return;

			if (num > a.Length)
				throw new ArgumentOutOfRangeException();
			for (var i = 0; i < num; i++)
				b[i] = a[i];
		}

		public unsafe interface IPropertyContainer {
			IntPtr GetPropertySize(UInt32 key);
			void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer);
		}

		#region Helper functions to read properties
		public static Boolean ReadBool(IPropertyContainer propertyContainer, UInt32 key) {
			return InteropTools.ReadUInt(propertyContainer, key) == (UInt32)Bool.TRUE ? true : false;
		}

		unsafe public static Byte[] ReadBytes(IPropertyContainer propertyContainer, UInt32 key) {
			IntPtr size;

			size = propertyContainer.GetPropertySize(key);
			var data = new Byte[size.ToInt64()];
			fixed (Byte* pData = data) {
				propertyContainer.ReadProperty(key, size, pData);
			}
			return data;
		}

		unsafe public static String ReadString(IPropertyContainer propertyContainer, UInt32 key) {
			IntPtr size;
			String s;

			size = propertyContainer.GetPropertySize(key);
			var stringData = new Byte[size.ToInt64()];
			fixed (Byte* pStringData = stringData) {
				propertyContainer.ReadProperty(key, size, pStringData);
			}

			s = Encoding.UTF8.GetString(stringData);
			var nullIndex = s.IndexOf('\0');
			if (nullIndex >= 0)
				return s.Substring(0, nullIndex);
			else
				return s;
		}

		unsafe public static Int32 ReadInt(IPropertyContainer propertyContainer, UInt32 key) {
			Int32 output;

			propertyContainer.ReadProperty(key, new IntPtr(sizeof(Int32)), &output);
			return output;
		}

		unsafe public static UInt32 ReadUInt(IPropertyContainer propertyContainer, UInt32 key) {
			UInt32 output;

			propertyContainer.ReadProperty(key, new IntPtr(sizeof(UInt32)), &output);
			return output;
		}

		unsafe public static Int64 ReadLong(IPropertyContainer propertyContainer, UInt32 key) {
			Int64 output;

			propertyContainer.ReadProperty(key, new IntPtr(sizeof(Int64)), &output);
			return output;
		}

		unsafe public static UInt64 ReadULong(IPropertyContainer propertyContainer, UInt32 key) {
			UInt64 output;

			propertyContainer.ReadProperty(key, new IntPtr(sizeof(UInt64)), &output);
			return output;
		}

		unsafe public static IntPtr ReadIntPtr(IPropertyContainer propertyContainer, UInt32 key) {
			IntPtr output;

			propertyContainer.ReadProperty(key, new IntPtr(sizeof(IntPtr)), &output);
			return output;
		}

		unsafe public static IntPtr[] ReadIntPtrArray(IPropertyContainer propertyContainer, UInt32 key) {
			var size = propertyContainer.GetPropertySize(key);
			var numElements = (Int64)size / sizeof(IntPtr);
			var ptrs = new IntPtr[numElements];
			var data = InteropTools.ReadBytes(propertyContainer, key);

			fixed (Byte* pData = data) {
				var pBS = (void**)pData;
				for (var i = 0; i < numElements; i++)
					ptrs[i] = new IntPtr(pBS[i]);
			}
			return ptrs;
		}

		unsafe public static void ReadPreAllocatedBytePtrArray(IPropertyContainer propertyContainer, UInt32 key, Byte[][] buffers) {
			var pinnedArrays = new GCHandle[buffers.Length];

			// Pin arrays
			for (var i = 0; i < buffers.Length; i++)
				pinnedArrays[i] = GCHandle.Alloc(buffers[i], GCHandleType.Pinned);

			Byte** pointerArray = stackalloc Byte*[buffers.Length];
			for (var i = 0; i < buffers.Length; i++)
				pointerArray[i] = (Byte*)(pinnedArrays[i].AddrOfPinnedObject().ToPointer());

			propertyContainer.ReadProperty(key, new IntPtr(sizeof(IntPtr) * buffers.Length), pointerArray);

			for (var i = 0; i < buffers.Length; i++)
				pinnedArrays[i].Free();
		}
		#endregion
	}
}