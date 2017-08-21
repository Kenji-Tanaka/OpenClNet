using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OpenCLNet;

namespace UnitTests {
	public unsafe partial class Form1 : Form {
		Int32 NativeKernelCalled;
		NativeKernel NativeKernelCallRef;
		readonly OpenCLManager OpenCLManager = new OpenCLManager();
		readonly Regex ParseOpenCLVersion = new Regex(@"OpenCL (?<MajorVersion>\d+)\.(?<MinorVersion>\d+).*");
		Platform[] Platforms;

		public Form1() {
			this.InitializeComponent();
		}

		private Boolean CompareArray(Byte[] a, Byte[] b) {
			if (a.Length != b.Length)
				return false;
			for (var i = 0; i < a.Length; i++) {
				if (a[i] != b[i])
					return false;
			}
			return true;
		}

		private void ContextNotifyFunc(String errInfo, Byte[] privateInfo, IntPtr cb, IntPtr userData) {
			this.Error(errInfo);
		}

		private void Error(String s) {
			this.listBoxErrors.Items.Add(s);
			this.listBoxOutput.SelectedIndex = this.listBoxOutput.Items.Count - 1;
			Application.DoEvents();
		}

		private void Form1_Load(Object sender, EventArgs e) { }

		private void Output(String s) {
			this.listBoxOutput.Items.Add(s);
			this.listBoxOutput.SelectedIndex = this.listBoxOutput.Items.Count - 1;
			Application.DoEvents();
		}

		private void RunTests() {
			this.TestOpenCLClass();
			this.TestOpenCLManager();
		}

		private void startToolStripMenuItem_Click(Object sender, EventArgs e) {
			try {
				this.listBoxErrors.Items.Clear();
				this.listBoxWarnings.Items.Clear();
				this.listBoxOutput.Items.Clear();
				this.RunTests();
				this.Output("Unit testing complete");
			}
			catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Test terminated with a fatal exception.");
			}
		}

		#region TestBufferRectFunctions
		/// <summary>
		///     Test all versions of:
		///     EnqueueReadBufferRect
		///     EnqueueWriteBufferRect
		///     EnqueueCopyBufferRect
		///     The test just copies the entirety of a buffer and checks if the result is equal to the original.
		///     An error indicates that one of the above functions failed and further manual analysis is required
		///     to pinpoint the error.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="cq"></param>
		private void TestBufferRectFunctions(Context c, CommandQueue cq) {
			if (!(cq.Device.Platform.OpenCLMajorVersion >= 1 && cq.Device.Platform.OpenCLMinorVersion >= 1)) {
				this.Output("Skipping EnqueueReadBufferRect, EnqueueWriteBufferRect and EnqueueCopyBufferRect tests(Requires OpenCL 1.1 or higher)");
				return;
			}

			this.Output("Testing EnqueueReadBufferRect, EnqueueWriteBufferRect and EnqueueCopyBufferRect");

			Mem mem0 = null;
			Mem mem1 = null;
			var bufWidth = 16;
			var bufHeight = 16;
			var bufLen = bufWidth * bufHeight;
			var srcData = new Byte[bufLen];
			var cmpData = new Byte[bufLen];
			Event event0;
			Event event1;
			Event event2;
			Event event3;
			Event event4;
			Event event5;

			Array.Clear(srcData, 0, srcData.Length);
			for (var i = 8; i < 12; i++)
				for (var j = 8; j < 12; j++)
					srcData[bufWidth * i + j] = 1;
			Array.Clear(cmpData, 0, cmpData.Length);

			try {
				mem0 = c.CreateBuffer(MemFlags.READ_WRITE, bufLen, IntPtr.Zero);
				mem1 = c.CreateBuffer(MemFlags.READ_WRITE, bufLen, IntPtr.Zero);

				fixed (Byte* pSrc = srcData) {
					fixed (Byte* pCmp = cmpData) {
						{
							var bufferOffset = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var hostOffset = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var region = new IntPtr[3] { (IntPtr)bufWidth, (IntPtr)bufHeight, (IntPtr)1 };
							var bufferRowPitch = (IntPtr)bufWidth;
							var bufferSlicePitch = (IntPtr)0;
							var hostRowPitch = (IntPtr)bufWidth;
							var hostSlicePitch = (IntPtr)0;

							cq.EnqueueWriteBufferRect(mem0, true, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch);
							cq.Finish();
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, true, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (IntPtr version)Copy not identical to source when using no event args");

							cq.EnqueueWriteBufferRect(mem0, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueWaitForEvent(event0);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, 0, null, out event1);
							cq.EnqueueWaitForEvent(event1);
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp, 0, null, out event2);
							cq.Finish();
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (IntPtr version)Copy not identical to source when using event output and no event args");

							var events = new[] { event0, event1, event2 };
							cq.EnqueueWriteBufferRect(mem0, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueWaitForEvent(event3);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, 3, events, out event4);
							cq.EnqueueWaitForEvent(event4);
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp, 3, events, out event5);
							cq.Finish();
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (IntPtr version)Copy not identical to source when using event output and event args");
							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}
						{
							var bufferOffset = new Int32[3] { 0, 0, 0 };
							var hostOffset = new Int32[3] { 0, 0, 0 };
							var region = new Int32[3] { bufWidth, bufHeight, 1 };
							var bufferRowPitch = bufWidth;
							var bufferSlicePitch = 0;
							var hostRowPitch = bufWidth;
							var hostSlicePitch = 0;

							cq.EnqueueWriteBufferRect(mem0, true, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch);
							cq.Finish();
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, true, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (int version)Copy not identical to source when using no event args");

							cq.EnqueueWriteBufferRect(mem0, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueWaitForEvent(event0);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, 0, null, out event1);
							cq.EnqueueWaitForEvent(event1);
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp, 0, null, out event2);
							cq.Finish();
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (int version)Copy not identical to source when using event output and no event args");

							var events = new[] { event0, event1, event2 };
							cq.EnqueueWriteBufferRect(mem0, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueWaitForEvent(event3);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, 3, events, out event4);
							cq.EnqueueWaitForEvent(event4);
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp, 3, events, out event5);
							cq.Finish();
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (int version)Copy not identical to source when using event output and event args");
							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}
						{
							var bufferOffset = new Int64[3] { 0L, 0L, 0L };
							var hostOffset = new Int64[3] { 0L, 0L, 0L };
							var region = new Int64[3] { bufWidth, bufHeight, 1 };
							Int64 bufferRowPitch = bufWidth;
							Int64 bufferSlicePitch = 0;
							Int64 hostRowPitch = bufWidth;
							Int64 hostSlicePitch = 0;

							cq.EnqueueWriteBufferRect(mem0, true, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch);
							cq.Finish();
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, true, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (long version)Copy not identical to source when using no event args");

							cq.EnqueueWriteBufferRect(mem0, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueWaitForEvent(event0);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, 0, null, out event1);
							cq.EnqueueWaitForEvent(event1);
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp, 0, null, out event2);
							cq.Finish();
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (long version)Copy not identical to source when using event output and no event args");

							var events = new[] { event0, event1, event2 };
							cq.EnqueueWriteBufferRect(mem0, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueWaitForEvent(event3);
							cq.EnqueueCopyBufferRect(mem0, mem1, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, 3, events, out event4);
							cq.EnqueueWaitForEvent(event4);
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueReadBufferRect(mem1, false, bufferOffset, hostOffset, region, bufferRowPitch, bufferSlicePitch, hostRowPitch, hostSlicePitch, (IntPtr)pCmp, 3, events, out event5);
							cq.Finish();
							if (!this.CompareArray(cmpData, srcData))
								this.Error("Read-/Write-/CopyRect: (long version)Copy not identical to source when using event output and event args");
							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}
					}
				}
			}
			catch (Exception e) {
				this.Error("Exception during testing: " + e);
			}
			finally {
				if (mem0 != null)
					mem0.Dispose();
				if (mem1 != null)
					mem1.Dispose();
			}
		}
		#endregion

		private void TestCommandQueue(Context c, CommandQueue cq) {
			var programName = "OpenCL" + Path.DirectorySeparatorChar + "src" + Path.DirectorySeparatorChar + "MemoryTests.cl";

			this.Output("Testing compilation of: " + programName);
			var p0 = c.CreateProgramWithSource(File.ReadAllLines(programName));
			var p = c.CreateProgramWithSource(File.ReadAllText(programName));
			p0.Build();
			p.Build();
			var k = p.CreateKernel(@"LoopAndDoNothing");

			this.TestCommandQueueMemCopy(c, cq);
			this.TestCommandQueueAsync(c, cq, k);
		}

		private void TestContext(Context c) {
			var devices = c.Devices;
			var p = c.CreateProgramFromFile("OpenCL" + Path.DirectorySeparatorChar + "src" + Path.DirectorySeparatorChar + "MemoryTests.cl");
			Dictionary<String, Kernel> kernelDictionary;

			try {
				p.Build();
			}
			catch (OpenCLException ocle) {
				throw ocle;
			}
			kernelDictionary = p.CreateKernelDictionary();
			this.NativeKernelCallRef = this.NativeKernelTest;
			for (var deviceIndex = 0; deviceIndex < devices.Length; deviceIndex++) {
				Device d;

				d = devices[deviceIndex];
				using (var cq = c.CreateCommandQueue(d)) {
					if (d.ExecutionCapabilities.HasFlag(DeviceExecCapabilities.NATIVE_KERNEL)) {
						this.Output("Testing native kernel execution");
						cq.EnqueueNativeKernel(this.NativeKernelCallRef, this, null);
						cq.Finish();
						if (this.NativeKernelCalled != 1)
							this.Error("EnqueueNativeKernel failed");
						Interlocked.Decrement(ref this.NativeKernelCalled);
					}
					else {
						this.Output("Testing native kernel execution: Not supported");
					}

					this.TestMem(c, cq, kernelDictionary);
					this.TestDevice(d);
					this.TestCommandQueue(c, cq);
					this.TestKernel(c, cq, kernelDictionary["ArgIO"]);
					this.TestUserEventCallbacks(c, cq);
					this.TestVecKernel(c, cq, kernelDictionary["TestVectorFloat2"]);
				}
			}
		}

		private void TestDevice(Device d) {
			this.Output("");
			this.Output("Testing device: \"" + d.Name + "\"");
			// d.ToString() is overloaded to output all properties as a string, so every property will be used that way
			this.Output(d.ToString());
		}

		#region TestEnqueueNDRangeKernel
		/// <summary>
		///     Test all versions of:
		///     EnqueueNDRangeKernel
		///     The tests just issue a dummy kernel a bunch of times with the various overloads
		/// </summary>
		/// <param name="c"></param>
		/// <param name="cq"></param>
		private void TestEnqueueNDRangeKernel(Context c, CommandQueue cq, Kernel k) {
			this.Output("Testing EnqueueNDRangeKernel");

			Event event0 = null;
			Event event1 = null;

			try {
				{
					var globalWorkSize = new[] { (IntPtr)10 };
					var localWorkSize = new[] { (IntPtr)1 };
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize);
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize, 0, null, out event0);
					var waitList = new[] { event0 };
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize, 1, waitList, out event1);
					cq.Finish();
					event0.Dispose();
					event1.Dispose();
				}
				{
					var globalWorkSize = new[] { 10 };
					var localWorkSize = new[] { 1 };
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize);
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize, 0, null, out event0);
					var waitList = new[] { event0 };
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize, 1, waitList, out event1);
					cq.Finish();
					event0.Dispose();
					event1.Dispose();
				}
				{
					var globalWorkSize = new[] { (Int64)10 };
					var localWorkSize = new[] { (Int64)1 };
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize);
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize, 0, null, out event0);
					var waitList = new[] { event0 };
					cq.EnqueueNDRangeKernel(k, 1, null, globalWorkSize, localWorkSize, 1, waitList, out event1);
					cq.Finish();
					event0.Dispose();
					event1.Dispose();
				}
			}
			catch (Exception e) {
				this.Error("Exception during testing: " + e);
			}
			finally {
				if (event0 != null)
					event0.Dispose();
				if (event1 != null)
					event1.Dispose();
			}
		}
		#endregion

		#region TestImageReadWriteCopyOps
		/// <summary>
		///     Test all versions of:
		///     EnqueueWriteImage
		///     EnqueueReadImage
		///     EnqueueCopyImage
		///     The test just copies the entirety of a buffer and checks if the result is equal to the original.
		///     An error indicates that one of the above functions failed and further manual analysis is required
		///     to pinpoint the error.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="cq"></param>
		private void TestImageReadWriteCopyOps(Context c, CommandQueue cq) {
			if (!cq.Device.ImageSupport) {
				this.Output("Skipping image read/write/copy tests(not supported on this device)");
				return;
			}

			this.Output("Testing image read/write/copy functions");

			Image img0 = null;
			Image img1 = null;
			Image img2 = null;
			var imgWidth = 1024;
			var imgHeight = 1024;
			var bufLen = imgWidth * 4 * imgHeight;
			var srcData = new Byte[bufLen];
			var cmpData = new Byte[bufLen];
			Event event0;
			Event event1;
			Event event2;
			Event event3;
			Event event4;
			Event event5;

			for (var i = 0; i < srcData.Length; i++)
				srcData[i] = (Byte)(i);
			Array.Clear(cmpData, 0, cmpData.Length);

			try {
				img0 = c.CreateImage2D(MemFlags.READ_WRITE, ImageFormat.RGBA8U, imgWidth, imgHeight);
				img1 = c.CreateImage2D(MemFlags.READ_WRITE, ImageFormat.RGBA8U, imgWidth, imgHeight);
				img2 = c.CreateImage2D(MemFlags.READ_WRITE, ImageFormat.RGBA8U, imgWidth, imgHeight);

				Array.Clear(cmpData, 0, cmpData.Length);
				fixed (Byte* pSrc = srcData) {
					fixed (Byte* pCmp = cmpData) {
						{
							var origin = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var region = new IntPtr[3] { (IntPtr)imgWidth, (IntPtr)imgHeight, (IntPtr)1 };
							var dstOrigin = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var dstRegion = new IntPtr[3] { (IntPtr)imgWidth, (IntPtr)imgHeight, (IntPtr)1 };

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pSrc);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (IntPtr version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 0, null, out event1);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pCmp, 0, null, out event2);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (IntPtr version)Copy not identical to source with event output and no wait list");

							Array.Clear(cmpData, 0, cmpData.Length);
							var events = new[] { event0, event1, event2 };
							cq.EnqueueWriteImage(img0, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pSrc, 3, events);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 3, events);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pCmp, 3, events);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (IntPtr version)Copy not identical to source using no event output and a wait list");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 3, events, out event4);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, IntPtr.Zero, IntPtr.Zero, (IntPtr)pCmp, 3, events, out event5);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (IntPtr version)Copy not identical to source using event output and a wait list");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}
						{
							var origin = new Int32[3] { 0, 0, 0 };
							var region = new Int32[3] { imgWidth, imgHeight, 1 };
							var dstOrigin = new Int32[3] { 0, 0, 0 };
							var dstRegion = new Int32[3] { imgWidth, imgHeight, 1 };

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, 0, 0, (IntPtr)pSrc);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0, 0, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (int version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, 0, 0, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 0, null, out event1);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0, 0, (IntPtr)pCmp, 0, null, out event2);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (int version)Copy not identical to source with event output and no wait list");

							Array.Clear(cmpData, 0, cmpData.Length);
							var events = new[] { event0, event1, event2 };
							cq.EnqueueWriteImage(img0, true, origin, region, 0, 0, (IntPtr)pSrc, 3, events);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 0, events);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0, 0, (IntPtr)pCmp, 3, events);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (int version)Copy not identical to source using no event output and a wait list");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, 0, 0, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 0, events, out event4);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0, 0, (IntPtr)pCmp, 3, events, out event5);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (int version)Copy not identical to source using no event output and a wait list");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}
						{
							var origin = new Int64[3] { 0, 0, 0 };
							var region = new Int64[3] { imgWidth, imgHeight, 1 };
							var dstOrigin = new Int64[3] { 0, 0, 0 };
							var dstRegion = new Int64[3] { imgWidth, imgHeight, 1 };

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, 0L, 0L, (IntPtr)pSrc);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0L, 0L, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (long version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, 0L, 0L, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 0, null, out event1);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0L, 0L, (IntPtr)pCmp, 0, null, out event2);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (long version)Copy not identical to source with event output and no wait list");

							Array.Clear(cmpData, 0, cmpData.Length);
							var events = new[] { event0, event1, event2 };
							cq.EnqueueWriteImage(img0, true, origin, region, 0L, 0L, (IntPtr)pSrc, 3, events);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 0, events);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0L, 0L, (IntPtr)pCmp, 3, events);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (long version)Copy not identical to source using no event output and a wait list");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteImage(img0, true, origin, region, 0L, 0L, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueCopyImage(img0, img1, origin, dstOrigin, region, 0, events, out event4);
							cq.EnqueueBarrier();
							cq.EnqueueReadImage(img1, true, origin, region, 0L, 0L, (IntPtr)pCmp, 3, events, out event5);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestImageReadWriteCopyOps: (long version)Copy not identical to source using no event output and a wait list");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}
					}
				}
			}
			catch (Exception e) {
				this.Error("Exception during testing: " + e);
			}
			finally {
				if (img0 != null)
					img0.Dispose();
				if (img1 != null)
					img1.Dispose();
				if (img2 != null)
					img2.Dispose();
			}
		}
		#endregion

		private void TestKernel(Context c, CommandQueue cq, Kernel argIOKernel) {
			var outArgBuffer = c.CreateBuffer((MemFlags)((UInt64)MemFlags.ALLOC_HOST_PTR | (UInt64)MemFlags.READ_WRITE), sizeof(IOKernelArgs), IntPtr.Zero);
			var data = new Byte[sizeof(IOKernelArgs)];
			this.Output("Testing kernel - Argument return");

			argIOKernel.SetArg(0, 1);
			argIOKernel.SetArg(1, 65L);
			argIOKernel.SetArg(2, 38.4f);
			argIOKernel.SetArg(3, outArgBuffer);

			Event ev;
			cq.EnqueueTask(argIOKernel, 0, null, out ev);
			cq.Finish();

			if ((Int32)ev.ExecutionStatus < 0) {
				this.Error(cq.Device.Name + ": argIOKernel failed with error code " + (ErrorCode)ev.ExecutionStatus);
				ev.Dispose();
			}
			else {
				outArgBuffer.Read(cq, 0L, data, 0, sizeof(IOKernelArgs));
				var outArgPtr = cq.EnqueueMapBuffer(outArgBuffer, true, MapFlags.READ, IntPtr.Zero, (IntPtr)sizeof(IOKernelArgs));
				var args = (IOKernelArgs)Marshal.PtrToStructure(outArgPtr, typeof(IOKernelArgs));
				cq.EnqueueUnmapMemObject(outArgBuffer, outArgPtr);

				if (args.outInt != 1)
					this.Error(cq.Device.Name + ": argIOKernel failed to return correct arguments");
				if (args.outLong != 65)
					this.Error(cq.Device.Name + ": argIOKernel failed to return correct arguments");
				if (args.outSingle != 38.4f)
					this.Error(cq.Device.Name + ": argIOKernel failed to return correct arguments");
			}
		}

		#region TestMapBuffer
		/// <summary>
		///     Test all versions of:
		///     EnqueueMapBuffer
		///     EnqueueMapImage
		///     The test bounces an array from a managed byte buffer to a mapped buffer,
		///     to an image. The image is then mapped and copied to a new managed buffer
		///     where the result is compared to the original.
		///     On error, the actual point of failure will have to be identified manually.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="cq"></param>
		private void TestMapBuffer(Context c, CommandQueue cq) {
			if (!cq.Device.ImageSupport) {
				this.Output("Skipping EnqueueMapBuffer and EnqueueMapImage tests(not supported on this device)");
				return;
			}

			this.Output("Testing MapBuffer");

			Image img0 = null;
			Mem mem0 = null;
			var imgWidth = 1024;
			var imgHeight = 1024;
			var bufLen = imgWidth * 4 * imgHeight;
			var srcData = new Byte[bufLen];
			var cmpData = new Byte[bufLen];
			Event event0;
			Event event1;

			for (var i = 0; i < srcData.Length; i++)
				srcData[i] = (Byte)(i);
			Array.Clear(cmpData, 0, cmpData.Length);

			try {
				img0 = c.CreateImage2D(MemFlags.READ_WRITE, ImageFormat.RGBA8U, imgWidth, imgHeight);
				mem0 = c.CreateBuffer(MemFlags.READ_WRITE, bufLen, IntPtr.Zero);

				Array.Clear(cmpData, 0, cmpData.Length);
				fixed (Byte* pSrc = srcData) {
					fixed (Byte* pCmp = cmpData) {
						{
							var origin = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var region = new IntPtr[3] { (IntPtr)imgWidth, (IntPtr)imgHeight, (IntPtr)1 };
							var dstOrigin = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var dstRegion = new IntPtr[3] { (IntPtr)imgWidth, (IntPtr)imgHeight, (IntPtr)1 };
							IntPtr mapPtr;
							Byte* pMapPtr;
							IntPtr image_row_pitch;
							IntPtr image_slice_pitch;

							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, (IntPtr)0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, true, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * (Int32)image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (IntPtr version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							Event fdjk;
							cq.EnqueueCopyBufferToImage(mem0, img0, (IntPtr)0, origin, region, 0, null, out fdjk);
							cq.Finish();

							mapPtr = cq.EnqueueMapImage(img0, false, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch, 0, null, out event0);
							cq.EnqueueWaitForEvent(event0);
							cq.Finish();
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * (Int32)image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (IntPtr version)Copy not identical to source when using event output and no wait list");

							var waitList = new[] { event0 };
							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, (IntPtr)0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, false, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch, 1, waitList, out event1);
							cq.EnqueueWaitForEvent(event1);
							cq.Finish();
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * (Int32)image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (IntPtr version)Copy not identical to source when using event output and no wait list");

							event0.Dispose();
							event1.Dispose();
						}
						{
							var origin = new Int32[3] { 0, 0, 0 };
							var region = new Int32[3] { imgWidth, imgHeight, 1 };
							IntPtr mapPtr;
							Byte* pMapPtr;
							Int32 image_row_pitch;
							Int32 image_slice_pitch;

							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, true, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (int version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, false, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch, 0, null, out event0);
							cq.EnqueueWaitForEvent(event0);
							cq.Finish();
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (int version)Copy not identical to source when using event output and no wait list");

							var waitList = new[] { event0 };
							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, false, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch, 1, waitList, out event1);
							cq.EnqueueWaitForEvent(event1);
							cq.Finish();
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (int version)Copy not identical to source when using event output and no wait list");

							event0.Dispose();
							event1.Dispose();
						}
						{
							var origin = new Int64[3] { 0, 0, 0 };
							var region = new Int64[3] { imgWidth, imgHeight, 1 };
							IntPtr mapPtr;
							Byte* pMapPtr;
							Int64 image_row_pitch;
							Int64 image_slice_pitch;

							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, true, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (long version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, false, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch, 0, null, out event0);
							cq.EnqueueWaitForEvent(event0);
							cq.Finish();
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (long version)Copy not identical to source when using event output and no wait list");

							var waitList = new[] { event0 };
							Array.Clear(cmpData, 0, cmpData.Length);
							mapPtr = cq.EnqueueMapBuffer(mem0, true, MapFlags.WRITE, 0, bufLen);
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var i = 0; i < bufLen; i++)
								pMapPtr[i] = srcData[i];
							cq.EnqueueUnmapMemObject(mem0, mapPtr);
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region);

							mapPtr = cq.EnqueueMapImage(img0, false, MapFlags.READ, origin, region, out image_row_pitch, out image_slice_pitch, 1, waitList, out event1);
							cq.EnqueueWaitForEvent(event1);
							cq.Finish();
							pMapPtr = (Byte*)mapPtr.ToPointer();
							for (var y = 0; y < imgHeight; y++) {
								var pSrcRowPtr = pMapPtr + y * image_row_pitch;
								var pDstRowPtr = pCmp + y * imgWidth * 4;
								for (var x = 0; x < imgWidth * 4; x++) {
									pDstRowPtr[x] = pSrcRowPtr[x];
								}
							}
							cq.EnqueueUnmapMemObject(img0, mapPtr);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueEnqueueMapBuffer/EnqueueMapImage: (long version)Copy not identical to source when using event output and no wait list");

							event0.Dispose();
							event1.Dispose();
						}
					}
				}
			}
			catch (Exception e) {
				this.Error("Exception during testing: " + e);
			}
			finally {
				if (img0 != null)
					img0.Dispose();
				if (mem0 != null)
					mem0.Dispose();
			}
		}
		#endregion

		#region TestMem
		private void TestMem(Context c, CommandQueue cq, Dictionary<String, Kernel> kernelDictionary) {
			var size = 8192;
			var testData = new Byte[size];

			this.Output("Testing Mem class");
			//Output( "Allocating "+size+" bytes of READ_WRITE memory" );
			using (var buffer = c.CreateBuffer(MemFlags.READ_WRITE, size, IntPtr.Zero)) {
				for (var i = 0; i < size / 2; i++) {
					testData[i] = 0;
					testData[size / 2 + i] = 1;
				}
				//Output("Mem.MemSize=" + size);
				if (buffer.MemSize.ToInt64() != size)
					this.Error("Mem.Size!=input size");
				//Output("Mem.MemType=" + buffer.MemType);
				if (buffer.MemType != MemObjectType.BUFFER)
					this.Error("Mem.MemType!=MemObjectType.BUFFER");

				//Output("Mem.MapCount=" + buffer.MapCount);
				if (buffer.MapCount != 0)
					this.Error("Mem.MapCount!=0");

				buffer.Write(cq, 0L, testData, 0, size);
				var k = kernelDictionary["TestReadWriteMemory"];
				k.SetArg(0, buffer);
				k.SetArg(1, (Int64)size);
				Event bleh;
				cq.EnqueueTask(k, 0, null, out bleh);
				cq.EnqueueBarrier();
				cq.Finish();
				buffer.Read(cq, 0L, testData, 0, size);
				for (var i = 0; i < size / 2; i++) {
					if (testData[i] != 1) {
						this.Error("TestReadWriteMemory failed");
						break;
					}
					if (testData[size / 2 + i] != 0) {
						this.Error("TestReadWriteMemory failed");
						break;
					}
				}
			}

			//Output("Allocating " + size + " bytes of READ memory");
			using (var buffer = c.CreateBuffer(MemFlags.READ_ONLY, size, IntPtr.Zero)) {
				//Output("Mem.MemSize=" + size);
				if (buffer.MemSize.ToInt64() != size)
					this.Error("Mem.Size!=input size");
				//Output("Mem.MemType=" + buffer.MemType);
				if (buffer.MemType != MemObjectType.BUFFER)
					this.Error("Mem.MemType!=MemObjectType.BUFFER");

				//Output("Mem.MapCount=" + buffer.MapCount);
				if (buffer.MapCount != 0)
					this.Error("Mem.MapCount!=0");

				var k = kernelDictionary["TestReadMemory"];
				k.SetArg(0, buffer);
				k.SetArg(1, (Int64)size);
				cq.EnqueueTask(k);
				cq.Finish();
			}

			//Output("Allocating " + size + " bytes of WRITE memory");
			using (var buffer = c.CreateBuffer(MemFlags.WRITE_ONLY, size, IntPtr.Zero)) {
				Array.Clear(testData, 0, size);
				//Output("Mem.MemSize=" + size);
				if (buffer.MemSize.ToInt64() != size)
					this.Error("Mem.Size!=input size");
				//Output("Mem.MemType=" + buffer.MemType);
				if (buffer.MemType != MemObjectType.BUFFER)
					this.Error("Mem.MemType!=MemObjectType.BUFFER");

				//Output("Mem.MapCount=" + buffer.MapCount);
				if (buffer.MapCount != 0)
					this.Error("Mem.MapCount!=0");

				var k = kernelDictionary["TestWriteMemory"];
				k.SetArg(0, buffer);
				k.SetArg(1, (Int64)size);
				cq.EnqueueTask(k);
				cq.Finish();
				buffer.Read(cq, 0L, testData, 0, size);
				for (var i = 0; i < size; i++) {
					if (testData[i] != 1) {
						this.Error("TestWriteMemory failed");
						break;
					}
				}
			}
			this.TestReadWriteCopyOps(c, cq);
			this.TestImageReadWriteCopyOps(c, cq);
			this.TestTransfersBetweenImageAndBuffers(c, cq);
			this.TestMapBuffer(c, cq);
			this.TestEnqueueNDRangeKernel(c, cq, kernelDictionary["EmptyKernel"]);
			this.TestBufferRectFunctions(c, cq);
		}
		#endregion

		private void TestOpenCLClass() {
			if (OpenCL.NumberOfPlatforms <= 0) {
				this.listBoxOutput.Items.Add("OpenCL.NumberOfPlatforms=" + OpenCL.NumberOfPlatforms);
				throw new Exception("TestOpenCLClass: NumberOfPlatforms<0. Is the API available at all?");
			}

			this.Platforms = OpenCL.GetPlatforms();
			if (this.Platforms.Length != OpenCL.NumberOfPlatforms)
				this.Error("OpenCL.NumberOfPlatforms!=Length of openCL.GetPlatforms()" + OpenCL.NumberOfPlatforms);

			for (var platformIndex = 0; platformIndex < this.Platforms.Length; platformIndex++) {
				if (OpenCL.GetPlatform(platformIndex) != this.Platforms[platformIndex])
					this.Error("openCL.GetPlatform(platformIndex)!=Platforms[platformIndex]");

				this.Output("======================================================");
				this.Output("Testing Platform Index=" + platformIndex + " Name=" + this.Platforms[platformIndex].Name);
				this.Output("======================================================");
				this.TestPlatform(this.Platforms[platformIndex]);
				this.Output("");
				this.Output("");
			}
		}

		private void TestOpenCLManager() {
			this.Output("======================================================");
			this.Output("Testing OpenCLManager");
			this.Output("======================================================");

			this.OpenCLManager.CreateDefaultContext(0, DeviceType.CPU);
			this.OpenCLManager.BuildOptions = "";
			this.OpenCLManager.Defines = "";
			var p0 = this.OpenCLManager.CompileFile("MemoryTests.cl");

			this.OpenCLManager.BuildOptions = "-D TestDefinition=1";
			this.OpenCLManager.Defines = "/*Woo defines!*/";
			var p1 = this.OpenCLManager.CompileFile("MemoryTests.cl");

			this.OpenCLManager.BuildOptions = "";
			this.OpenCLManager.Defines = "";
			var p2 = this.OpenCLManager.CompileSource("kernel void NullFunction(){}");

			this.OpenCLManager.BuildOptions = "-D TESTDEFINE=1";
			this.OpenCLManager.Defines = "";
			var p3 = this.OpenCLManager.CompileSource("kernel void NullFunction(){ int a=TESTDEFINE; if( a<3 ) ; }");

			this.OpenCLManager.BuildOptions = "";
			this.OpenCLManager.Defines = "#define TESTDEFINE 1";
			var p4 = this.OpenCLManager.CompileSource("kernel void NullFunction(){ int a=TESTDEFINE; if( a<3 ) ; }");

			var kernels0 = p0.CreateKernelDictionary();
			var kernels1 = p1.CreateKernelDictionary();
			var kernels2 = p2.CreateKernelDictionary();
			var kernels3 = p3.CreateKernelDictionary();
			var kernels4 = p4.CreateKernelDictionary();

			if (this.OpenCLManager.Context.Devices[0].HasExtension("cl_ext_device_fission")) {
				this.Output("Testing CreateSubDevices");
				var properties = new List<Object>();
				properties.Add((UInt64)DevicePartition.EQUALLY);
				properties.Add(2);
				properties.Add((UInt64)ListTerminators.PROPERTIES_LIST_END);

				Device[] subDevices;
				subDevices = this.OpenCLManager.Context.Devices[0].CreateSubDevicesEXT(properties);
				foreach (var d in subDevices)
					d.Dispose();
			}
			else {
				this.Output("Skipping test of CreateSubDevices: cl_ext_device_fission not supported");
			}
			this.Output("");
			this.Output("");
		}

		private void TestPlatform(Platform p) {
			Device[] allDevices;
			Device[] cpuDevices;
			Device[] gpuDevices;
			Device[] acceleratorDevices;

			this.Output("Name: " + p.Name);
			this.Output("Vendor:" + p.Vendor);
			this.Output("Version:" + p.Version);

			// Check format of version string
			var m = this.ParseOpenCLVersion.Match(p.Version);
			if (!m.Success)
				this.Warning("Platform " + p.Name + " has an invalid version string");
			else {
				if (m.Groups["MajorVersion"].Value != "1" && m.Groups["MinorVersion"].Value != "0")
					this.Warning("Platform " + p.Name + " has a version number!=1.0(Not really a problem, but this test is written for 1.0)");
			}

			// Check format of profile
			this.Output("Profile:" + p.Profile);
			if (p.Profile == "FULL_PROFILE" || p.Profile == "EMBEDDED_PROFILE")
				this.Output("Profile:" + p.Profile);
			else
				this.Warning("Platform " + p.Name + " has unknown profile " + p.Profile);

			this.Output("Extensions: " + p.Extensions);

			// Test whether number of devices is consistent
			allDevices = p.QueryDevices(DeviceType.ALL);
			if (allDevices.Length <= 0)
				this.Warning("Platform " + p.Name + " has no devices");

			var sb = new StringBuilder();
			foreach (var d in allDevices)
				sb.Append("\"" + d.Name + "\" ");
			this.Output("Devices: " + sb);

			cpuDevices = p.QueryDevices(DeviceType.CPU);
			gpuDevices = p.QueryDevices(DeviceType.GPU);
			acceleratorDevices = p.QueryDevices(DeviceType.ACCELERATOR);
			if (allDevices.Length != cpuDevices.Length + gpuDevices.Length + acceleratorDevices.Length)
				this.Warning("QueryDevices( DeviceType.ALL ) return length inconsistent with sum of special purpose queries");

			// Create a few contexts and test them
			this.Output("Testing Platform.CreateDefaultContext()");
			using (var c = p.CreateDefaultContext()) {
				this.Output("Testing context" + c);
				this.TestContext(c);
			}
			this.Output("");
			this.Output("");

			if (cpuDevices.Length > 0) {
				this.Output("Testing Platform.CreateContext() with CPU devices");
				using (var c = p.CreateContext(null, cpuDevices, this.ContextNotifyFunc, (IntPtr)0x01234567)) {
					this.Output("Testing context " + c);
					this.TestContext(c);
				}
				this.Output("");
				this.Output("");
			}

			if (gpuDevices.Length > 0) {
				this.Output("Testing Platform.CreateContext() with GPU devices");
				using (var c = p.CreateContext(null, gpuDevices, this.ContextNotifyFunc, (IntPtr)0x01234567)) {
					//Output("Testing context " + c);
					this.TestContext(c);
				}
				this.Output("");
				this.Output("");
			}

			if (cpuDevices.Length > 0) {
				this.Output("Testing Platform.CreateContextFromType()");
				var contextProperties = new[] {
					(IntPtr)ContextProperties.PLATFORM,
					p.PlatformID,
					IntPtr.Zero
				};
				using (var c = p.CreateContextFromType(contextProperties, DeviceType.CPU, this.ContextNotifyFunc, (IntPtr)0x01234567)) {
					//Output("Testing context " + c);
					this.TestContext(c);
				}
				this.Output("");
				this.Output("");
			}

			if (gpuDevices.Length > 0) {
				this.Output("Testing Platform.CreateContextFromType()");
				var contextProperties = new[] {
					(IntPtr)ContextProperties.PLATFORM,
					p.PlatformID,
					IntPtr.Zero
				};
				using (var c = p.CreateContextFromType(contextProperties, DeviceType.GPU, this.ContextNotifyFunc, (IntPtr)0x01234567)) {
					//Output("Testing context " + c);
					this.TestContext(c);
					this.Output("");
					this.Output("");
				}
			}
		}

		#region TestReadWriteCopyOps
		/// <summary>
		///     Test all versions of:
		///     EnqueueReadBuffer
		///     EnqueueWriteBuffer
		///     EnqueueCopyBuffer
		///     The test just copies the entirety of a buffer and checks if the result is equal to the original.
		///     An error indicates that one of the above functions failed and further manual analysis is required
		///     to pinpoint the error.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="cq"></param>
		private void TestReadWriteCopyOps(Context c, CommandQueue cq) {
			this.Output("Testing read/write/copy functions");
			Mem buf0 = null;
			Mem buf1 = null;
			Mem buf2 = null;
			var bufLen = 1024 * 1024;
			var srcData = new Byte[bufLen];
			var cmpData = new Byte[bufLen];
			Event event0;
			Event event1;
			Event event2;
			Event event3;
			Event event4;
			Event event5;

			for (var i = 0; i < srcData.Length; i++)
				srcData[i] = (Byte)(i);
			Array.Clear(cmpData, 0, cmpData.Length);

			try {
				buf0 = c.CreateBuffer(MemFlags.READ_WRITE, bufLen, IntPtr.Zero);
				buf1 = c.CreateBuffer(MemFlags.READ_WRITE, bufLen, IntPtr.Zero);
				buf2 = c.CreateBuffer(MemFlags.READ_WRITE, bufLen, IntPtr.Zero);

				#region Test EnqueueReadBuffer EnqueueWriteBuffer EnqueueCopyBuffer
				fixed (Byte* pSrc = srcData) {
					fixed (Byte* pCmp = cmpData) {
						{
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pSrc);
							cq.EnqueueCopyBuffer(buf0, buf1, (IntPtr)0, (IntPtr)0, (IntPtr)bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(IntPtr version): Copy not identical to source");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueCopyBuffer(buf0, buf1, (IntPtr)0, (IntPtr)0, (IntPtr)bufLen, 0, null, out event1);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pCmp, 0, null, out event2);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(IntPtr version): Copy not identical to source");

							var events = new[] { event0, event1, event2 };

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pSrc, 3, events);
							cq.EnqueueCopyBuffer(buf0, buf1, (IntPtr)0, (IntPtr)0, (IntPtr)bufLen, 3, events);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pCmp, 3, events);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(IntPtr version): Copy not identical to source");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueCopyBuffer(buf0, buf1, (IntPtr)0, (IntPtr)0, (IntPtr)bufLen, 3, events, out event4);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, (IntPtr)0, (IntPtr)bufLen, (IntPtr)pCmp, 3, events, out event5);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(IntPtr version): Copy not identical to source");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}

						{
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0, bufLen, (IntPtr)pSrc);
							cq.EnqueueCopyBuffer(buf0, buf1, 0, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0, bufLen, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(int version): Copy not identical to source");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0, bufLen, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueCopyBuffer(buf0, buf1, 0, 0, bufLen, 0, null, out event1);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0, bufLen, (IntPtr)pCmp, 0, null, out event2);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(int version): Copy not identical to source");

							var events = new[] { event0, event1, event2 };

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0, bufLen, (IntPtr)pSrc, 3, events);
							cq.EnqueueCopyBuffer(buf0, buf1, 0, 0, bufLen, 3, events);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0, bufLen, (IntPtr)pCmp, 3, events);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(int version): Copy not identical to source");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0, bufLen, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueCopyBuffer(buf0, buf1, 0, 0, bufLen, 3, events, out event4);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0, bufLen, (IntPtr)pCmp, 3, events, out event5);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(int version): Copy not identical to source");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}

						{
							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0L, bufLen, (IntPtr)pSrc);
							cq.EnqueueCopyBuffer(buf0, buf1, 0L, 0L, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0L, bufLen, (IntPtr)pCmp);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(long version): Copy not identical to source");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0L, bufLen, (IntPtr)pSrc, 0, null, out event0);
							cq.EnqueueCopyBuffer(buf0, buf1, 0L, 0L, bufLen, 0, null, out event1);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0L, bufLen, (IntPtr)pCmp, 0, null, out event2);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(long version): Copy not identical to source");

							var events = new[] { event0, event1, event2 };

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0L, bufLen, (IntPtr)pSrc, 3, events);
							cq.EnqueueCopyBuffer(buf0, buf1, 0L, 0L, bufLen, 3, events);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0L, bufLen, (IntPtr)pCmp, 3, events);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(long version): Copy not identical to source");

							Array.Clear(cmpData, 0, cmpData.Length);
							cq.EnqueueWriteBuffer(buf0, true, 0L, bufLen, (IntPtr)pSrc, 3, events, out event3);
							cq.EnqueueCopyBuffer(buf0, buf1, 0L, 0L, bufLen, 3, events, out event4);
							cq.EnqueueBarrier();
							cq.EnqueueReadBuffer(buf1, true, 0L, bufLen, (IntPtr)pCmp, 3, events, out event5);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("TestReadWriteCopyOps(long version): Copy not identical to source");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
							event4.Dispose();
							event5.Dispose();
						}
					}
				}
				#endregion
			}
			catch (Exception e) {
				this.Error("Exception during testing: " + e);
			}
			finally {
				if (buf0 != null)
					buf0.Dispose();
				if (buf1 != null)
					buf1.Dispose();
				if (buf2 != null)
					buf2.Dispose();
			}
		}
		#endregion

		#region TestTransfersBetweenImageAndBuffers
		/// <summary>
		///     Test all versions of:
		///     EnqueueCopyImageToBuffer
		///     EnqueueCopyBufferToImage
		///     The test just copies the entirety of a buffer and checks if the result is equal to the original.
		///     An error indicates that one of the above functions failed and further manual analysis is required
		///     to pinpoint the error.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="cq"></param>
		private void TestTransfersBetweenImageAndBuffers(Context c, CommandQueue cq) {
			if (!cq.Device.ImageSupport) {
				this.Output("Skipping CopyImageToBuffer and CopyBufferToImage tests(not supported on this device)");
				return;
			}

			this.Output("Testing CopyImageToBuffer and CopyBufferToImage");

			Image img0 = null;
			Mem mem0 = null;
			var imgWidth = 1024;
			var imgHeight = 1024;
			var bufLen = imgWidth * 4 * imgHeight;
			var srcData = new Byte[bufLen];
			var cmpData = new Byte[bufLen];
			Event event0;
			Event event1;
			Event event2;
			Event event3;

			for (var i = 0; i < srcData.Length; i++)
				srcData[i] = (Byte)(i);
			Array.Clear(cmpData, 0, cmpData.Length);

			try {
				img0 = c.CreateImage2D(MemFlags.READ_WRITE, ImageFormat.RGBA8U, imgWidth, imgHeight);
				mem0 = c.CreateBuffer(MemFlags.READ_WRITE, bufLen, IntPtr.Zero);

				Array.Clear(cmpData, 0, cmpData.Length);
				fixed (Byte* pSrc = srcData) {
					fixed (Byte* pCmp = cmpData) {
						{
							var origin = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var region = new IntPtr[3] { (IntPtr)imgWidth, (IntPtr)imgHeight, (IntPtr)1 };
							var dstOrigin = new IntPtr[3] { (IntPtr)0, (IntPtr)0, (IntPtr)0 };
							var dstRegion = new IntPtr[3] { (IntPtr)imgWidth, (IntPtr)imgHeight, (IntPtr)1 };

							Array.Clear(cmpData, 0, cmpData.Length);
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, (IntPtr)0, origin, region);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, (IntPtr)0);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (IntPtr version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, (IntPtr)0, origin, region, 0, null, out event0);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, (IntPtr)0, 0, null, out event1);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (IntPtr version)Copy not identical to source when using event output and event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							var events = new[] { event0, event1 };
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, (IntPtr)0, origin, region, 2, events, out event2);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, (IntPtr)0, 2, events, out event3);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (IntPtr version)Copy not identical to source when using event output and a wait list");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
						}
						{
							var origin = new Int32[3] { 0, 0, 0 };
							var region = new Int32[3] { imgWidth, imgHeight, 1 };
							var dstOrigin = new Int32[3] { 0, 0, 0 };
							var dstRegion = new Int32[3] { imgWidth, imgHeight, 1 };

							Array.Clear(cmpData, 0, cmpData.Length);
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, 0);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (int version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region, 0, null, out event0);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, 0, 0, null, out event1);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (int version)Copy not identical to source when using event output no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							var events = new[] { event0, event1 };
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, 0, origin, region, 2, events, out event2);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, 0, 2, events, out event3);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (int version)Copy not identical to source when using event output and a wait list");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
						}
						{
							var origin = new Int64[3] { 0, 0, 0 };
							var region = new Int64[3] { imgWidth, imgHeight, 1 };
							var dstOrigin = new Int64[3] { 0, 0, 0 };
							var dstRegion = new Int64[3] { imgWidth, imgHeight, 1 };

							Array.Clear(cmpData, 0, cmpData.Length);
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, 0L, origin, region);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, 0L);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (long version)Copy not identical to source when using no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, 0L, origin, region, 0, null, out event0);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, 0L, 0, null, out event1);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (long version)Copy not identical to source when using event output and no event args");

							Array.Clear(cmpData, 0, cmpData.Length);
							var events = new[] { event0, event1 };
							mem0.Write(cq, 0L, srcData, 0, bufLen);
							cq.EnqueueBarrier();
							cq.EnqueueCopyBufferToImage(mem0, img0, 0L, origin, region, 2, events, out event2);
							cq.EnqueueBarrier();
							cq.EnqueueCopyImageToBuffer(img0, mem0, origin, region, 0L, 2, events, out event3);
							cq.EnqueueBarrier();
							mem0.Read(cq, 0L, cmpData, 0, bufLen);
							if (!this.CompareArray(cmpData, srcData))
								this.Error("EnqueueCopyBufferToImage/EnqueueCopyImageToBuffer: (long version)Copy not identical to source when using event output and a wait list");

							event0.Dispose();
							event1.Dispose();
							event2.Dispose();
							event3.Dispose();
						}
					}
				}
			}
			catch (Exception e) {
				this.Error("Exception during testing: " + e);
			}
			finally {
				if (img0 != null)
					img0.Dispose();
				if (mem0 != null)
					mem0.Dispose();
			}
		}
		#endregion

		private void TestVecKernel(Context c, CommandQueue cq, Kernel k) {
			var f2 = new Float2(0.0f, 1.0f);
			var memory = new Single[2];

			fixed (Single* pMemory = memory) {
				var mem = c.CreateBuffer((MemFlags)((UInt64)MemFlags.READ_WRITE | (UInt64)MemFlags.USE_HOST_PTR), 4 * 2, pMemory);

				k.SetArg(0, f2);
				k.SetArg(1, mem);
				cq.EnqueueTask(k);
				cq.EnqueueBarrier();
				var pMap = cq.EnqueueMapBuffer(mem, true, MapFlags.READ, 0, 2 * 4);
				cq.EnqueueUnmapMemObject(mem, pMap);
			}
		}

		private void Warning(String s) {
			this.listBoxWarnings.Items.Add(s);
			this.listBoxOutput.SelectedIndex = this.listBoxOutput.Items.Count - 1;
			Application.DoEvents();
		}

		public void NativeKernelTest(Object o, void*[] buffers) {
			Interlocked.Increment(ref this.NativeKernelCalled);
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct IOKernelArgs {
			internal Int64 outLong;
			internal Int32 outInt;
			internal Single outSingle;
		}

		#region TestUserEventCallbacks
		volatile Boolean TestUserEventCallbackCalled;

		private void TestUserEventCallbacks(Context c, CommandQueue cq) {
			if (!(cq.Device.Platform.OpenCLMajorVersion >= 1 && cq.Device.Platform.OpenCLMinorVersion >= 1)) {
				this.Output("Skipping TestUserEventCallbacks tests(Requires OpenCL 1.1 or higher)");
				return;
			}

			this.Output("Testing User Events and event callbacks");

			this.TestUserEventCallbackCalled = false;
			var event0 = c.CreateUserEvent();
			event0.SetCallback(ExecutionStatus.COMPLETE, this.TestUserEventCallback, this);
			event0.SetUserEventStatus(ExecutionStatus.COMPLETE);
			if (this.TestUserEventCallbackCalled != true) {
				this.Error("Event callback not called");
				return;
			}
			c.WaitForEvent(event0);
		}

		private void TestUserEventCallback(Event e, ExecutionStatus executionStatus, Object userData) {
			this.TestUserEventCallbackCalled = true;
		}
		#endregion

		#region TestCommandQueue helper functions
		private void TestCommandQueueAsync(Context c, CommandQueue cq, Kernel kernel) {
			var events = new List<Event>();
			Event clEvent;

			this.Output("Testing asynchronous task issuing (clEnqueueTask) and waiting for events");

			// Issue a bunch of slow operations
			kernel.SetArg(0, 5000000);
			for (var i = 0; i < 10; i++) {
				cq.EnqueueTask(kernel, 0, null, out clEvent);
				events.Add(clEvent);
			}

			// Issue a bunch of fast operations
			kernel.SetArg(0, 500);
			for (var i = 0; i < 10; i++) {
				cq.EnqueueTask(kernel, 0, null, out clEvent);
				events.Add(clEvent);
			}

			var eventList = events.ToArray();
			cq.EnqueueWaitForEvents(eventList.Length, eventList);
			while (events.Count > 0) {
				if ((Int32)events[0].ExecutionStatus < 0) {
					this.Output(cq.Device.Name + ": TestCommandQueueAsync failed with error code " + (ErrorCode)events[0].ExecutionStatus);
				}
				events[0].Dispose();
				events.RemoveAt(0);
			}
		}

		private void TestCommandQueueMemCopy(Context c, CommandQueue cq) {
			this.Output("Testing synchronous host memory->memory copy");

			var aafSrc = new AlignedArrayFloat(1024 * 1024, 64);
			var aafDst = new AlignedArrayFloat(1024 * 1024, 64);

			this.SetAAF(aafSrc, 0.0f);
			this.SetAAF(aafDst, 1.0f);

			/// Test HOST_PTR -> HOST_PTR copy
			/// The call to EnqueueMapBuffer synchronizes caches before testing the result
			using (var memSrc = c.CreateBuffer((MemFlags)((UInt64)MemFlags.READ_WRITE + (UInt64)MemFlags.USE_HOST_PTR), aafSrc.ByteLength, aafSrc)) {
				using (var memDst = c.CreateBuffer((MemFlags)((UInt64)MemFlags.READ_WRITE + (UInt64)MemFlags.USE_HOST_PTR), aafDst.ByteLength, aafDst)) {
					cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
					cq.EnqueueBarrier();
					var mappedPtr = cq.EnqueueMapBuffer(memDst, true, MapFlags.READ_WRITE, (IntPtr)0, (IntPtr)aafDst.ByteLength);
					if (!this.TestAAF(aafDst, 0.0f))
						this.Error("EnqueueCopyBuffer failed, destination is invalid");
					cq.EnqueueUnmapMemObject(memDst, mappedPtr);
					cq.EnqueueBarrier();
				}
			}

			/// Test COPY_HOST_PTR -> COPY_HOST_PTR copy
			/// Verify that original source buffers are intact and that the copy was successful
			this.SetAAF(aafSrc, 0.0f);
			this.SetAAF(aafDst, 1.0f);
			using (var memSrc = c.CreateBuffer(MemFlags.COPY_HOST_PTR, aafSrc.ByteLength, aafSrc)) {
				using (var memDst = c.CreateBuffer(MemFlags.COPY_HOST_PTR, aafSrc.ByteLength, aafDst)) {
					this.SetAAF(aafSrc, 2.0f);
					this.SetAAF(aafDst, 3.0f);

					cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
					cq.Finish();

					if (!this.TestAAF(aafSrc, 2.0f))
						this.Error("Memory copy destroyed src buffer");
					if (!this.TestAAF(aafDst, 3.0f))
						this.Error("Memory copy destroyed dst buffer");
					Event ev;
					cq.EnqueueReadBuffer(memDst, false, IntPtr.Zero, (IntPtr)aafDst.ByteLength, aafDst, 0, null, out ev);
					cq.EnqueueWaitForEvents(1, new[] { ev });
					ev.Dispose();
					cq.Finish();
					if (!this.TestAAF(aafDst, 0.0f))
						this.Error("Memory copy failed");
				}
			}

			/// Test ALLOC_HOST_PTR -> ALLOC_HOST_PTR copy
			this.SetAAF(aafSrc, 0.0f);
			this.SetAAF(aafDst, 1.0f);
			using (var memSrc = c.CreateBuffer((MemFlags)((UInt64)MemFlags.ALLOC_HOST_PTR + (UInt64)MemFlags.READ_WRITE), aafSrc.ByteLength, IntPtr.Zero)) {
				using (var memDst = c.CreateBuffer((MemFlags)((UInt64)MemFlags.ALLOC_HOST_PTR + (UInt64)MemFlags.WRITE_ONLY), aafSrc.ByteLength, IntPtr.Zero)) {
					cq.EnqueueWriteBuffer(memSrc, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
					cq.EnqueueWriteBuffer(memDst, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
					cq.EnqueueBarrier();

					cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
					cq.EnqueueBarrier();

					cq.EnqueueReadBuffer(memDst, true, IntPtr.Zero, (IntPtr)aafDst.ByteLength, aafDst);
					if (!this.TestAAF(aafDst, 0.0f))
						this.Error("Memory copy failed");
				}
			}

			/// Test DEFAULT -> DEFAULT copy
			this.SetAAF(aafSrc, 0.0f);
			this.SetAAF(aafDst, 1.0f);
			using (var memSrc = c.CreateBuffer((MemFlags)((UInt64)MemFlags.ALLOC_HOST_PTR + (UInt64)MemFlags.READ_ONLY), aafSrc.ByteLength, IntPtr.Zero)) {
				using (var memDst = c.CreateBuffer((MemFlags)((UInt64)MemFlags.ALLOC_HOST_PTR + (UInt64)MemFlags.WRITE_ONLY), aafSrc.ByteLength, IntPtr.Zero)) {
					cq.EnqueueWriteBuffer(memSrc, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
					cq.EnqueueWriteBuffer(memDst, false, (IntPtr)0, (IntPtr)aafSrc.ByteLength, aafSrc);
					cq.EnqueueBarrier();

					cq.EnqueueCopyBuffer(memSrc, memDst, IntPtr.Zero, IntPtr.Zero, (IntPtr)aafSrc.ByteLength);
					cq.EnqueueBarrier();

					cq.EnqueueReadBuffer(memDst, true, IntPtr.Zero, (IntPtr)aafDst.ByteLength, aafDst);
					if (!this.TestAAF(aafDst, 0.0f))
						this.Error("Memory copy failed");
				}
			}
		}

		private Boolean TestAAF(AlignedArrayFloat aaf, Single c) {
			for (var i = 0; i < aaf.Length; i++)
				if (aaf[i] != c)
					return false;
			return true;
		}

		private void SetAAF(AlignedArrayFloat aaf, Single c) {
			for (var i = 0; i < aaf.Length; i++)
				aaf[i] = c;
		}

		private void SetAAFLinear(AlignedArrayFloat aaf) {
			for (var i = 0; i < aaf.Length; i++)
				aaf[i] = i;
		}
		#endregion
	}
}