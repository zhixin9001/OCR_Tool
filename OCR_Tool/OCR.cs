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
            Thread objThread = new Thread(new ThreadStart(delegate
            {
                ThreadMethodTxt(formCapture.base64Image);
            }));
            objThread.Start();
        }

        //创建一个用于操作richtextbox的委托
        public delegate void ocrDelegete(byte[] base64Img);
        public void ThreadMethodTxt(byte[] base64Img)
        {
            var ocrDelMethod = new ocrDelegete(OcrMethod);
            this.BeginInvoke(ocrDelMethod, base64Img);
        }

        private void OcrMethod(byte[] base64Img)
        {
            try
            {
                var apiAuthConfig = Api_Auth.apiAuthConfig;
                var client = new Baidu.Aip.Ocr.Ocr(apiAuthConfig.API_KEY, apiAuthConfig.SECRET_KEY);
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
            this.rtbResult.Focus();
            this.rtbResult.SelectAll();
            Clipboard.SetText(this.rtbResult.Text);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.rtbResult.ResetText();
        }
    }


}
