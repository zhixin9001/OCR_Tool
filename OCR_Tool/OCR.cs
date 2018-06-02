using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace OCR_Tool
{
    public partial class OCR : Form
    {
        public OCR()
        {
            InitializeComponent();
            this.btnCapture.Focus();
        }

        private Capture formCapture;

        private void btnCapture_Click(object sender, EventArgs e)
        {
            this.Hide();
            Thread.Sleep(500);
            Bitmap catchedBmp = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
            Graphics g = Graphics.FromImage(catchedBmp);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height));
            formCapture = new Capture(this);
            formCapture.BackgroundImage = catchedBmp;
            formCapture.Height = Screen.AllScreens[0].Bounds.Height;
            formCapture.Width = Screen.AllScreens[0].Bounds.Width;
            formCapture.captureFinished += Capture_captureFinished;
            formCapture.Show();
        }

        private void Capture_captureFinished(object sender, EventArgs e)
        {
            this.ocrCaptured(formCapture.base64Image);
        }

        private void ocrCaptured(byte[] base64Img)
        {
            try
            {
                var apiAuthConfig = Api_Auth.apiAuthConfig;
                var API_KEY = apiAuthConfig.API_KEY;
                var SECRET_KEY = apiAuthConfig.SECRET_KEY;
                var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
                var result = client.AccurateBasic(base64Img);
                var last = result.Last.Last();
                this.rtbResult.ResetText();
                for (var i = 0; i < last.Count(); i++)
                {
                    var str = JsonConvert.DeserializeObject<Result>(last[i].ToString());
                    this.rtbResult.AppendText(str.words);
                    this.rtbResult.AppendText("\r\n");
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(e);
            }
            finally
            {
                this.Show();
                formCapture.Close();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.rtbResult.Text))
            {
                return;
            }
            this.rtbResult.SelectionBackColor = Color.Blue;
            this.rtbResult.SelectAll();
            Clipboard.SetText(this.rtbResult.Text);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.rtbResult.ResetText();
        }
    }


}
