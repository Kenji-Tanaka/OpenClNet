using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using CL = OpenCLNet;

namespace OpenCLImageTest {
	public partial class Form1 : Form {
		readonly List<String> CallBackEventList = new List<String>();
		CL.Kernel FilterKernel;
		// Currently active context
		CL.Context oclContext;
		readonly CL.ContextNotify oclContextNotify;
		// Command queue to selected device
		CL.CommandQueue oclCQ;
		// Currently selected device
		CL.Device oclDevice;
		// All devices in the currently selected platform
		CL.Device[] oclDevices;

		Boolean oclFullyInitialized;
		CL.Image OCLInputImage;
		CL.Image OCLOutputImage;

		// Currently selected platform
		CL.Platform oclPlatform;

		// Current program+data
		CL.Program oclProgram;
		CL.Sampler OCLSampler;
		String OpenCLSource;
		Bitmap TestImage;
		Bitmap TestImageOutput;

		public Form1() {
			this.InitializeComponent();
			this.oclContextNotify = this.OpenCLContextNotifyCallBack;
		}

		private void buttonScaleImage_Click(Object sender, EventArgs e) {
			if (this.oclFullyInitialized) {
				this.ScaleImage();
			}
			this.groupBoxScaled.Refresh();
		}

		private void comboBoxOpenCLDevices_SelectedIndexChanged(Object sender, EventArgs e) {
			Boolean supportsImages;
			Boolean supportsImageFormat;

			try {
				this.ReleaseDeviceResources();
				this.panelScaled.Refresh();

				this.oclDevice = this.oclDevices[this.comboBoxOpenCLDevices.SelectedIndex];
				this.CreateContext(this.oclPlatform, this.oclDevice);
				supportsImages = this.oclDevice.ImageSupport;
				supportsImageFormat = this.oclContext.SupportsImageFormat(CL.MemFlags.READ_WRITE, CL.MemObjectType.IMAGE2D, CL.ChannelOrder.RGBA, CL.ChannelType.UNSIGNED_INT8);
				if (this.oclDevice.ImageSupport && supportsImageFormat) {
					this.buttonScaleImage.Enabled = true;
					this.labelImageSupportIndicator.Text = "Yes";
					this.OpenCLSource = File.ReadAllText(@"OpenCLFunctions.cl");
					this.BuildOCLSource(this.OpenCLSource);
					this.CreateOCLImages(this.oclContext);
					this.oclFullyInitialized = true;
				}
				else {
					this.buttonScaleImage.Enabled = false;
					this.labelImageSupportIndicator.Text = "No " + (supportsImageFormat ? "(No Image support at all)" : "(Images supported, but no support for RGBA8888)");
					this.oclContext = null;
				}
			}
			catch (CL.OpenCLBuildException oclbe) {
				MessageBox.Show(this, oclbe.BuildLogs[0], "OpenCL build error");
			}
			catch (CL.OpenCLException ocle) {
				MessageBox.Show(this, ocle.Message, "OpenCL exception");
			}
		}

		private void comboBoxOpenCLPlatforms_SelectedIndexChanged(Object sender, EventArgs e) {
			try {
				this.ReleaseDeviceResources();

				this.oclPlatform = CL.OpenCL.GetPlatform(this.comboBoxOpenCLPlatforms.SelectedIndex);
				this.oclDevices = this.oclPlatform.QueryDevices(CL.DeviceType.ALL);
				this.PopulateOCLDevicesComboBox(this.oclPlatform, CL.DeviceType.ALL);
				if (this.comboBoxOpenCLDevices.Items.Count > 0) {
					this.comboBoxOpenCLDevices.SelectedIndex = 0;
				}
				else {
					this.oclDevice = null;
				}
			}
			catch (CL.OpenCLException ocle) {
				MessageBox.Show(this, ocle.Message, "OpenCL exception");
			}
		}

		private void Form1_Load(Object sender, EventArgs e) {
			try {
				this.Setup();
			}
			catch (CL.OpenCLException ocle) {
				MessageBox.Show(ocle.Message);
				Application.Exit();
			}
		}

		private void panelOriginal_Paint(Object sender, PaintEventArgs e) {
			var g = e.Graphics;

			if (this.TestImage != null)
				g.DrawImageUnscaled(this.TestImage, 0, 0);
		}

		private void panelScaled_Paint(Object sender, PaintEventArgs e) {
			var g = e.Graphics;

			if (this.oclFullyInitialized) {
				if (this.TestImageOutput != null)
					g.DrawImageUnscaled(this.TestImageOutput, 0, 0);
			}
		}

		public void BuildOCLSource(String source) {
			this.oclProgram = this.oclContext.CreateProgramWithSource(source);
			this.oclProgram.Build();
			this.FilterKernel = this.oclProgram.CreateKernel("FilterImage");
		}

		public void CopyOCLBitmapToBitmap(CL.Mem oclBitmap, Bitmap bitmap) {
			var origin = new IntPtr[3];
			var region = new IntPtr[3];
			CL.Mem buffer;

			var bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			buffer = this.oclContext.CreateBuffer((CL.MemFlags)((Int64)CL.MemFlags.WRITE_ONLY | (Int64)CL.MemFlags.USE_HOST_PTR), bd.Height * bd.Stride, bd.Scan0);
			origin[0] = (IntPtr)0;
			origin[1] = (IntPtr)0;
			origin[2] = (IntPtr)0;
			region[0] = (IntPtr)bd.Width;
			region[1] = (IntPtr)bd.Height;
			region[2] = (IntPtr)1;
			this.oclCQ.EnqueueCopyImageToBuffer(oclBitmap, buffer, origin, region, IntPtr.Zero);
			this.oclCQ.EnqueueBarrier();
			var p = this.oclCQ.EnqueueMapBuffer(buffer, true, CL.MapFlags.READ, IntPtr.Zero, (IntPtr)(bd.Height * bd.Stride));
			this.oclCQ.EnqueueUnmapMemObject(buffer, p);
			this.oclCQ.Finish();
			buffer.Dispose();
			bitmap.UnlockBits(bd);
		}

		public void CreateContext(CL.Platform platform, CL.Device device) {
			var contextProperties = new[] {
				(IntPtr)CL.ContextProperties.PLATFORM,
				platform.PlatformID,
				IntPtr.Zero,
				IntPtr.Zero
			};

			var devices = new[] {
				device
			};

			this.oclContext = platform.CreateContext(contextProperties, devices, this.oclContextNotify, IntPtr.Zero);
			this.oclCQ = this.oclContext.CreateCommandQueue(device, CL.CommandQueueProperties.PROFILING_ENABLE);
		}

		public CL.Image CreateOCLBitmapFromBitmap(Bitmap bitmap) {
			CL.Image oclImage;

			var bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			oclImage = this.oclContext.CreateImage2D((CL.MemFlags)((Int64)CL.MemFlags.READ_ONLY | (Int64)CL.MemFlags.COPY_HOST_PTR),
				CL.ImageFormat.RGBA8U, bd.Width, bd.Height, bd.Stride, bd.Scan0);
			bitmap.UnlockBits(bd);
			return oclImage;
		}

		public void CreateOCLImages(CL.Context context) {
			this.OCLInputImage = this.CreateOCLBitmapFromBitmap(this.TestImage);
			this.OCLOutputImage = this.oclContext.CreateImage2D(CL.MemFlags.WRITE_ONLY, CL.ImageFormat.RGBA8U, this.panelScaled.Width, this.panelScaled.Height, 0, IntPtr.Zero);
			this.OCLSampler = this.oclContext.CreateSampler(true, CL.AddressingMode.CLAMP_TO_EDGE, CL.FilterMode.LINEAR);
		}

		public void OpenCLContextNotifyCallBack(String errInfo, Byte[] privateInfo, IntPtr cb, IntPtr userData) {
			this.CallBackEventList.Add(errInfo);
			this.textBoxCallBackEvents.Lines = this.CallBackEventList.ToArray();
		}

		public void PopulateOCLDevicesComboBox(CL.Platform p, CL.DeviceType deviceType) {
			var devices = p.QueryDevices(deviceType);
			this.comboBoxOpenCLDevices.Items.Clear();
			foreach (var d in devices) {
				this.comboBoxOpenCLDevices.Items.Add(d.Vendor + " " + d.Name);
			}
		}

		public void PopulateOCLPlatformsComboBox() {
			this.comboBoxOpenCLPlatforms.Items.Clear();
			for (var platformID = 0; platformID < CL.OpenCL.NumberOfPlatforms; platformID++) {
				var p = CL.OpenCL.GetPlatform(platformID);

				this.comboBoxOpenCLPlatforms.Items.Add(p.Vendor + ":" + p.Name + " " + p.Version);
			}
		}

		public void ReleaseDeviceResources() {
			this.oclFullyInitialized = false;
			if (this.OCLSampler != null) {
				this.OCLSampler.Dispose();
				this.OCLSampler = null;
			}
			if (this.OCLInputImage != null) {
				this.OCLInputImage.Dispose();
				this.OCLInputImage = null;
			}

			if (this.OCLOutputImage != null) {
				this.OCLOutputImage.Dispose();
				this.OCLOutputImage = null;
			}

			if (this.FilterKernel != null) {
				this.FilterKernel.Dispose();
				this.FilterKernel = null;
			}

			if (this.oclProgram != null) {
				this.oclProgram.Dispose();
				this.oclProgram = null;
			}

			if (this.oclCQ != null) {
				this.oclCQ.Dispose();
				this.oclCQ = null;
			}

			if (this.oclContext != null) {
				this.oclContext.Dispose();
				this.oclContext = null;
			}
		}

		public void ScaleImage() {
			var globalWorkSize = new IntPtr[3];

			globalWorkSize[0] = (IntPtr)this.TestImageOutput.Width;
			globalWorkSize[1] = (IntPtr)this.TestImageOutput.Height;
			this.FilterKernel.SetArg(0, 0.0f);
			this.FilterKernel.SetArg(1, 0.0f);
			this.FilterKernel.SetArg(2, 1.0f);
			this.FilterKernel.SetArg(3, 1.0f);
			this.FilterKernel.SetArg(4, 0.0f);
			this.FilterKernel.SetArg(5, 0.0f);
			this.FilterKernel.SetArg(6, 1.0f);
			this.FilterKernel.SetArg(7, 1.0f);
			this.FilterKernel.SetArg(8, this.OCLInputImage);
			this.FilterKernel.SetArg(9, this.OCLOutputImage);
			this.FilterKernel.SetArg(10, this.OCLSampler);
			this.oclCQ.EnqueueNDRangeKernel(this.FilterKernel, 2, null, globalWorkSize, null);
			this.oclCQ.EnqueueBarrier();
			this.CopyOCLBitmapToBitmap(this.OCLOutputImage, this.TestImageOutput);
			this.oclCQ.Finish();
		}

		public void Setup() {
			this.TestImage = (Bitmap)Image.FromFile(@"Input0.png");
			this.TestImage = new Bitmap(this.TestImage, 256, 256);
			this.TestImageOutput = new Bitmap(this.panelScaled.Width, this.panelScaled.Height, PixelFormat.Format32bppArgb);

			if (CL.OpenCL.NumberOfPlatforms <= 0) {
				MessageBox.Show("OpenCL not available");
				Application.Exit();
			}

			this.PopulateOCLPlatformsComboBox();
			this.oclPlatform = CL.OpenCL.GetPlatform(0);
			this.comboBoxOpenCLPlatforms.SelectedIndex = 0;
		}

		public void SetupOpenCL() {
			if (CL.OpenCL.NumberOfPlatforms == 0) {
				MessageBox.Show("OpenCL not available");
				Application.Exit();
			}
		}
	}
}