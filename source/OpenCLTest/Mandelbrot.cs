using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OpenCLNet;

namespace OpenCLTest {
	public class Mandelbrot {
		readonly Kernel mandelbrotKernel;
		readonly Mem mandelbrotMemBuffer;
		readonly Program mandelBrotProgram;
		readonly Context openCLContext;
		readonly CommandQueue openCLCQ;
		readonly Device[] openCLDevices;

		readonly Platform openCLPlatform;

		public UInt64 CalculationTimeMS;

		public Bitmap Bitmap { get; protected set; }
		public Int32 BitmapHeight { get; set; }

		public Int32 BitmapWidth { get; set; }
		public Single Bottom { get; set; }
		public Single Left { get; set; }
		public Single Right { get; set; }
		public Single Top { get; set; }

		public Mandelbrot(Platform platform, Int32 width, Int32 height) {
			this.openCLPlatform = platform;
			this.openCLDevices = this.openCLPlatform.QueryDevices(DeviceType.ALL);
			this.openCLContext = this.openCLPlatform.CreateDefaultContext();
			this.openCLCQ = this.openCLContext.CreateCommandQueue(this.openCLDevices[0], CommandQueueProperties.PROFILING_ENABLE);
			this.mandelBrotProgram = this.openCLContext.CreateProgramWithSource(File.ReadAllText("Mandelbrot.cl"));
			try {
				this.mandelBrotProgram.Build();
			}
			catch (OpenCLException) {
				var buildLog = this.mandelBrotProgram.GetBuildLog(this.openCLDevices[0]);
				MessageBox.Show(buildLog, "Build error(64 bit debug sessions in vs2008 always fail like this - debug in 32 bit or use vs2010)");
				Application.Exit();
			}
			this.mandelbrotKernel = this.mandelBrotProgram.CreateKernel("Mandelbrot");

			this.Left = -2.0f;
			this.Top = 2.0f;
			this.Right = 2.0f;
			this.Bottom = -2.0f;
			this.BitmapWidth = width;
			this.BitmapHeight = height;

			this.mandelbrotMemBuffer = this.openCLContext.CreateBuffer((MemFlags)((Int64)MemFlags.WRITE_ONLY), width * height * 4, IntPtr.Zero);
		}

		public void AllocBuffers() {
			this.Bitmap = new Bitmap(this.BitmapWidth, this.BitmapHeight, PixelFormat.Format32bppArgb);
		}

		public void Calculate() {
			var bd = this.Bitmap.LockBits(new Rectangle(0, 0, this.Bitmap.Width, this.Bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			var bitmapSize = bd.Width * bd.Height;

			this.mandelbrotKernel.SetArg(0, this.Left);
			this.mandelbrotKernel.SetArg(1, this.Top);
			this.mandelbrotKernel.SetArg(2, this.Right);
			this.mandelbrotKernel.SetArg(3, this.Bottom);
			this.mandelbrotKernel.SetArg(4, this.BitmapWidth);
			this.mandelbrotKernel.SetArg(5, this.mandelbrotMemBuffer);

			Event calculationStart;
			Event calculationEnd;

			this.openCLCQ.EnqueueMarker(out calculationStart);

			var globalWorkSize = new IntPtr[2];

			globalWorkSize[0] = (IntPtr)this.BitmapWidth;
			globalWorkSize[1] = (IntPtr)this.BitmapHeight;
			this.openCLCQ.EnqueueNDRangeKernel(this.mandelbrotKernel, 2, null, globalWorkSize, null);

			for (var y = 0; y < this.BitmapHeight; y++)
				this.openCLCQ.EnqueueReadBuffer(this.mandelbrotMemBuffer, true, (IntPtr)(this.BitmapWidth * 4 * y), (IntPtr)(this.BitmapWidth * 4), (IntPtr)(bd.Scan0.ToInt64() + y * bd.Stride));
			this.openCLCQ.Finish();
			this.openCLCQ.EnqueueMarker(out calculationEnd);
			this.openCLCQ.Finish();

			UInt64 start = 0;
			UInt64 end = 0;
			try {
				calculationStart.GetEventProfilingInfo(ProfilingInfo.QUEUED, out start);
				calculationEnd.GetEventProfilingInfo(ProfilingInfo.END, out end);
			}
			catch (OpenCLException) { }
			finally {
				this.CalculationTimeMS = (end - start) / 1000000;
				calculationStart.Dispose();
				calculationEnd.Dispose();
			}
			this.Bitmap.UnlockBits(bd);
		}
	}
}