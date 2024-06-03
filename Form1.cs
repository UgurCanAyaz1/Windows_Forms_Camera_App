using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;


namespace Windows_Forms_Camera_App
{
	public partial class Form1 : Form
	{
		// Defining attributes of the AForge

		FilterInfoCollection fico;
		VideoCaptureDevice vcd;

		public Form1()
		{
			InitializeComponent();
		}

		
		private void Form1_Load(object sender, EventArgs e)
		{
			fico = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo f in fico)
			{
				comboBox1.Items.Add(f.Name);
				comboBox1.SelectedIndex = 0;
			}

			this.FormClosing += Form1_FormClosing;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			vcd= new VideoCaptureDevice(fico[comboBox1.SelectedIndex].MonikerString);
			vcd.NewFrame += Vcd_NewFrame;
			vcd.Start();
		}

		private void Vcd_NewFrame(object sender, NewFrameEventArgs eventArgs)

		{
			bool isCrop = checkBox1.Checked;

			int x_min = (int)numericUpDown1.Value;
			int x_max = (int)numericUpDown2.Value;
			int y_min = (int)numericUpDown3.Value;
			int y_max = (int)numericUpDown4.Value;

			if (isCrop ==false)
			{

				// Assign default Image to Picturebox
				pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();

				return;
			}


			if ( (x_max == 0 || y_max == 0) && isCrop == true)
			{

				// Assign default Image to Picturebox
				pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
				return;
			}

			else
			{
				Bitmap frame = eventArgs.Frame;

				// Assign max values for the appropriate crop
				numericUpDown2.Maximum = (int)frame.PhysicalDimension.Width;
				numericUpDown4.Maximum = (int)frame.PhysicalDimension.Height;


				// Define area for the crop operation
				Rectangle cropArea = new Rectangle(x_min, y_min, x_max, y_max); // (x, y, width, height)

				// Create cropped image
				Bitmap croppedFrame = CropBitmap(frame, cropArea);

				// Assign cropped Image to Picturebox
				pictureBox1.Image = croppedFrame;
				return;
			}

		}

		private Bitmap CropBitmap(Bitmap bitmap, Rectangle cropArea)
		{
			// Kırpılmış görüntü için yeni bir Bitmap oluştur
			Bitmap croppedBitmap = new Bitmap(cropArea.Width, cropArea.Height);

			// Yeni Bitmap'e kırpılmış görüntüyü çizin
			using (Graphics g = Graphics.FromImage(croppedBitmap))
			{
				g.DrawImage(bitmap, new Rectangle(0, 0, cropArea.Width, cropArea.Height), cropArea, GraphicsUnit.Pixel);
			}

			return croppedBitmap;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Form kapatıldığında VideoCaptureDevice'ı durdur
			if (vcd != null && vcd.IsRunning)
			{
				vcd.Stop();
			}
		}

	}
}
