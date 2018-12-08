/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Drawing;

namespace Symbol.Drawing {

    /// <summary>
    /// 水印上下文类
    /// </summary>
    public class ImageWaterMarkContext {
        /// <summary>
        /// 默认边缘距离（5像素）
        /// </summary>
        public static readonly Padding DefaultMargin = new Padding(5);
        /// <summary>
        /// 默认文本字体（Arial，18号，粗体）
        /// </summary>
        public static readonly Font DefaultTextFont = new Font("楷体", 18F, FontStyle.Regular, GraphicsUnit.Pixel);
        /// <summary>
        /// 默认文本背景颜色（Color.WhiteSmoke，雾白色）
        /// </summary>
        public static readonly Color DefaultTextBackColor = Color.WhiteSmoke;
        /// <summary>
        /// 默认文本前景颜色（Color.Black，黑色）
        /// </summary>
        public static readonly Color DefaultTextForeColor = Color.Black;
        /// <summary>
        /// 默认文本边框颜色（Color.DimGray，暗灰色）
        /// </summary>
        public static readonly Color DefaultBorderColor = Color.DimGray;


        /// <summary>
        /// 水印位置
        /// </summary>
        public ImageWaterMarkLocation Location { get; set; }
        /// <summary>
        /// 边缘距离（单位：像素）
        /// </summary>
        public Padding Margin { get; set; }

        /// <summary>
        /// 文本水印
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// 文本字体
        /// </summary>
        public Font TextFont { get; set; }
        /// <summary>
        /// 文本背景颜色
        /// </summary>
        public Color TextBackColor { get; set; }
        /// <summary>
        /// 文本前景颜色
        /// </summary>
        public Color TextForeColor { get; set; }
        /// <summary>
        /// 文本边框颜色
        /// </summary>
        public Color TextBorderColor { get; set; }
        /// <summary>
        /// 透明度，默认是 0.51,即51的透明度，有效值0.00F-1.00F，值越大越不透明。
        /// </summary>
        public float Opacity { get; set; }
        /// <summary>
        /// 图像水印
        /// </summary>
        public Bitmap Image { get; private set; }

        /// <summary>
        /// 创建水印上下文实例（文本水印）
        /// </summary>
        /// <param name="text">文本水印</param>
        /// <param name="location">水印位置，默认为下-右</param>
        public ImageWaterMarkContext(string text, ImageWaterMarkLocation location = ImageWaterMarkLocation.BottomRight)
            : this(location) {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text", "文本水印内容不能为null或“”。");
            Text = text;
        }
        /// <summary>
        /// 创建水印上下文实例（图像水印）
        /// </summary>
        /// <param name="image">图像水印</param>
        /// <param name="location">水印位置，默认为下-右</param>
        public ImageWaterMarkContext(Bitmap image, ImageWaterMarkLocation location = ImageWaterMarkLocation.BottomRight)
            : this(location) {
            if (image == null)
                throw new ArgumentNullException("image", "图像水印不能为null。");
            Image = image;
        }
        private ImageWaterMarkContext(ImageWaterMarkLocation location) {
            Location = location;
            Margin = DefaultMargin;
            TextFont = DefaultTextFont;
            TextBackColor = TextBackColor;
            TextForeColor = DefaultTextForeColor;
            TextBorderColor = DefaultBorderColor;
            Opacity = 0.51F;
        }
    }
}