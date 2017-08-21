using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace OpenCLNet {
	[Serializable]
	public class BinaryMetaInfo : IDisposable {
		// Track whether Dispose has been called.
		private Boolean disposed;
		private readonly Random Random = new Random();
		internal FileStream FileStream;
		public List<MetaFile> MetaFiles = new List<MetaFile>();

		[DefaultValue("")]
		public String MetaFileName => this.Root + Path.DirectorySeparatorChar + "metainfo.xml";

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
}