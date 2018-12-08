/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Drawing;
using System.Drawing.Imaging;

namespace Symbol.Drawing {

    /// <summary>
    /// 图像水印
    /// </summary>
    public static class ImageWaterMark {

        #region methods

        #region WaterMark
        /// <summary>
        /// 为图像加上水印（忽略gif格式），直接在原始图像上做处理。
        /// </summary>
        /// <param name="bitmap">需要处理的图像</param>
        /// <param name="context">水印上下文实例</param>
        /// <returns>返回是否处理过图像。</returns>
        public static bool WaterMark(
            Bitmap bitmap, ImageWaterMarkContext context) {
            if (bitmap == null || context == null)
                return false;
            //Gif不加水印的
            if (bitmap.RawFormat == ImageFormat.Gif)
                return false;
            float width;//水印宽
            float height;//水印高
            float x = 0F;//坐标.x
            float y = 0F;//坐标.y
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Bitmap image = null;
            if (context.Image != null) {
                //图像水印
                width = context.Image.Width;
                height = context.Image.Height;
                image = context.Image;
            } else {
                //文本水印
                if (context.TextFont == null)
                    context.TextFont = ImageWaterMarkContext.DefaultTextFont;
                SizeF size = g.MeasureString(context.Text, context.TextFont);
                width = size.Width + 10F;
                height = size.Height + 6F;
                image = new Bitmap((int)width, (int)height);
                Graphics g2 = Graphics.FromImage(image);
                g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g2.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                if (context.TextBackColor == null)
                    context.TextBackColor = ImageWaterMarkContext.DefaultTextBackColor;
                if (context.TextForeColor == null)
                    context.TextForeColor = ImageWaterMarkContext.DefaultTextForeColor;
                if (context.TextBorderColor == null)
                    context.TextBorderColor = ImageWaterMarkContext.DefaultBorderColor;
                g2.Clear(context.TextBackColor);
                g2.DrawRectangle(new Pen(context.TextBorderColor, 1F), 0F, 0F, width - 2F, height - 2F);
                g2.DrawString(context.Text, context.TextFont, new SolidBrush(context.TextForeColor), 3, 3);
                g2.Dispose();
                g2 = null;
            }
            //水印超出范围
            if (width * 2 >= bitmap.Width || height * 2 >= bitmap.Height)
                return false;

            //推算坐标
            switch (context.Location) {
                case ImageWaterMarkLocation.TopLeft:
                    x = context.Margin.Left;
                    y = context.Margin.Top;
                    break;
                case ImageWaterMarkLocation.TopCenter:
                    x = (bitmap.Width - width) / 2; y = context.Margin.Top;
                    break;
                case ImageWaterMarkLocation.TopRight:
                    x = bitmap.Width - context.Margin.Right - width; y = context.Margin.Top;
                    break;
                case ImageWaterMarkLocation.MiddleLeft:
                    x = context.Margin.Left; y = (bitmap.Height - height) / 2;
                    break;
                case ImageWaterMarkLocation.MiddleCenter:
                    x = (bitmap.Width - width) / 2; y = (bitmap.Height - height) / 2;
                    break;
                case ImageWaterMarkLocation.MiddleRight:
                    x = bitmap.Width - context.Margin.Right - width; y = (bitmap.Height - height) / 2;
                    break;
                case ImageWaterMarkLocation.BottomLeft:
                    x = context.Margin.Left; y = bitmap.Height - height - context.Margin.Bottom;
                    break;
                case ImageWaterMarkLocation.BottomCenter:
                    x = (bitmap.Width - width) / 2; y = bitmap.Height - height - context.Margin.Bottom;
                    break;
                case ImageWaterMarkLocation.BottomRight:
                    x = bitmap.Width - context.Margin.Right - width; y = bitmap.Height - height - context.Margin.Bottom;
                    break;
            }
            if (x < 0F || y < 0F)
                return false;
            //图像水印
            image.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetRemapTable(new ColorMap[]{
                    new ColorMap(){
                        OldColor= Color.FromArgb(255,0,255,0),
                        NewColor= Color.FromArgb(0,0,0,0),
                    }
                }, ColorAdjustType.Bitmap);
            if (context.Opacity < 0F || context.Opacity > 1F)
                context.Opacity = 0.51F;
            imageAttributes.SetColorMatrix(new ColorMatrix(new float[][]{
                    new float[]{1.0F,0.0F,0.0F,0.0F,0.0F},
                    new float[]{0.0F,1.0F,0.0F,0.0F,0.0F},
                    new float[]{0.0F,0.0F,1.0F,0.0F,0.0F},
                    new float[]{0.0F,0.0F,0.0F,context.Opacity,0.0F},//65透明度
                    new float[]{0.0F,0.0F,0.0F,0.0F,1.0F},
                }), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            g.DrawImage(image, new Rectangle((int)x, (int)y, (int)width, (int)height), 0, 0, width, height, GraphicsUnit.Pixel, imageAttributes);

            g.Dispose();
            g = null;
            if (context.Image == null) {
                image.Dispose();
                image = null;
            }
            return true;
        }
        #endregion

        #endregion
    }

}