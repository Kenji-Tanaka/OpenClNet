using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenCLNet;
using Image = System.Drawing.Image;

namespace ImageCrossFade {
	public partial class Form1 : Form {
		readonly IntPtr[] CrossFadeGlobalWorkSize = new IntPtr[2];
		Kernel CrossFadeKernel;
		Boolean Initialized;

		Bitmap InputBitmap0;
		Bitmap InputBitmap1;
		BitmapData InputBitmapData0;
		BitmapData InputBitmapData1;
		Mem InputBuffer0;
		Mem InputBuffer1;

		OpenCLManager OCLMan;
		OpenCLNet.Program OCLProgram;
		Bitmap OutputBitmap;

		public Form1() {
			this.InitializeComponent();
		}

		private void comboBoxDeviceSelector_SelectedIndexChanged(Object sender, EventArgs e) { }

		/// <summary>
		///     Launch the CrossFadeKernel
		///     First we set its arguments,
		///     then we enqueue the kernel using EnqueueNDRangeKernel,
		///     and finally, map the buffer for reading to make sure
		///     there aren't any cache issues when OpenCL completes.
		/// </summary>
		/// <param name="ratio"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="input0"></param>
		/// <param name="inputStride0"></param>
		/// <param name="input1"></param>
		/// <param name="inputStride1"></param>
		/// <param name="output"></param>
		private void DoCrossFade(
			Single ratio,
			Int32 width, Int32 height,
			Mem input0, Int32 inputStride0,
			Mem input1, Int32 inputStride1,
			BitmapData output) {
			Mem outputBuffer;

			var deviceIndex = this.comboBoxDeviceSelector.SelectedIndex;

			outputBuffer = this.OCLMan.Context.CreateBuffer(MemFlags.USE_HOST_PTR, output.Stride * output.Height, output.Scan0);

			this.CrossFadeGlobalWorkSize[0] = (IntPtr)width;
			this.CrossFadeGlobalWorkSize[1] = (IntPtr)height;
			this.CrossFadeKernel.SetArg(0, ratio);
			this.CrossFadeKernel.SetArg(1, width);
			this.CrossFadeKernel.SetArg(2, height);
			this.CrossFadeKernel.SetArg(3, input0);
			this.CrossFadeKernel.SetArg(4, inputStride0);
			this.CrossFadeKernel.SetArg(5, input1);
			this.CrossFadeKernel.SetArg(6, inputStride1);
			this.CrossFadeKernel.SetArg(7, outputBuffer);
			this.CrossFadeKernel.SetArg(8, output.Stride);

			this.OCLMan.CQ[deviceIndex].EnqueueNDRangeKernel(this.CrossFadeKernel, 2, null, this.CrossFadeGlobalWorkSize, null);
			this.OCLMan.CQ[deviceIndex].EnqueueBarrier();
			var p = this.OCLMan.CQ[deviceIndex].EnqueueMapBuffer(outputBuffer, true, MapFlags.READ, IntPtr.Zero, (IntPtr)(output.Stride * output.Height));
			this.OCLMan.CQ[deviceIndex].EnqueueUnmapMemObject(outputBuffer, p);
			this.OCLMan.CQ[deviceIndex].Finish();
			outputBuffer.Dispose();
		}

		private void Form1_Load(Object sender, EventArgs e) {
			try {
				this.InitializeOpenCL();
				this.SetupBitmaps();
				this.Initialized = true;
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message, "Initiation failed...");
				Application.Exit();
			}
		}

		private void Form1_Paint(Object sender, PaintEventArgs e) {
			var g = e.Graphics;

			if (!this.Initialized) {
				g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, this.Width, this.Height);
				return;
			}

			g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, this.Width, this.Height);
			g.DrawImageUnscaled(this.OutputBitmap, 0, 50);
		}

		private void hScrollBarRatio_ValueChanged(Object sender, EventArgs e) {
			if (!this.Initialized)
				return;

			var bd = this.OutputBitmap.LockBits(new Rectangle(0, 0, this.OutputBitmap.Width, this.OutputBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			this.DoCrossFade(this.hScrollBarRatio.Value * 0.01f, this.InputBitmapData0.Width, this.InputBitmapData0.Height, this.InputBuffer0, this.InputBitmapData0.Stride, this.InputBuffer1, this.InputBitmapData1.Stride,
				bd);
			this.OutputBitmap.UnlockBits(bd);
			this.Refresh();
		}

		/// <summary>
		///     Create a OpenCLManager, configure it, and then create a context using all devices in platform 0,
		///     Once the context is up and running we compile our source file "OpenCLFunctions.cl"
		///     The Helper automatically compiles and creates kernels.
		///     We can then extract named kernels using the GetKernel method.
		///     For more advanced scenarios, one might use the functions in the Platform class
		///     to query devices, create contexts etc. Platforms can be enumerated using
		///     for( int i=0; i
		///     <OpenCL.NumberofPlatforms; i++ )
		///         Platform p= OpenCL.GetPlatform( i);
		/// </summary>
		private void InitializeOpenCL() {
			if (OpenCL.NumberOfPlatforms == 0) {
				MessageBox.Show("OpenCL not available");
				Application.Exit();
			}

			this.OCLMan = new OpenCLManager();
			// Attempt to save binaries after compilation, as well as load precompiled binaries
			// to avoid compilation. Usually you'll want this to be true. 
			this.OCLMan.AttemptUseBinaries = true;
			// Attempt to compile sources. This should probably be true for almost all projects.
			// Setting it to false means that when you attempt to compile "mysource.cl", it will
			// only scan the precompiled binary directory for a binary corresponding to a source
			// with that name. There's a further restriction that the compiled binary also has to
			// use the same Defines and BuildOptions
			this.OCLMan.AttemptUseSource = true;
			// Binary and source paths
			// This is where we store our sources and where compiled binaries are placed
			this.OCLMan.BinaryPath = @"OpenCL\bin";
			this.OCLMan.SourcePath = @"OpenCL\src";
			// If true, RequireImageSupport will filter out any devices without image support
			// In this project we don't need image support though, so we set it to false
			this.OCLMan.RequireImageSupport = false;
			// The Defines string gets prepended to any and all sources that are compiled
			// and serve as a convenient way to pass configuration information to the compilation process
			this.OCLMan.Defines = "#define MyCompany_MyProject_Define 1";
			// The BuildOptions string is passed directly to clBuild and can be used to do debug builds etc
			this.OCLMan.BuildOptions = "";

			this.OCLMan.CreateDefaultContext(0, DeviceType.ALL);

			this.OCLProgram = this.OCLMan.CompileFile("OpenCLFunctions.cl");

			for (var i = 0; i < this.OCLMan.Context.Devices.Length; i++)
				this.comboBoxDeviceSelector.Items.Add(this.OCLMan.Context.Devices[i].Vendor + ":" + this.OCLMan.Context.Devices[i].Name);
			this.comboBoxDeviceSelector.SelectedIndex = 0;

			this.CrossFadeKernel = this.OCLProgram.CreateKernel("CrossFade");
		}

		/// <summary>
		///     Loads two bitmaps and locks them for the duration of the program.
		///     Also creates two OpenCL buffers that map to the locked images
		/// </summary>
		private void SetupBitmaps() {
			this.InputBitmap0 = (Bitmap)Image.FromFile(@"Input0.png");
			this.InputBitmap1 = (Bitmap)Image.FromFile(@"Input1.png");
			if (this.InputBitmap1.Size != this.InputBitmap0.Size)
				this.InputBitmap1 = new Bitmap(this.InputBitmap1, this.InputBitmap0.Size);
			this.OutputBitmap = new Bitmap(this.InputBitmap0);

			this.InputBitmapData0 = this.InputBitmap0.LockBits(new Rectangle(0, 0, this.InputBitmap0.Width, this.InputBitmap0.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			this.InputBitmapData1 = this.InputBitmap1.LockBits(new Rectangle(0, 0, this.InputBitmap1.Width, this.InputBitmap1.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			this.InputBuffer0 = this.OCLMan.Context.CreateBuffer(MemFlags.USE_HOST_PTR, this.InputBitmapData0.Stride * this.InputBitmapData0.Height, this.InputBitmapData0.Scan0);
			this.InputBuffer1 = this.OCLMan.Context.CreateBuffer(MemFlags.USE_HOST_PTR, this.InputBitmapData1.Stride * this.InputBitmapData1.Height, this.InputBitmapData1.Scan0);
		}

		protected override void OnPaintBackground(PaintEventArgs e) { }
	}
}