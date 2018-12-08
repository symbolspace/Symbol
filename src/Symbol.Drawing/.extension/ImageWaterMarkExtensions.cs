/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

#if !net20

using System.Drawing;

namespace Symbol.Drawing {
    /// <summary>
    /// 图像水印扩展类
    /// </summary>
    public static class ImageWaterMarkExtensions {

        #region methods

        #region WaterMark
        /// <summary>
        /// 为图像加上水印（忽略gif格式），直接在原始图像上做处理。
        /// </summary>
        /// <param name="bitmap">需要处理的图像</param>
        /// <param name="context">水印上下文实例</param>
        /// <returns>返回是否处理过图像。</returns>
        public static bool WaterMark(
            this 
            Bitmap bitmap, ImageWaterMarkContext context) {
            return ImageWaterMark.WaterMark(bitmap, context);
        }
        #endregion

        #endregion
    }

}
#endif