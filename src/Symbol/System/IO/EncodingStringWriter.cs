/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace System.IO {

    /// <summary>
    /// 带有编码的StringWriter
    /// </summary>
    [System.Serializable]
    public class EncodingStringWriter : System.IO.StringWriter {

        #region fields
        private System.Text.Encoding _encoding;
        #endregion

        #region properties
        /// <summary>
        /// 获取当前采用的编码。
        /// </summary>
        public override System.Text.Encoding Encoding {
            get {
                return _encoding;
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创造一个实例，将采用utf-8编码
        /// </summary>
        public EncodingStringWriter()
            : this(null) {
        }
        /// <summary>
        /// 创建一个实例，并指定编码
        /// </summary>
        /// <param name="encoding">如果传值为null，将视为utf-8编码。</param>
        public EncodingStringWriter(System.Text.Encoding encoding) {
            if (encoding == null)
                _encoding = System.Text.Encoding.UTF8;
            else
                _encoding = encoding;
        }
        #endregion

    }
}