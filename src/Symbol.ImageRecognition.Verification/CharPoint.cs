/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 字符点
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{X}x{Y}=rgb({R},{G},{B})")]
    [Symbol.IO.Packing.PropertyPackage]
    public class CharPoint {

        #region properties
        /// <summary>
        /// 原始X
        /// </summary>
        public int OriginalX { get; set; }
        /// <summary>
        /// 原始Y
        /// </summary>
        public int OriginalY { get; set; }
        /// <summary>
        /// X
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// R
        /// </summary>
        public byte R { get; set; }
        /// <summary>
        /// G
        /// </summary>
        public byte G { get; set; }
        /// <summary>
        /// B
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// 左侧点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint Left { get; set; }

        /// <summary>
        /// 右侧点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint Right { get; set; }

        /// <summary>
        /// 上方点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint Top { get; set; }

        /// <summary>
        /// 下方点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint Bottom { get; set; }

        /// <summary>
        /// 左上角点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint LeftTop { get; set; }

        /// <summary>
        /// 右上角点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint RightTop { get; set; }

        /// <summary>
        /// 左下角点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint LeftBottom { get; set; }

        /// <summary>
        /// 右下角点
        /// </summary>
        [Symbol.IO.Packing.NonPackage]
        public CharPoint RightBottom { get; set; }

        #endregion

        #region methods

        #region ToString
        /// <summary>
        /// 输出摘要
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0},{1}=[{2},{3},{4}]", X, Y, R, G, B);
        }
        #endregion

        #endregion
    }

}
