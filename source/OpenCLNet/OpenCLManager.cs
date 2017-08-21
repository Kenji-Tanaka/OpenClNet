using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace OpenCLNet {
	/// <summary>
	///     OpenCLManager is a class that provides generic setup, compilation and caching services.
	/// </summary>
	public class OpenCLManager : IDisposable {
		private String CreateRandomDirectory(String path) {
			var tries = 0;

			while (true) {
				var randomFileName = path + this.FileSystem.GetDirectorySeparator() + this.FileSystem.GetRandomFileName();
				try {
					if (!this.FileSystem.DirectoryExists(randomFileName))
						this.FileSystem.CreateDirectory(randomFileName);
					return randomFileName;
				}
				catch (IOException e) {
					if (tries++ > 50) {
						throw e;
					}
					if (!this.FileSystem.DirectoryExists(randomFileName))
						throw e;
				}
			}
		}

		private String CreateRandomFile(String path) {
			var tries = 0;

			while (true) {
				var randomFileName = path + this.FileSystem.GetDirectorySeparator() + this.FileSystem.GetRandomFileName();
				try {
					if (!this.FileSystem.Exists(randomFileName)) {
						var fs = this.FileSystem.Open(randomFileName, FileMode.CreateNew, FileAccess.ReadWrite);
						fs.Close();
						return randomFileName;
					}
				}
				catch (IOException e) {
					if (tries++ > 50)
						throw e;
					if (!this.FileSystem.Exists(randomFileName))
						throw e;
				}
			}
		}

		private void DefaultProperties() {
			this.MaxCachedBinaries = 50;
			this.FileSystem = new OCLManFileSystem();
			this.RequireImageSupport = false;
			this.BuildOptions = "";
			this.Defines = "";
			this.SourcePath = "OpenCL" + this.FileSystem.GetDirectorySeparator() + "src";
			this.BinaryPath = "OpenCL" + this.FileSystem.GetDirectorySeparator() + "bin";
			this.AttemptUseBinaries = true;
			this.AttemptUseSource = true;
		}

		private void TestAndCreateDirectory(String path) {
			if (!this.FileSystem.DirectoryExists(path))
				this.FileSystem.CreateDirectory(path);
		}

		private void TestAndCreateFile(String path) {
			try {
				if (!this.FileSystem.Exists(path)) {
					var fs = this.FileSystem.Open(path, FileMode.Create, FileAccess.ReadWrite);
					fs.Close();
				}
			}
			catch (Exception e) {
				if (!this.FileSystem.Exists(path))
					throw e;
			}
		}

		protected Byte[][] LoadAllBinaries(Context context, String source, String fileName) {
			var sourcePath = this.SourcePath + this.FileSystem.GetDirectorySeparator() + fileName;
			var sourceDateTime = this.FileSystem.GetLastWriteTime(sourcePath);
			var binaries = new Byte[context.Devices.Length][];

			if (!Directory.Exists(this.BinaryPath))
				throw new DirectoryNotFoundException(this.BinaryPath);

			using (var bmi = BinaryMetaInfo.FromPath(this.BinaryPath, FileAccess.Read, FileShare.Read)) {
				Device[] devices;

				devices = context.Devices;
				for (var i = 0; i < devices.Length; i++) {
					if (binaries[i] != null)
						continue;

					var device = devices[i];
					String binaryFilePath;
					var mf = bmi.FindMetaFile("", fileName, this.Context.Platform.Name, device.Name, device.DriverVersion, this.Defines, this.BuildOptions);
					if (mf == null)
						throw new FileNotFoundException("No compiled binary file present in MetaFile");
					binaryFilePath = this.BinaryPath + this.FileSystem.GetDirectorySeparator() + mf.BinaryName;
					if (this.AttemptUseSource) {
						// This exception will be caught inside the manager and cause recompilation
						if (this.FileSystem.GetLastWriteTime(binaryFilePath) < sourceDateTime)
							throw new Exception("Binary older than source");
					}
					binaries[i] = this.FileSystem.ReadAllBytes(binaryFilePath);

					// Check of there are other identical devices that can use the binary we just loaded
					// If there are, patch it in in the proper slots in the list of binaries
					for (var j = i + 1; j < devices.Length; j++) {
						if (devices[i].Name == devices[j].Name &&
							devices[i].Vendor == devices[j].Vendor &&
							devices[i].Version == devices[j].Version &&
							devices[i].AddressBits == devices[j].AddressBits &&
							devices[i].DriverVersion == devices[j].DriverVersion &&
							devices[i].EndianLittle == devices[j].EndianLittle) {
							binaries[j] = binaries[i];
						}
					}
				}
			}
			return binaries;
		}

		protected void SaveAllBinaries(Context context, String source, String fileName, Byte[][] binaries) {
			var xml = new XmlSerializer(typeof(BinaryMetaInfo));
			this.TestAndCreateDirectory(this.BinaryPath);
			using (var bmi = BinaryMetaInfo.FromPath(this.BinaryPath, FileAccess.ReadWrite, FileShare.None)) {
				for (var i = 0; i < context.Devices.Length; i++) {
					var device = context.Devices[i];
					String binaryFileName;

					var mf = bmi.FindMetaFile(source, fileName, context.Platform.Name, device.Name, device.DriverVersion, this.Defines, this.BuildOptions);
					if (mf == null)
						mf = bmi.CreateMetaFile(source, fileName, context.Platform.Name, device.Name, device.DriverVersion, this.Defines, this.BuildOptions);

					binaryFileName = this.BinaryPath + this.FileSystem.GetDirectorySeparator() + mf.BinaryName;
					this.FileSystem.WriteAllBytes(binaryFileName, binaries[i]);
				}
				bmi.TrimBinaryCache(this.FileSystem, this.MaxCachedBinaries);
				bmi.Save();
			}
		}

		protected void SaveDeviceBinary(Context context, String fileName, Byte[][] binaries, String platformDirectoryName, Device device) {
			throw new NotImplementedException("SaveDeviceBinary not implemented");
		}

		/// <summary>
		///     CompileFile
		///     Attempt to compile the file identified by fileName.
		///     If the AttemptUseBinaries property is true, the method will first check if an up-to-date precompiled binary exists.
		///     If it does, it will load the binary instead, if no binary exists, compilation will be performed and the resulting binaries saved.
		///     If the AttemptUseBinaries property is false, only compilation will be attempted.
		///     Caveat: If AttemptUseSource is false, no compilation will be attempted - ever.
		///     If both AttemptUseSource and AttemptUseBinaries are false this function will throw an exception.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public Program CompileFile(String fileName) {
			var sourcePath = this.SourcePath + this.FileSystem.GetDirectorySeparator() + fileName;
			var binaryPath = this.BinaryPath + this.FileSystem.GetDirectorySeparator() + fileName;
			Program p;

			if (!this.FileSystem.Exists(sourcePath))
				throw new FileNotFoundException(sourcePath);

			if (this.AttemptUseBinaries && !this.AttemptUseSource) {
				var binaries = this.LoadAllBinaries(this.Context, "", fileName);
				var status = new ErrorCode[this.Context.Devices.Length];
				p = this.Context.CreateProgramWithBinary(this.Context.Devices, binaries, status);
				p.Build();
				return p;
			}
			else if (!this.AttemptUseBinaries && this.AttemptUseSource) {
				var source = this.Defines + Environment.NewLine + File.ReadAllText(sourcePath);
				p = this.Context.CreateProgramWithSource(source);
				p.Build(this.Context.Devices, this.BuildOptions, null, IntPtr.Zero);
				this.SaveAllBinaries(this.Context, "", fileName, p.Binaries);
				return p;
			}
			else if (this.AttemptUseBinaries && this.AttemptUseSource) {
				try {
					var binaries = this.LoadAllBinaries(this.Context, "", fileName);
					var status = new ErrorCode[this.Context.Devices.Length];
					p = this.Context.CreateProgramWithBinary(this.Context.Devices, binaries, status);
					p.Build();
					return p;
				}
				catch (Exception) {
					// Loading binaries failed for some reason. Attempt to compile instead.
					var source = this.Defines + Environment.NewLine + File.ReadAllText(sourcePath);
					p = this.Context.CreateProgramWithSource(source);
					p.Build(this.Context.Devices, this.BuildOptions, null, IntPtr.Zero);
					this.SaveAllBinaries(this.Context, "", fileName, p.Binaries);
					return p;
				}
			}
			else {
				throw new OpenCLException("OpenCLManager has both AttemptUseBinaries and AttemptUseSource set to false, and therefore can't build Programs from files");
			}
		}

		/// <summary>
		///     CompileSource
		///     Attempt to create a program from a source string and build it.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public Program CompileSource(String source) {
			Program p;

			try {
				var binaries = this.LoadAllBinaries(this.Context, source, "");
				var status = new ErrorCode[this.Context.Devices.Length];
				p = this.Context.CreateProgramWithBinary(this.Context.Devices, binaries, status);
				p.Build();
			}
			catch (Exception) {
				p = this.Context.CreateProgramWithSource(this.Defines + Environment.NewLine + source);
				p.Build(this.Context.Devices, this.BuildOptions, null, IntPtr.Zero);
				this.SaveAllBinaries(this.Context, source, "", p.Binaries);
			}
			return p;
		}

		/// <summary>
		///     Create a context and initialize default command queues in the CQ property
		/// </summary>
		/// <param name="platform"></param>
		/// <param name="devices"></param>
		public void CreateContext(Platform platform, IntPtr[] contextProperties, IEnumerable<Device> devices) {
			this.CreateContext(platform, contextProperties, devices, null, IntPtr.Zero);
		}

		/// <summary>
		///     Create a context and initialize default command queues in the CQ property
		/// </summary>
		/// <param name="platform"></param>
		/// <param name="devices"></param>
		public void CreateContext(Platform platform, IntPtr[] contextProperties, IEnumerable<Device> devices, ContextNotify notify, IntPtr userData) {
			if (!this.OpenCLIsAvailable)
				throw new OpenCLNotAvailableException();

			this.Platform = platform;
			this.Context = platform.CreateContext(contextProperties, devices.ToArray(), notify, userData);
			this.CQ = new CommandQueue[this.Context.Devices.Length];
			for (var i = 0; i < this.Context.Devices.Length; i++)
				this.CQ[i] = this.Context.CreateCommandQueue(this.Context.Devices[0]);
		}

		/// <summary>
		///     Create a context containing all devices in the platform returned by OpenCL.GetPlatform(0) that satisfy the current RequireImageSupport and RequireExtensions property settings.
		///     Default command queues are made available through the CQ property
		/// </summary>
		/// <param name="platform"></param>
		/// <param name="devices"></param>
		public void CreateDefaultContext() {
			this.CreateDefaultContext(0, DeviceType.ALL);
		}

		/// <summary>
		///     Create a context containing all devices of a given type that satisfy the
		///     current RequireImageSupport and RequireExtensions property settings.
		///     Default command queues are made available through the CQ property
		/// </summary>
		/// <param name="deviceType"></param>
		public void CreateDefaultContext(Int32 platformNumber, DeviceType deviceType) {
			if (!this.OpenCLIsAvailable)
				throw new OpenCLNotAvailableException();

			this.Platform = OpenCL.GetPlatform(platformNumber);
			var devices = from d in this.Platform.QueryDevices(deviceType)
						where ((this.RequireImageSupport && d.ImageSupport) || !this.RequireImageSupport) && d.HasExtensions(this.RequiredExtensions.ToArray<String>())
						select d;
			var properties = new[] {
				(IntPtr)ContextProperties.PLATFORM,
				this.Platform,
				IntPtr.Zero
			};

			if (devices.Count() == 0)
				throw new OpenCLException("CreateDefaultContext: No OpenCL devices found that matched filter criteria.");

			this.CreateContext(this.Platform, properties, devices);
		}

		#region Properties
		private Boolean disposed;

		/// <summary>
		///     FileSystem is an instance of the OCLManFileSystem class containing accessor
		///     methods to a file system.
		///     This property has a default implementation that uses normal .Net file access.
		///     However, if one requires OpenCLManager to access a virtual file system,
		///     like a .zip file, or similar. It is possible to subclass OCLManFileSystem
		///     and provide an instance of such an alternate file system implementation through
		///     this property.
		/// </summary>
		public OCLManFileSystem FileSystem { get; set; }

		/// <summary>
		///     True if OpenCL is available on this machine
		/// </summary>
		public Boolean OpenCLIsAvailable {
			get { return OpenCL.NumberOfPlatforms > 0; }
		}

		/// <summary>
		///     Each element in this list is interpreted as the name of an extension.
		///     Any device that does not present this extension in its Extensions
		///     property will be filtered out during context creation.
		/// </summary>
		public List<String> RequiredExtensions = new List<String>();

		/// <summary>
		///     If true, OpenCLManager will filter out any devices that don't signal image support through the HasImageSupport property
		/// </summary>
		public Boolean RequireImageSupport { get; set; }

		/// <summary>
		///     If true, OpenCLManager will attempt to use stored binaries(Stored at 'BinaryPath') to avoid recompilation
		/// </summary>
		public Boolean AttemptUseBinaries { get; set; }

		/// <summary>
		///     The location to store and look for compiled binaries
		/// </summary>
		public String BinaryPath { get; set; }

		/// <summary>
		///     If true, OpenCLManager will attempt to compile sources(Stored at 'SourcePath') to compile programs, and possibly
		///     to store binaries(If 'AttemptUseBinaries' is true)
		/// </summary>
		public Boolean AttemptUseSource { get; set; }

		/// <summary>
		///     The location where sources are stored
		/// </summary>
		public String SourcePath { get; set; }

		public List<DeviceType> DeviceTypes = new List<DeviceType>();

		/// <summary>
		///     BuildOptions is passed to the OpenCL build functions that take compiler options
		/// </summary>
		public String BuildOptions { get; set; }

		/// <summary>
		///     This string is prepended verbatim to any and all sources that are compiled.
		///     It can contain any kind of useful global definitions.
		/// </summary>
		public String Defines { get; set; }
		public Platform Platform;
		public Context Context;

		/// <summary>
		///     Array of CommandQueues. Indices correspond to the devices in Context.Devices.
		///     Simple OpenCL programs will typically just enqueue operations on CQ[0] and ignore any additional devices.
		/// </summary>
		public CommandQueue[] CQ;

		/// <summary>
		///     The maximum number of entries in the binary cache. Default value = 50.
		///     Setting MaxCachedBinaries to a negative number disables cache trimming.
		///     In general, it's ok to disable cache trimming if your OpenCL code is believed to be fairly static.
		///     For example if the sources are "user plugins" or in file form.
		///     However, if you do on-the-fly code generation, it should be set to a reasonable value to avoid excessive disk space consumption.
		/// </summary>
		public Int32 MaxCachedBinaries { get; set; }
		#endregion

		#region Construction/Destruction
		public OpenCLManager() {
			this.DefaultProperties();
		}

		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~OpenCLManager() {
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

				if (this.CQ != null) {
					foreach (var cq in this.CQ)
						cq.Dispose();
					this.CQ = null;
				}

				if (this.Context != null) {
					this.Context.Dispose();
					this.Context = null;
				}

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion
	}

	#region OCLManFileSystem
	public class OCLManFileSystem {
		public virtual void CreateDirectory(String path) {
			Directory.CreateDirectory(path);
		}

		public virtual void Delete(String path) {
			File.Delete(path);
		}

		public virtual Boolean DirectoryExists(String path) {
			return Directory.Exists(path);
		}

		public virtual Boolean Exists(String path) {
			return File.Exists(path);
		}

		/// <summary>
		///     Returns a DateTime object referring to the time of creation
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual DateTime GetCreationTime(String path) {
			return File.GetCreationTime(path);
		}

		/// <summary>
		///     Return the names of the directories in path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual String[] GetDirectories(String path) {
			return Directory.GetDirectories(path);
		}

		/// <summary>
		///     Returns the directory separator character
		/// </summary>
		/// <returns></returns>
		public virtual Char GetDirectorySeparator() {
			return Path.DirectorySeparatorChar;
		}

		/// <summary>
		///     Return the names of the files in path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual String[] GetFiles(String path) {
			return Directory.GetFiles(path);
		}

		/// <summary>
		///     Returns a DateTime object referring to the time of the last write.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual DateTime GetLastWriteTime(String path) {
			return File.GetLastWriteTime(path);
		}

		public virtual String GetRandomFileName() {
			return Path.GetRandomFileName();
		}

		/// <summary>
		///     Returns the root of the filesystem, say "/", "c:\" or something.
		///     "" means the root is the current directory in the default implementation.
		/// </summary>
		public virtual String GetRoot() {
			return "";
		}

		/// <summary>
		///     Return true if this file system is read only.
		///     The implied consequences of this function returning true are that CreateDirectory
		///     and WriteAll functions won't work
		/// </summary>
		/// <returns></returns>
		public virtual Boolean IsReadOnly() {
			return false;
		}

		public FileStream Open(String path, FileMode mode, FileAccess access) {
			return this.Open(path, mode, access, FileShare.None);
		}

		public virtual FileStream Open(String path, FileMode mode, FileAccess access, FileShare share) {
			return File.Open(path, mode, access, share);
		}

		/// <summary>
		///     Returns the entire contents of the file as a byte array
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual Byte[] ReadAllBytes(String path) {
			return File.ReadAllBytes(path);
		}

		/// <summary>
		///     Return the entire contents of the file as a text string using default encoding
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual String ReadAllText(String path) {
			return File.ReadAllText(path);
		}

		/// <summary>
		///     Returns the entire contents of the file as a text string using te specified encoding
		/// </summary>
		/// <param name="path"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public virtual String ReadAllText(String path, Encoding encoding) {
			return File.ReadAllText(path, encoding);
		}

		public virtual void WriteAllBytes(String path, Byte[] bytes) {
			File.WriteAllBytes(path, bytes);
		}

		public virtual void WriteAllText(String path, String text) {
			File.WriteAllText(path, text);
		}

		public virtual void WriteAllText(String path, String text, Encoding encoding) {
			File.WriteAllText(path, text, encoding);
		}
	}
	#endregion

	[Serializable]
	public class BinaryMetaInfo : IDisposable {
		// Track whether Dispose has been called.
		private Boolean disposed;
		private readonly Random Random = new Random();
		internal FileStream FileStream;
		public List<MetaFile> MetaFiles = new List<MetaFile>();

		[DefaultValue("")]
		public String MetaFileName {
			get { return this.Root + Path.DirectorySeparatorChar + "metainfo.xml"; }
		}

		[XmlIgnore]
		[DefaultValue("")]
		public String Root { get; set; }

		public static BinaryMetaInfo FromPath(String path, FileAccess fileAccess, FileShare fileShare) {
			var rnd = new Random();
			BinaryMetaInfo bmi;
			var xml = new XmlSerializer(typeof(BinaryMetaInfo));
			var metaFileName = path + Path.DirectorySeparatorChar + "metainfo.xml";

			if (File.Exists(metaFileName)) {
				var obtainLockStart = DateTime.Now;
				FileStream fs = null;
				while (true) {
					var obtainLockNow = DateTime.Now;
					var dt = obtainLockNow - obtainLockStart;
					if (dt.TotalSeconds > 30)
						break;

					try {
						fs = File.Open(metaFileName, FileMode.Open, fileAccess, fileShare);
						break;
					}
					catch (Exception) {
						Thread.CurrentThread.Join(50 + rnd.Next(50));
					}
				}
				var xmlReader = XmlReader.Create(fs);
				try {
					bmi = (BinaryMetaInfo)xml.Deserialize(xmlReader);
					bmi.FileStream = fs;
					bmi.Root = path;
					xmlReader.Close();
				}
				catch (Exception) {
					xmlReader.Close();
					bmi = new BinaryMetaInfo();
					bmi.Root = path;
					bmi.FileStream = fs;
				}
			}
			else {
				FileStream fs = null;
				try {
					fs = File.Open(metaFileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
				}
				catch (Exception) {
					if (File.Exists(metaFileName)) {
						// Another process created the file just before us. Just call ourselves recursively,
						// which should land us in the other branch of the if-statement now that the metaFile exists
						return BinaryMetaInfo.FromPath(path, fileAccess, fileShare);
					}
				}

				bmi = new BinaryMetaInfo();
				bmi.Root = path;
				bmi.FileStream = fs;
			}
			return bmi;
		}

		public MetaFile CreateMetaFile(String source, String sourceName, String platform, String device, String driverVersion, String defines, String buildOptions) {
			MetaFile mf = null;
			while (true) {
				var randomFileName = Path.GetRandomFileName();
				try {
					var fs = File.Open(this.Root + Path.DirectorySeparatorChar + randomFileName, FileMode.CreateNew, FileAccess.ReadWrite);
					fs.Close();
					mf = new MetaFile(source, sourceName, platform, device, driverVersion, defines, buildOptions, randomFileName);
					this.MetaFiles.Add(mf);
					break;
				}
				catch (Exception) {
					Thread.CurrentThread.Join(50 + this.Random.Next(50));
				}
			}
			return mf;
		}

		public void Exists(String sourceName, String defines, String buildOptions) {
			this.MetaFiles.Exists(file => file.SourceName == sourceName && file.Defines == defines && file.BuildOptions == buildOptions);
		}

		public MetaFile FindMetaFile(String source, String sourceName, String platform, String device, String driverVersion, String defines, String buildOptions) {
			return this.MetaFiles.Find(file => file.Source == source && file.SourceName == sourceName && file.Platform == platform && file.Device == device && file.DriverVersion == driverVersion && file.Defines == defines && file.BuildOptions == buildOptions);
		}

		public void Save() {
			var xml = new XmlSerializer(typeof(BinaryMetaInfo));
			this.FileStream.SetLength(0L);
			var xmlWriter = XmlWriter.Create(this.FileStream);
			xml.Serialize(xmlWriter, this);
			xmlWriter.Close();
		}

		/// <summary>
		///     Delete excess items in MetaFiles
		/// </summary>
		public void TrimBinaryCache(OCLManFileSystem fileSystem, Int32 size) {
			if (size < 0)
				return;

			while (this.MetaFiles.Count > size && this.MetaFiles.Count > 0) {
				var mf = this.MetaFiles[0];
				fileSystem.Delete(this.Root + Path.DirectorySeparatorChar + mf.BinaryName);
				this.MetaFiles.RemoveAt(0);
			}
		}

		#region Construction / Destruction
		// Use C# destructor syntax for finalization code.
		// This destructor will run only if the Dispose method
		// does not get called.
		// It gives your base class the opportunity to finalize.
		// Do not provide destructors in types derived from this class.
		~BinaryMetaInfo() {
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
				this.FileStream.Dispose();
				this.FileStream = null;

				// Note disposing has been done.
				this.disposed = true;
			}
		}
		#endregion
	}

	[Serializable]
	public class MetaFile {
		[DefaultValue("")]
		public String BinaryName { get; set; }
		[DefaultValue("")]
		public String BuildOptions { get; set; }
		[DefaultValue("")]
		public String Defines { get; set; }
		[DefaultValue("")]
		public String Device { get; set; }
		[DefaultValue("")]
		public String DriverVersion { get; set; }
		[DefaultValue("")]
		public String Platform { get; set; }
		[DefaultValue("")]
		public String Source { get; set; }
		[DefaultValue("")]
		public String SourceName { get; set; }

		public MetaFile() {
			this.Source = "";
			this.SourceName = "";
			this.Platform = "";
			this.Device = "";
			this.DriverVersion = "";
			this.Defines = "";
			this.BuildOptions = "";
			this.BinaryName = "";
		}

		public MetaFile(String source, String sourceName, String platform, String device, String driverVersion, String defines, String buildOptions, String binaryName) {
			if (source != null)
				this.Source = source;
			else
				this.Source = "";

			if (sourceName != null)
				this.SourceName = sourceName;
			else
				this.SourceName = "";

			if (platform != null)
				this.Platform = platform;
			else
				this.Platform = "";

			if (device != null)
				this.Device = device;
			else
				this.Device = "";

			if (driverVersion != null)
				this.DriverVersion = driverVersion;
			else
				this.DriverVersion = "";

			if (defines != null)
				this.Defines = defines;
			else
				this.Defines = "";

			if (buildOptions != null)
				this.BuildOptions = buildOptions;
			else
				this.BuildOptions = "";

			if (binaryName != null)
				this.BinaryName = binaryName;
			else
				this.BinaryName = "";
		}
	}
}