/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.Encryption {
    /// <summary>
    /// SHA加密辅助类。
    /// </summary>
    public static class SHAEncryptionHelper {

        #region fields
        private static readonly System.Collections.Generic.Dictionary<SHALevels, SHAFunc<System.Security.Cryptography.HashAlgorithm>> _algorithms;
        #endregion

        #region ctor
        static SHAEncryptionHelper() {
            _algorithms = new System.Collections.Generic.Dictionary<SHALevels, SHAFunc<System.Security.Cryptography.HashAlgorithm>>();
            _algorithms.Add(SHALevels.Sha1, () => System.Security.Cryptography.SHA1.Create());
            _algorithms.Add(SHALevels.Sha256, () => System.Security.Cryptography.SHA256.Create());
            _algorithms.Add(SHALevels.Sha384, () => System.Security.Cryptography.SHA384.Create());
            _algorithms.Add(SHALevels.Sha512, () => System.Security.Cryptography.SHA512.Create());
        }
        #endregion

        #region methods

        #region ToString
        private static string ToString(byte[] array) {
            return ByteExtensions.ToHex(array);// System.BitConverter.ToString(array).Replace("-", "");
        }
        #endregion

        #region Encrypt
        /// <summary>
        /// 获取文本内容的Sha值
        /// </summary>
        /// <param name="text">需要处理的文本，为null时自动以""处理。</param>
        /// <param name="level">处理级别，默认为 Sha-1。</param>
        /// <param name="encoding">字符编码，默认以utf-8处理（通用）。</param>
        /// <returns>返回哈希值。</returns>
        public static string Encrypt(string text, SHALevels level = SHALevels.Sha1, System.Text.Encoding encoding=null) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            if (text == null)
                text = string.Empty;
            using (System.Security.Cryptography.HashAlgorithm algorithm = _algorithms[level]()) {
                return ToString(algorithm.ComputeHash(encoding.GetBytes(text)));
            }
        }
        /// <summary>
        /// 获取数据的Sha值
        /// </summary>
        /// <param name="inputStream">需要处理的数据。</param>
        /// <param name="level">处理级别，默认为 Sha-1。</param>
        /// <returns>返回哈希值。</returns>
        public static string Encrypt(System.IO.Stream inputStream,SHALevels level= SHALevels.Sha1) {
            using (System.Security.Cryptography.HashAlgorithm algorithm = _algorithms[level]()) {
                return ToString(algorithm.ComputeHash(inputStream));
            }
        }
        /// <summary>
        /// 获取数据的Sha值
        /// </summary>
        /// <param name="buffer">需要处理的数据。</param>
        /// <param name="level">处理级别，默认为 Sha-1。</param>
        /// <returns>返回哈希值。</returns>
        public static string Encrypt(byte[] buffer,SHALevels level= SHALevels.Sha1) {
            using (System.Security.Cryptography.HashAlgorithm algorithm = _algorithms[level]()) {
                return ToString(algorithm.ComputeHash(buffer));
            }
        }
        /// <summary>
        /// 获取数据的Sha值
        /// </summary>
        /// <param name="buffer">需要处理的数据。</param>
        /// <param name="offset">字节数组中的偏移量，从该位置开始使用数据。</param>
        /// <param name="count">数组中用作数据的字节数。</param>
        /// <param name="level">处理级别，默认为 Sha-1。</param>
        /// <returns>返回哈希值。</returns>
        public static string Encrypt(byte[] buffer,int offset,int count, SHALevels level = SHALevels.Sha1) {
            using (System.Security.Cryptography.HashAlgorithm algorithm = _algorithms[level]()) {
                return ToString(algorithm.ComputeHash(buffer,offset,count));
            }
        }
        #endregion

        #endregion

        #region types
        delegate TResult SHAFunc<TResult>();
        #endregion
    }
    
}