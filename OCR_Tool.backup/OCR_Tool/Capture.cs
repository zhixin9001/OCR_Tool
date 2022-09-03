using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace OCR_Tool
{
    /*
     * 代码来源：https://www.cnblogs.com/zhiboday/p/6024427.html
     */
    public partial class Capture : Form
    {
        public Capture(OCR ocrForm)
        {
            InitializeComponent();
            this.ocrForm = ocrForm;
        }

        #region 定义程序变量
        private OCR ocrForm;
        // 用来记录鼠标按下的坐标，用来确定绘图起点
        private Point DownPoint;
        // 用来表示是否截图完成
        private bool CatchFinished = false;
        // 用来表示截图开始
        private bool CatchStart = false;
        // 用来保存原始图像
        private Bitmap originBmp;
        // 用来保存截图的矩形
        private Rectangle CatchRectangle;

        public event EventHandler captureFinished;
        public byte[] base64Image;
        #endregion

        private void Capture_Load(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            this.Cursor = Cursors.Cross;
            this.originBmp = new Bitmap(this.BackgroundImage);
        }

        private void Capture_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.ocrForm.Show();
                this.Close();
            }
        }

        private void Capture_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!this.CatchStart)
                {
                    CatchStart = true;
                    DownPoint = new Point(e.X, e.Y);
                    System.Diagnostics.Debug.WriteLine("Capture_MouseDown");
                }
            }
        }

        private void Capture_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (CatchStart)
                {
                    CatchStart = false;
                    CatchFinished = true;
                    System.Diagnostics.Debug.WriteLine("Capture_MouseUp");

                    BeginOCR();
                }
            }
        }

        private void Capture_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // 确保截图开始
                if (CatchStart)
                {
                    System.Diagnostics.Debug.WriteLine("Capture_MouseMove");
                    // 新建一个图片对象，让它与屏幕图片相同
                    Bitmap copyBmp = (Bitmap)originBmp.Clone();

                    // 获取鼠标按下的坐标
                    Point newPoint = new Point(DownPoint.X, DownPoint.Y);

                    // 新建画板和画笔
                    Graphics g = Graphics.FromImage(copyBmp);
                    Pen p = new Pen(Color.Red, 2);

                    // 获取矩形的长宽
                    int width = Math.Abs(e.X - DownPoint.X);
                    int height = Math.Abs(e.Y - DownPoint.Y);
                    if (e.X < DownPoint.X)
                    {
                        newPoint.X = e.X;
                    }
                    if (e.Y < DownPoint.Y)
                    {
                        newPoint.Y = e.Y;
                    }

                    CatchRectangle = new Rectangle(newPoint, new Size(width, height));

                    // 将矩形画在画板上
                    g.DrawRectangle(p, CatchRectangle);

                    // 释放目前的画板
                    g.Dispose();
                    p.Dispose();
                    // 从当前窗体创建新的画板
                    Graphics g1 = this.CreateGraphics();

                    // 将刚才所画的图片画到截图窗体上
                    // 为什么不直接在当前窗体画图呢？
                    // 如果自己解决将矩形画在窗体上，会造成图片抖动并且有无数个矩形
                    // 这样实现也属于二次缓冲技术
                    g1.DrawImage(copyBmp, new Point(0, 0));
                    g1.Dispose();
                    // 释放拷贝图片，防止内存被大量消耗
                    copyBmp.Dispose();
                }
            }
            catch (Exception exp)
            {
                LogHelper.WriteLog(exp);
            }
        }

        private void Capture_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    if (e.Button == MouseButtons.Left && CatchFinished && (CatchRectangle.Width > 0 && CatchRectangle.Height > 0))
            //    {
            //        System.Diagnostics.Debug.WriteLine("Capture_MouseDoubleClick");
            //        // 新建一个与矩形一样大小的空白图片
            //        Bitmap CatchedBmp = new Bitmap(CatchRectangle.Width, CatchRectangle.Height);

            //        Graphics g = Graphics.FromImage(CatchedBmp);

            //        // 把originBmp中指定部分按照指定大小画到空白图片上
            //        // CatchRectangle指定originBmp中指定部分
            //        // 第二个参数指定绘制到空白图片的位置和大小
            //        // 画完后CatchedBmp不再是空白图片了，而是具有与截取的图片一样的内容
            //        g.DrawImage(originBmp, new Rectangle(0, 0, CatchRectangle.Width, CatchRectangle.Height), CatchRectangle, GraphicsUnit.Pixel);

            //        CatchFinished = false;
            //        MemoryStream m = new MemoryStream();
            //        CatchedBmp.Save(m, ImageFormat.Jpeg);
            //        this.base64Image = m.GetBuffer();
            //        this.lbTip.Top = Convert.ToInt32(Screen.AllScreens[0].Bounds.Height * 0.5);
            //        this.lbTip.Left = Convert.ToInt32(Screen.AllScreens[0].Bounds.Width * 0.5 - this.lbTip.Width);
            //        this.lbTip.Visible = true;
            //        captureFinished?.Invoke(this, EventArgs.Empty);
            //    }
            //}
            //catch (Exception exp)
            //{
            //    LogHelper.WriteLog(exp);
            //}
        }

        private void BeginOCR()
        {
            try
            {
                if (CatchRectangle.Width > 0 && CatchRectangle.Height > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Capture_MouseDoubleClick");
                    // 新建一个与矩形一样大小的空白图片
                    Bitmap CatchedBmp = new Bitmap(CatchRectangle.Width, CatchRectangle.Height);

                    Graphics g = Graphics.FromImage(CatchedBmp);

                    // 把originBmp中指定部分按照指定大小画到空白图片上
                    // CatchRectangle指定originBmp中指定部分
                    // 第二个参数指定绘制到空白图片的位置和大小
                    // 画完后CatchedBmp不再是空白图片了，而是具有与截取的图片一样的内容
                    g.DrawImage(originBmp, new Rectangle(0, 0, CatchRectangle.Width, CatchRectangle.Height), CatchRectangle, GraphicsUnit.Pixel);

                    CatchFinished = false;
                    MemoryStream m = new MemoryStream();
                    CatchedBmp.Save(m, ImageFormat.Jpeg);
                    this.base64Image = m.GetBuffer();
                    this.lbTip.Top = Convert.ToInt32(Screen.AllScreens[0].Bounds.Height * 0.5);
                    this.lbTip.Left = Convert.ToInt32(Screen.AllScreens[0].Bounds.Width * 0.5 - this.lbTip.Width);
                    this.lbTip.Visible = true;
                    captureFinished?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    LogHelper.WriteLog(new Exception(string.Format("Invalid size, width:{0}, height:{1}", CatchRectangle.Width, CatchRectangle.Height)));
                }
            }
            catch (Exception exp)
            {
                LogHelper.WriteLog(exp);
            }
        }
    }
}
