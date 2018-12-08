/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Drawing;

namespace Symbol.Drawing {

    /// <summary>
    /// 产生随即图片
    /// </summary>
    public class RandomImage : IDisposable {

        #region fields
        private static readonly string _randCharString = "0百1邦2浪3数4语5看6顶7国8我9视AB青C汗D重E无F纹G新H项K好L理MP去QR透TUV案WX走Y度Z猜";
        private static readonly string[] _fontNames = new string[] { "Arial", "华文行楷", "华文细黑", "宋体", "黑体", "微软雅黑", "方正姚体", "隶书" };
        private static readonly Brush[] _brushes = new Brush[] { Brushes.Black, Brushes.DarkBlue, Brushes.Red, Brushes.Green };
        private static readonly Color[] _lineColors = new Color[] { Color.Silver, Color.LightGray, };
        #endregion

        #region properties
        /// <summary>
        /// 获取图像的宽度。
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// 获取图像的高度。
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// 获取图像的字符个数。
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// 获取或设置随机字符集。
        /// </summary>
        public string RandomChars { get; set; }
        /// <summary>
        /// 获取当前随机字符。
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// 获取当前随机图像。
        /// </summary>
        public Image Image { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建 RandomImage 的实例（大小48x48，4字符）。
        /// </summary>
        public RandomImage()
            : this(48, 24, 4) {
        }
        /// <summary>
        /// 创建 RandomImage 的实例。
        /// </summary>
        /// <param name="width">图像的宽度。</param>
        /// <param name="height">图像的高度。</param>
        public RandomImage(int width, int height)
            : this(width, height, 4) {
        }
        /// <summary>
        /// 创建 RandomImage 的实例。
        /// </summary>
        /// <param name="width">图像的宽度。</param>
        /// <param name="height">图像的高度。</param>
        /// <param name="length">字符个数。</param>
        public RandomImage(int width, int height, int length) {
            Width = width;
            Height = height;
            Length = length;
        }
        #endregion

        #region methods

        #region GetNextFontName
        private static string GetNextFontName() {
            string result = string.Empty;
            Random random = new Random();
            while (result.Length != 0) {
                int index = random.Next(0, _fontNames.Length);
                try {
                    using (Font font = new Font(_fontNames[index], 9)) {
                        result = _fontNames[index];
                    }
                } catch {
                }
            }
            return result;
        }
        #endregion

        #region Execute
        /// <summary>
        /// 以默认的大小和默认的字符个数产生图片
        /// </summary>
        /// <returns></returns>
        public void Execute() {
            Execute(null);
        }
        /// <summary>
        /// 以默认的大小和默认的字符个数产生图片
        /// </summary>
        /// <param name="code">输入指定的码</param>
        /// <returns></returns>
        public void Execute(string code) {
            Bitmap image = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            string randChars = RandomChars;
            if (string.IsNullOrEmpty(randChars))
                randChars = _randCharString;
            string fontName = GetNextFontName();
            string randString = "";
            Random random = new Random();
            if (string.IsNullOrEmpty(code)) {
                do {
                    //使用DateTime.Now.Millisecond作为生成随机数的参数，增加随机性
                    randString += randChars.Substring(random.Next(0, randChars.Length), 1);
                }
                while (randString.Length < Length);
            } else {
                randString = code;
            }
            int border = 1;//边界
            int maxWidth = Width - border * 2;

            float emSize = (float)(maxWidth) / randString.Length;//预想很合适
            Font font = new Font(fontName, emSize);

            //调整到合适为止
            SizeF sizeF;
            while ((sizeF = g.MeasureString(randString, font)).Width > maxWidth || sizeF.Height > Height) {
                emSize--;
                font = new Font(fontName, emSize);
            }

            Pen pen = new Pen(_lineColors[random.Next(0, _lineColors.Length)]);

            #region 画图片的背景噪音线
            int x1, y1, x2, y2;

            for (int i = 0; i < 25; i++) {
                x1 = random.Next(image.Width);
                y1 = random.Next(image.Height);
                x2 = random.Next(image.Width);
                y2 = random.Next(image.Height);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
            #endregion

            #region 画图片的前景噪音点
            for (int i = 0; i < 100; i++) {
                x1 = random.Next(image.Width);
                y1 = random.Next(image.Height);
                image.SetPixel(x1, y1, Color.FromArgb(random.Next(int.MaxValue)));
            }
            #endregion

            float y = ((float)(Height - font.GetHeight(g))) / 2;
            g.DrawString(randString, font, _brushes[random.Next(0, _brushes.Length)], border, y);
            g.Dispose();

            Image = image;
            Value = randString;

        }
        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        /// <param name="disposing">是否为终结。</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing)
                return;
            if (Image != null) {
                Image.Dispose();
                Image = null;
            }
            Value = null;
        }
        #endregion

        #endregion

    }
}