/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace System.Drawing {
    /// <summary>
    /// Bitmap扩展类。
    /// </summary>
    public static class BitmapExtensions {

        #region methods

        #region AutoZool
        /// <summary>
        /// 自动等比例缩放图片。
        /// </summary>
        /// <param name="bitmap">需要处理的图像。</param>
        /// <param name="newSize">新的大小（像素）。</param>
        /// <returns>返回新的图像（如果图片过小，将还是bitmap自身）。</returns>
        public static Bitmap AutoZool(
#if !net20
            this
#endif
            Bitmap bitmap, Size newSize) {
            return AutoZool(bitmap, newSize.Width, newSize.Height);
        }
        /// <summary>
        /// 自动等比例缩放图片。
        /// </summary>
        /// <param name="bitmap">需要处理的图像。</param>
        /// <param name="width">新的宽度（像素）。</param>
        /// <param name="height">新的高度（像素）。</param>
        /// <returns>返回新的图像（如果图片过小，将还是bitmap自身）。</returns>
        public static Bitmap AutoZool(
#if !net20
            this
#endif
            Bitmap bitmap, int width, int height) {
            Size oldSize = bitmap.Size;
            if (oldSize.Height < height && oldSize.Width < width)
                return bitmap;

            float oldScale = (float)oldSize.Width / oldSize.Height;
            float newScale = (float)width / height;
            float newWidth = 0;
            float newHeight = 0;
            if (oldScale > newScale) {
                newWidth = width;
                newHeight = newWidth / oldScale;
            } else {
                newHeight = height;
                newWidth = newHeight * oldScale;
            }
            //float x = Math.Abs(newWidth - width) / 2F;
            //float y = Math.Abs(newHeight - height) / 2F;
            Bitmap result = new Bitmap((int)newWidth, (int)newHeight);
            result.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
            Graphics g2 = Graphics.FromImage(result);
            g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g2.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g2.DrawImage(bitmap, new Rectangle(/*(int)x, (int)y*/0, 0, (int)newWidth, (int)newHeight), 0F, 0F, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
            g2.Dispose();
            return result;
        }
        #endregion
        #region Zool
        /// <summary>
        /// 缩放图像（非等比例）
        /// </summary>
        /// <param name="image">需要处理的图像。</param>
        /// <param name="newSize">新的大小（像素）。</param>
        /// <returns>返回新的图像。</returns>
        public static Bitmap Zool(
#if !net20
            this
#endif
            Image image, Size newSize) {
            return Zool(image, newSize.Width, newSize.Height);
        }
        /// <summary>
        /// 缩放图像（非等比例）
        /// </summary>
        /// <param name="image">需要处理的图像。</param>
        /// <param name="width">新的宽度（像素）。</param>
        /// <param name="height">新的高度（像素）。</param>
        /// <returns>返回新的图像。</returns>
        public static Bitmap Zool(
#if !net20
            this
#endif
            Image image, int width, int height) {
            Bitmap result = new Bitmap(width, height, image.PixelFormat);
            using (Graphics g = Graphics.FromImage(result)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.DrawImage(image, 0, 0, width, height);
            }
            return result;
        }
        #endregion

        #region Crop
        /// <summary>
        /// 裁剪（自适应等比例）
        /// </summary>
        /// <param name="image">需要处理的图像。</param>
        /// <param name="newSize">新的大小（像素）。</param>
        /// <returns>返回新的图像。</returns>
        public static Bitmap Crop(
#if !net20
        this
#endif
            Image image, Size newSize) {
            return Crop(image, newSize.Width, newSize.Height);
        }
        /// <summary>
        /// 裁剪（自适应等比例）
        /// </summary>
        /// <param name="image">需要处理的图像。</param>
        /// <param name="width">新的宽度（像素）。</param>
        /// <param name="height">新的高度（像素）。</param>
        /// <returns>返回新的图像。</returns>
        public static Bitmap Crop(
#if !net20
            this
#endif
            Image image, int width, int height) {
            float original_width = image.Width;
            float original_height = image.Height;
            float original_x = 0F;
            float original_y = 0F;

            float crop_width = width;
            float crop_height = height;

            if (crop_width <= crop_height) {
                //目标：高图

                if (original_width >= original_height) {
                    //源图：宽图
                    //  高比＝源高/目标高
                    //  新高＝源高
                    //  新宽＝目标宽*比例
                    //  X=（源宽－新宽）/2
                    //  Y=0

                    //高比
                    float scale = original_height / crop_height;

                    //新宽 新高
                    crop_height = original_height;
                    crop_width *= scale;
                } else {
                    //源图：高图
                    //  宽比＝源宽/目标宽
                    //  新宽＝源宽
                    //  新高＝目标高*比例
                    //  X=0
                    //  Y=（源高－新高）/2

                    //高比
                    float scale = original_width / crop_width;

                    //新宽 新高
                    crop_width = original_width;
                    crop_height *= scale;
                    original_x = 0F;
                    if (crop_height > original_height) {
                        scale = original_height / crop_height;
                        crop_height = original_height;
                        crop_width *= scale;
                    }
                }
            } else {
                //目标：宽图
                if (original_width < original_height) {
                    //源图：高图
                    //  宽比＝源宽/目标宽
                    //  新宽＝源宽
                    //  新高＝目标高*比例
                    //  X=0
                    //  Y=（源高－新高）/2

                    //高比
                    float scale = original_width / crop_width;

                    //新宽 新高
                    crop_width = original_width;
                    crop_height *= scale;

                } else {
                    //源图：宽图
                    //  高比＝源高/目标高
                    //  新高＝源高
                    //  新宽＝目标宽*比例
                    //  X=（源宽－新宽）/2
                    //  Y=0

                    //高比
                    float scale = original_height / crop_height;

                    //新宽 新高
                    crop_height = original_height;
                    crop_width *= scale;
                    original_y = 0F;
                    if (crop_width > original_width) {
                        scale = original_width / crop_width;
                        crop_width = original_width;
                        crop_height *= scale;
                        original_y = (original_height - crop_height) / 2F;
                    }
                }
            }
            //求出X Y
            original_x = (original_width - crop_width) / 2F;
            original_y = (original_height - crop_height) / 2F;

            var image_crop = new Bitmap((int)crop_width, (int)crop_height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = System.Drawing.Graphics.FromImage(image_crop)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.Clear(System.Drawing.Color.Transparent);
                g.DrawImage(image, new System.Drawing.RectangleF(0F, 0F, image_crop.Width, image_crop.Height), new System.Drawing.RectangleF(original_x, original_y, crop_width, crop_height), System.Drawing.GraphicsUnit.Pixel);
            }
            return image_crop;
        }
        #endregion

        #region ToBitmap
        /// <summary>
        /// 转换为图像。
        /// </summary>
        /// <param name="buffer">需要处理的数据。</param>
        /// <returns>返回转换后的图像。</returns>
        public static Bitmap ToBitmap(
#if !net20
            this
#endif
            byte[] buffer) {
            using (System.IO.MemoryStream stream = new IO.MemoryStream(buffer)) {
                return new Bitmap(stream);
            }
        }
        /// <summary>
        /// 转换为图像。
        /// </summary>
        /// <param name="buffer">需要处理的数据。</param>
        /// <param name="useIcm">如果要为此 System.Drawing.Bitmap 使用颜色校正，则为 true；否则为 false。</param>
        /// <returns>返回转换后的图像。</returns>
        public static Bitmap ToBitmap(
#if !net20
            this
#endif
            byte[] buffer, bool useIcm) {
            using (System.IO.MemoryStream stream = new IO.MemoryStream(buffer)) {
                return new Bitmap(stream, useIcm);
            }
        }
        /// <summary>
        /// 转换为图像。
        /// </summary>
        /// <param name="stream">一个可读取的流对象。</param>
        /// <returns>返回转换后的图像。</returns>
        public static Bitmap ToBitmap(
#if !net20
            this
#endif
            System.IO.Stream stream) {
            return new Bitmap(stream);
        }
        /// <summary>
        /// 转换为图像。
        /// </summary>
        /// <param name="stream">一个可读取的流对象。</param>
        /// <param name="useIcm">如果要为此 System.Drawing.Bitmap 使用颜色校正，则为 true；否则为 false。</param>
        /// <returns>返回转换后的图像。</returns>
        public static Bitmap ToBitmap(
#if !net20
            this
#endif
            System.IO.Stream stream, bool useIcm) {
            return new Bitmap(stream, useIcm);
        }
        #endregion


        #endregion
    }

}