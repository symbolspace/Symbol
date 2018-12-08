/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace System.Drawing {
    /// <summary>
    /// 颜色辅助类。
    /// </summary>
    public class ColorHelper {

        #region methods

        #region ToArgb
        private static readonly int _argbMinValue = System.Drawing.Color.Black.ToArgb();
        /// <summary>
        /// 将argb四个值转换为int值
        /// </summary>
        /// <param name="red">红色，0-255。</param>
        /// <param name="green">绿色，0-255。</param>
        /// <param name="blue">蓝色，0-255。</param>
        /// <returns>返回Color.ToArgb的值。</returns>
        public static int ToArgb(byte red, byte green, byte blue) {
            return ToArgb(255, red, green, blue);
        }
        /// <summary>
        /// 将argb四个值转换为int值
        /// </summary>
        /// <param name="alpha">透明度，0-255。</param>
        /// <param name="red">红色，0-255。</param>
        /// <param name="green">绿色，0-255。</param>
        /// <param name="blue">蓝色，0-255。</param>
        /// <returns>返回Color.ToArgb的值。</returns>
        public static int ToArgb(byte alpha, byte red, byte green, byte blue) {
            byte[] buffer = new byte[] { blue, green, red, alpha };
            int result;
            unsafe {
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, (System.IntPtr)(&result), 4);
            }
            return result;
            //if (alpha == red && red == green && green == blue && blue == 0)
            //    return 0;
            //if (alpha == 255) {
            //    return ((red * 256 + green) * 256 + blue) + _argbMinValue;
            //} else {
            //    return (((alpha * 256 + red) * 256 + green) * 256 + blue);
            //}
        }
        #endregion
        #region ToBytes
        /// <summary>
        /// 将Argb转换byte数组
        /// </summary>
        /// <param name="argb">Color.ToArgb值</param>
        /// <returns>返回Argb数组。</returns>
        public static byte[] ToBytes(int argb) {
            byte[] buffer = new byte[4];
            unsafe {
                System.Runtime.InteropServices.Marshal.Copy((System.IntPtr)(&argb), buffer, 0, 4);
            }
            return buffer;
        }
        /// <summary>
        /// 将rgb转换byte数组
        /// </summary>
        /// <param name="red">红色，0-255。</param>
        /// <param name="green">绿色，0-255。</param>
        /// <param name="blue">蓝色，0-255。</param>
        /// <returns>返回Argb数组。</returns>
        public static byte[] ToBytes(byte red, byte green, byte blue) {
            return new byte[] { blue, green, red, 255 };
        }
        /// <summary>
        /// 将argb四个值转换byte数组
        /// </summary>
        /// <param name="alpha">透明度，0-255。</param>
        /// <param name="red">红色，0-255。</param>
        /// <param name="green">绿色，0-255。</param>
        /// <param name="blue">蓝色，0-255。</param>
        /// <returns>返回Argb数组。</returns>
        public static byte[] ToBytes(byte alpha, byte red, byte green, byte blue) {
            return new byte[] { blue, green, red, alpha };
        }
        #endregion

        #endregion
    }
}