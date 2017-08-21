using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCLNet;

namespace OpenCLTest {
	public partial class Form1 : Form {
		Int32 FromSetPtr;
		Mandelbrot Mandelbrot;
		readonly Single[,] Sets = new Single[2, 4] {
			{ -2.0f, 2.0f, 2.0f, -2.0f },
			{ -0.17225f, 0.66116f, -0.17115f, 0.660347f }
		};
		Int32 ToSetPtr;
		DateTime ZoomStart;

		TimeSpan ZoomTime = TimeSpan.FromSeconds(30);

		public Form1() {
			this.InitializeComponent();
		}

		private void Form1_Load(Object sender, EventArgs e) {
			try {
				var platform = OpenCL.GetPlatform(0);
				this.Mandelbrot = new Mandelbrot(platform, this.Width, this.Height);
				this.Mandelbrot.AllocBuffers();
				this.UpdateMandelbrot();
			}
			catch (Exception oex) {
				MessageBox.Show(oex.ToString(), "OpenCL Initialization failed");
				Application.Exit();
			}
		}

		private void startToolStripMenuItem_Click(Object sender, EventArgs e) {
			this.startToolStripMenuItem.Enabled = false;
			this.stopToolStripMenuItem.Enabled = true;
			this.ToSetPtr = 1;
			this.ZoomStart = DateTime.Now;
			this.timer.Start();
		}

		private void stopToolStripMenuItem_Click(Object sender, EventArgs e) {
			this.startToolStripMenuItem.Enabled = true;
			this.stopToolStripMenuItem.Enabled = false;
			this.ToSetPtr = 0;
			this.ZoomStart = DateTime.Now;
			this.timer.Start();
		}

		private void timer_Tick(Object sender, EventArgs e) {
			var dt = DateTime.Now - this.ZoomStart;
			if (dt >= this.ZoomTime) {
				this.timer.Stop();
				this.FromSetPtr = this.ToSetPtr;
			}
			this.ZoomMandelbrot();
		}

		private void UpdateMandelbrot() {
			this.Mandelbrot.Calculate();

			using (var gfx = this.CreateGraphics()) {
				gfx.DrawImageUnscaled(this.Mandelbrot.Bitmap, 0, 0);
				gfx.DrawString("ms per frame=" + this.Mandelbrot.CalculationTimeMS, this.Font, Brushes.Yellow, new PointF(50.0f, 50.0f));
			}
		}

		protected Int32 Align(Int32 i, Int32 align) {
			return (i + align - 1) / align * align;
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			this.UpdateMandelbrot();
		}

		public void ZoomMandelbrot() {
			var zoomFactor = (Single)((DateTime.Now - this.ZoomStart).TotalMilliseconds / this.ZoomTime.TotalMilliseconds);
			this.Mandelbrot.Left = this.Sets[this.FromSetPtr, 0] + (this.Sets[this.ToSetPtr, 0] - this.Sets[this.FromSetPtr, 0]) * zoomFactor;
			this.Mandelbrot.Right = this.Sets[this.FromSetPtr, 2] + (this.Sets[this.ToSetPtr, 2] - this.Sets[this.FromSetPtr, 2]) * zoomFactor;
			this.Mandelbrot.Top = this.Sets[this.FromSetPtr, 1] + (this.Sets[this.ToSetPtr, 1] - this.Sets[this.FromSetPtr, 1]) * zoomFactor;
			this.Mandelbrot.Bottom = this.Sets[this.FromSetPtr, 3] + (this.Sets[this.ToSetPtr, 3] - this.Sets[this.FromSetPtr, 3]) * zoomFactor;
			this.UpdateMandelbrot();
		}
	}
}

#if false
            ErrorCode result;
            uint numPlatforms;
            IntPtr[] platformIDs;

            // Get number of platforms
            result = cl.GetPlatformIDs( 0, null, out numPlatforms );
            if( result!=ErrorCode.SUCCESS )
                throw new Exception( "GetPlatformIDs failed with ErrorCode."+result );
            Debug.WriteLine( "Number of platforms: "+numPlatforms );
            if( numPlatforms==0 )
                throw new Exception( "No openCL platforms available." );

            // Create an array of platform IDs
            platformIDs = new IntPtr[numPlatforms];
            result = cl.GetPlatformIDs( numPlatforms, platformIDs, out numPlatforms );
            if( result!=ErrorCode.SUCCESS )
                throw new Exception( "GetPlatformIDs failed with ErrorCode."+result );
            Debug.WriteLine("");


        int[] globSize = new int[3];
        int[] globID = new int[3];
        float left;
        float top;
        float right;
        float bottom;
        AlignedArrayFloat aaf;

        private void KernelIterator2d()
        {
            int activeIndex = globSize.Length-1;

            while( activeIndex>=0 )
            {
                MandelBrotKernel( left, top, right, bottom, aaf );
                while( activeIndex>=0 )
                {
                    globID[activeIndex]++;
                    if( globID[activeIndex]>=globSize[activeIndex] )
                    {
                        globID[activeIndex] = 0;
                        activeIndex--;
                    }
                    else
                    {
                        activeIndex = globSize.Length-1;
                        break;
                    }
                }
            }
        }

        private int get_global_size( int dimension ) { return globSize[dimension]; }
        private int get_global_id( int dimension ) { return globID[dimension]; }
        private void MandelBrotKernel( float left, float top, float right, float bottom, AlignedArrayFloat af )
        {
            int width = get_global_size(0);
            int height = get_global_size(1);
            int cx = get_global_id(0);
            int cy = get_global_id(1);
            float dx = (right-left)/(float)width;
            float dy = (bottom-top)/(float)height;

            float x0 = left+dx*(float)cx;
            float y0 = top+dy*(float)cy;
            float x = 0.0f;
            float y = 0.0f;
            int iteration = 0;
            int max_iteration = 1000;

            while( x*x-y*y<=(2*2) && iteration<max_iteration )
            {
                float xtemp = x*x-y*y+x0;
                y = 2*x*y+y0;
                x = xtemp;
                iteration++;
            }
            float color;
            color = iteration==max_iteration?0.0f: (float)iteration/(float)max_iteration;
            af[width*4*cy+cx*4+0] = 1.0f;
            af[width*4*cy+cx*4+1] = color;
            af[width*4*cy+cx*4+2] = color;
            af[width*4*cy+cx*4+3] = color;
        }

#endif