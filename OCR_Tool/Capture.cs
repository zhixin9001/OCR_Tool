﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCR_Tool
{
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
                this.DialogResult = DialogResult.OK;
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
                }
            }
        }

        private void Capture_MouseMove(object sender, MouseEventArgs e)
        {
            // 确保截图开始
            if (CatchStart)
            {
                // 新建一个图片对象，让它与屏幕图片相同
                Bitmap copyBmp = (Bitmap)originBmp.Clone();

                // 获取鼠标按下的坐标
                Point newPoint = new Point(DownPoint.X, DownPoint.Y);

                // 新建画板和画笔
                Graphics g = Graphics.FromImage(copyBmp);
                Pen p = new Pen(Color.Red, 1);

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

        private void Capture_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CatchFinished)
            {
                // 新建一个与矩形一样大小的空白图片
                Bitmap CatchedBmp = new Bitmap(CatchRectangle.Width, CatchRectangle.Height);

                Graphics g = Graphics.FromImage(CatchedBmp);

                // 把originBmp中指定部分按照指定大小画到空白图片上
                // CatchRectangle指定originBmp中指定部分
                // 第二个参数指定绘制到空白图片的位置和大小
                // 画完后CatchedBmp不再是空白图片了，而是具有与截取的图片一样的内容
                g.DrawImage(originBmp, new Rectangle(0, 0, CatchRectangle.Width, CatchRectangle.Height), CatchRectangle, GraphicsUnit.Pixel);

                // 将图片保存到剪切板中
                //Clipboard.SetImage(CatchedBmp);
                this.BackgroundImage = CatchedBmp;
                this.Width = CatchRectangle.Width;
                this.Height = CatchRectangle.Height;
                //g.Dispose();

                CatchFinished = false;
                //CatchedBmp.Dispose();
                MemoryStream m = new MemoryStream();
                CatchedBmp.Save(m, ImageFormat.Jpeg);
                this.base64Image = m.GetBuffer();
                captureFinished?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}