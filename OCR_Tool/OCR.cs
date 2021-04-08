using Newtonsoft.Json;
using System;
using System.Configuration;
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
            this.ckbReplaceComma.Checked = this.IsReplaceComma();
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
                var items = result.First.First;
                foreach(var item in items)
                {
                    var words = item["words"];
                    var str = words.ToString();
                    var outputStr = this.ckbReplaceComma.Checked ? ReplaceComma(str) : str;
                    this.rtbResult.AppendText(outputStr);
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

        private bool IsReplaceComma()
        {
            //string path = System.Windows.Forms.Application.ExecutablePath;
            //var config = ConfigurationManager.OpenExeConfiguration(path);
            var replaceCommaConfig = ConfigurationManager.AppSettings["replaceComma"];
            if (replaceCommaConfig != null)
            {
                return Boolean.Parse(replaceCommaConfig);
            }
            else
            {
                return false;
            }

        }

        private string ReplaceComma(string str)
        {
            return str.Replace(",", "，")
                            .Replace(".", "。")
                            .Replace(":", "：")
                            .Replace(";", "；");
        }

        private void ckbReplaceComma_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbReplaceComma.Checked)
            {
                ConfigurationManager.AppSettings.Set("replaceComma", "true");
            }
            else
            {
                ConfigurationManager.AppSettings.Set("replaceComma", "false");
            }
        }
    }


}
