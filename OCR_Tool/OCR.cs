using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCR_Tool
{
    public partial class OCR : Form
    {
        public OCR()
        {
            InitializeComponent();
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            this.Hide();
            Thread.Sleep(100);
            Bitmap CatchBmp = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
            Graphics g = Graphics.FromImage(CatchBmp);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height));
            var capture = new Capture(this);
            capture.BackgroundImage = CatchBmp;
            capture.Height = Screen.AllScreens[0].Bounds.Height;
            capture.Width = Screen.AllScreens[0].Bounds.Width;
            capture.Show();
      
        }
    }
}
