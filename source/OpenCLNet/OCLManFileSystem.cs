using System;
using System.IO;
using System.Text;

namespace OpenCLNet {
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
}