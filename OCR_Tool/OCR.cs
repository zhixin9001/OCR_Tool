using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            //this.Hide();
            //Thread.Sleep(500);
            //Bitmap CatchBmp = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
            //Graphics g = Graphics.FromImage(CatchBmp);
            //g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height));
            //var capture = new Capture(this);
            //capture.BackgroundImage = CatchBmp;
            //capture.Height = Screen.AllScreens[0].Bounds.Height;
            //capture.Width = Screen.AllScreens[0].Bounds.Width;
            //capture.Show();
            this.test();
        }

        private void test()
        {
            var APP_ID = "11332510";
            var API_KEY = "ThTBt3ratDfOvS43z2Ce1MdA";
            var SECRET_KEY = "cljSiZ1B2C6zxqk8P0edulqVjI9ks0Yo";
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            var image = File.ReadAllBytes("e:/capture.jpg");
            var result = client.AccurateBasic(image);
            var last = result.Last.Last();
            for(var i = 0; i < last.Count(); i++)
            {
                var str= JsonConvert.DeserializeObject<Result>(last[i].ToString());
                this.rtbResult.AppendText(str.words);
                this.rtbResult.AppendText("\r\n");
            }

        }
    }

    public class Result
    {
        public string words;
    }
}
