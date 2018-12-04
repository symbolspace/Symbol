/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Security.Cryptography;

namespace Symbol.Encryption {
    /// <summary>
    /// Md5加密辅助类。
    /// </summary>
    public static class MD5EncryptionHelper {

        #region methods

        #region MD5ToString
        private static string MD5ToString(byte[] array) {
            return ByteExtensions.ToHex(array);// System.BitConverter.ToString(array).Replace("-", "");
        }
        #endregion

        #region GetMD5
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="text">需要处理的文本。</param>
        /// <returns>返回Md5值。</returns>
        [Obsolete("请改用 Symbol.Encryption.MD5EncryptionHelper.Encrypt(string text) 。")]
        public static string GetMD5(string text) {
            return Encrypt(text);
        }
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="array">需要处理的数据。</param>
        /// <returns>返回Md5值。</returns>
        [Obsolete("请改用 Symbol.Encryption.MD5EncryptionHelper.Encrypt(byte[] array) 。")]
        public static string GetMD5(byte[] array) {
            return Encrypt(array);
        }
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="array">需要处理的数据。</param>
        /// <param name="offset">超始位置。</param>
        /// <param name="count">处理长度。</param>
        /// <returns>返回Md5值。</returns>
        [Obsolete("请改用 Symbol.Encryption.MD5EncryptionHelper.Encrypt(byte array, int offset, int count) 。")]
        public static string GetMD5(byte[] array, int offset, int count) {
            return Encrypt(array, offset, count);
        }
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="stream">一个可以读取的流对象。</param>
        /// <returns>返回Md5值。</returns>
        [Obsolete("请改用 Symbol.Encryption.MD5EncryptionHelper.Encrypt(System.IO.Stream stream) 。")]
        public static string GetMD5(System.IO.Stream stream) {
            return Encrypt(stream);
        }
        #endregion
        #region Encrypt
        /// <summary>
        /// 获取Md5值（Encoding.Default）。
        /// </summary>
        /// <param name="text">需要处理的文本。</param>
        /// <returns>返回Md5值。</returns>
        public static string Encrypt(string text) {
#if netcore
            byte[] array = System.Text.Encoding.UTF8.GetBytes(text == null ? string.Empty : text);
#else
            byte[] array = System.Text.Encoding.Default.GetBytes(text == null ? string.Empty : text);
#endif
            return Encrypt(array);
        }
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="text">需要处理的文本。</param>
        /// <param name="encoding">默认为utf-8</param>
        /// <returns>返回Md5值。</returns>
        public static string Encrypt(string text, System.Text.Encoding encoding) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            return Encrypt(encoding.GetBytes(text ?? string.Empty));
        }
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="array">需要处理的数据。</param>
        /// <returns>返回Md5值。</returns>
        public static string Encrypt(byte[] array) {
            return MD5ToString(MD5.Create().ComputeHash(array));
        }
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="array">需要处理的数据。</param>
        /// <param name="offset">超始位置。</param>
        /// <param name="count">处理长度。</param>
        /// <returns>返回Md5值。</returns>
        public static string Encrypt(byte[] array, int offset, int count) {
            return MD5ToString(MD5.Create().ComputeHash(array, offset, count));
        }
        /// <summary>
        /// 获取Md5值。
        /// </summary>
        /// <param name="stream">一个可以读取的流对象。</param>
        /// <returns>返回Md5值。</returns>
        public static string Encrypt(System.IO.Stream stream) {
            return MD5ToString(MD5.Create().ComputeHash(stream));
        }
        #endregion

        #region ToMD5
//        /// <summary>
//        /// 获取Md5值
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public static string ToMD5(
//#if !net20
//            this
//#endif
//            string value) {
//            if (string.IsNullOrEmpty(value))
//                throw new ArgumentNullException("value");
//            return Encrypt(value);
//        }
//        /// <summary>
//        /// 获取Md5值
//        /// </summary>
//        /// <param name="value"></param>
//        /// <param name="offset"></param>
//        /// <param name="count"></param>
//        /// <returns></returns>
//        public static string ToMD5(
//#if !net20
//            this
//#endif
//            string value, int offset, int count) {
//            if (string.IsNullOrEmpty(value))
//                throw new ArgumentNullException("value");
//            byte[] array = System.Text.UTF8Encoding.Default.GetBytes(value == null ? string.Empty : value);
//            return Encrypt(array, offset, count);
//        }
        #endregion

        #region ShortMd5
        /// <summary>
        /// 获取文档的Md5值（16位小写字母）。
        /// </summary>
        /// <param name="text">文本内容。</param>
        /// <param name="encoding">编码，为null时将采用utf-8编码。</param>
        /// <returns>返回md5值，16位小写字母。</returns>
        public static string ShortMd5(string text, System.Text.Encoding encoding = null) {
            string hash = BitConverter.ToString(MD5.Create().ComputeHash((encoding == null ? System.Text.Encoding.UTF8 : encoding).GetBytes(text == null ? "" : text)), 4, 8);
            hash = hash.Replace("-", "").ToLower();
            return hash;
        }
        #endregion

        #endregion

    }
}