using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenCLNet {
	public unsafe class Platform : InteropTools.IPropertyContainer {
		readonly IntPtr[] DeviceIDs;
		Device[] DeviceList;

		readonly Regex VersionStringRegex = new Regex("OpenCL (?<Major>[0-9]+)\\.(?<Minor>[0-9]+)");
		protected Dictionary<IntPtr, Device> _Devices = new Dictionary<IntPtr, Device>();

		protected HashSet<String> ExtensionHashSet = new HashSet<String>();

		/// <summary>
		///     Space separated string of extension names.
		///     Note that this class has some support functions to help query extension capbilities.
		///     This property is only present for completeness.
		/// </summary>
		public String Extensions => InteropTools.ReadString(this, (UInt32)PlatformInfo.EXTENSIONS);

		/// <summary>
		///     Platform name string
		/// </summary>
		public String Name => InteropTools.ReadString(this, (UInt32)PlatformInfo.NAME);

		/// <summary>
		///     Convenience method to get at the major_version field in the Version string
		/// </summary>
		public Int32 OpenCLMajorVersion { get; protected set; }
		/// <summary>
		///     Convenience method to get at the minor_version field in the Version string
		/// </summary>
		public Int32 OpenCLMinorVersion { get; protected set; }
		public IntPtr PlatformID { get; protected set; }

		/// <summary>
		///     Equal to "FULL_PROFILE" if the implementation supports the OpenCL specification or
		///     "EMBEDDED_PROFILE" if the implementation supports the OpenCL embedded profile.
		/// </summary>
		public String Profile => InteropTools.ReadString(this, (UInt32)PlatformInfo.PROFILE);

		/// <summary>
		///     Platform Vendor string
		/// </summary>
		public String Vendor => InteropTools.ReadString(this, (UInt32)PlatformInfo.VENDOR);

		/// <summary>
		///     OpenCL version string. Returns the OpenCL version supported by the implementation. This version string
		///     has the following format: OpenCL&lt;space&gt;&lt;major_version.minor_version&gt;&lt;space&gt;&lt;platform specific information&gt;
		/// </summary>
		public String Version => InteropTools.ReadString(this, (UInt32)PlatformInfo.VERSION);

		public Platform(IntPtr platformID) {
			this.PlatformID = platformID;

			// Create a local representation of all devices
			this.DeviceIDs = this.QueryDeviceIntPtr(DeviceType.ALL);
			for (var i = 0; i < this.DeviceIDs.Length; i++)
				this._Devices[this.DeviceIDs[i]] = new Device(this, this.DeviceIDs[i]);
			this.DeviceList = InteropTools.ConvertDeviceIDsToDevices(this, this.DeviceIDs);

			this.InitializeExtensionHashSet();

			var m = this.VersionStringRegex.Match(this.Version);
			if (m.Success) {
				this.OpenCLMajorVersion = Int32.Parse(m.Groups["Major"].Value);
				this.OpenCLMinorVersion = Int32.Parse(m.Groups["Minor"].Value);
			}
			else {
				this.OpenCLMajorVersion = 1;
				this.OpenCLMinorVersion = 0;
			}
		}

		public static implicit operator IntPtr(Platform p) {
			return p.PlatformID;
		}

		protected void InitializeExtensionHashSet() {
			var ext = this.Extensions.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var s in ext)
				this.ExtensionHashSet.Add(s);
		}

		protected IntPtr[] QueryDeviceIntPtr(DeviceType deviceType) {
			ErrorCode result;
			UInt32 numberOfDevices;
			IntPtr[] deviceIDs;

			result = OpenCL.GetDeviceIDs(this.PlatformID, deviceType, 0, null, out numberOfDevices);
			if (result == ErrorCode.DEVICE_NOT_FOUND)
				return new IntPtr[0];

			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetDeviceIDs failed: " + result, result);

			deviceIDs = new IntPtr[numberOfDevices];
			result = OpenCL.GetDeviceIDs(this.PlatformID, deviceType, numberOfDevices, deviceIDs, out numberOfDevices);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("GetDeviceIDs failed: " + result, result);

			return deviceIDs;
		}

		public Context CreateContext(IntPtr[] contextProperties, Device[] devices, ContextNotify notify, IntPtr userData) {
			IntPtr contextID;
			ErrorCode result;

			var deviceIDs = InteropTools.ConvertDevicesToDeviceIDs(devices);
			contextID = OpenCL.CreateContext(contextProperties,
				(UInt32)deviceIDs.Length,
				deviceIDs,
				notify,
				userData,
				out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateContext failed with error code: " + result, result);
			return new Context(this, contextID);
		}

		public Context CreateContextFromType(IntPtr[] contextProperties, DeviceType deviceType, ContextNotify notify, IntPtr userData) {
			IntPtr contextID;
			ErrorCode result;

			contextID = OpenCL.CreateContextFromType(contextProperties,
				deviceType,
				notify,
				userData,
				out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateContextFromType failed with error code: " + result, result);
			return new Context(this, contextID);
		}

		public Context CreateDefaultContext() {
			return this.CreateDefaultContext(null, IntPtr.Zero);
		}

		public Context CreateDefaultContext(ContextNotify notify, IntPtr userData) {
			var properties = new[] {
				new IntPtr((Int64)ContextProperties.PLATFORM),
				this.PlatformID,
				IntPtr.Zero
			};

			IntPtr contextID;
			ErrorCode result;

			contextID = OpenCL.CreateContext(properties,
				(UInt32)this.DeviceIDs.Length, this.DeviceIDs,
				notify,
				userData,
				out result);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("CreateContext failed with error code: " + result, result);
			return new Context(this, contextID);
		}

		public Device GetDevice(IntPtr index) {
			return this._Devices[index];
		}

		/// <summary>
		///     Test if this platform supports a specific extension
		/// </summary>
		/// <param name="extension"></param>
		/// <returns>Returns true if the extension is supported</returns>
		public Boolean HasExtension(String extension) {
			return this.ExtensionHashSet.Contains(extension);
		}

		/// <summary>
		///     Test if this platform supports a set of exentions
		/// </summary>
		/// <param name="extensions"></param>
		/// <returns>Returns true if all the extensions are supported</returns>
		public Boolean HasExtensions(String[] extensions) {
			foreach (var s in extensions)
				if (!this.ExtensionHashSet.Contains(s))
					return false;
			return true;
		}

		/// <summary>
		///     Find all devices of a specififc type
		/// </summary>
		/// <param name="deviceType"></param>
		/// <returns>Array containing the devices</returns>
		public Device[] QueryDevices(DeviceType deviceType) {
			IntPtr[] deviceIDs;

			deviceIDs = this.QueryDeviceIntPtr(deviceType);
			return InteropTools.ConvertDeviceIDsToDevices(this, deviceIDs);
		}

		#region IPropertyContainer Members
		public IntPtr GetPropertySize(UInt32 key) {
			IntPtr propertySize;
			ErrorCode result;

			result = OpenCL.GetPlatformInfo(this.PlatformID, key, IntPtr.Zero, null, out propertySize);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("Unable to get platform info for platform " + this.PlatformID + ": " + result, result);
			return propertySize;
		}

		public void ReadProperty(UInt32 key, IntPtr keyLength, void* pBuffer) {
			IntPtr propertySize;
			ErrorCode result;

			result = OpenCL.GetPlatformInfo(this.PlatformID, key, keyLength, pBuffer, out propertySize);
			if (result != ErrorCode.SUCCESS)
				throw new OpenCLException("Unable to get platform info for platform " + this.PlatformID + ": " + result, result);
		}
		#endregion
	}
}