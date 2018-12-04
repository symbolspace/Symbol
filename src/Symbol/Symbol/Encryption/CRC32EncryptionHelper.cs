/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Encryption {
    /// <summary>
    /// CRC32校验码处理类
    /// </summary>
    /// <remarks>如果需要得到16进制的CRC码，请用ulong的ToString("X2")</remarks>
    public static class CRC32EncryptionHelper {

        #region fields
        /// <summary>
        /// 空白的CRC32值。
        /// </summary>
        public const ulong Empty = 0xFFFFFFFF;
        private static readonly ulong[] _crc32Table;//采用static readonly 防止被AppDomain回收。

        #endregion

        #region cctor
        static CRC32EncryptionHelper() {
            //生成CRC32码表，有的算法是写死的，这里偷点懒，直接算出来的
            //    如果觉得这个会影响的话，可以自己写死_crc32Table
            //    这是在类调用前执行的，只会执行一次
            _crc32Table = new ulong[256];
            ulong crc;
            for (int i = 0; i < 256; i++) {
                crc = (ulong)i;
                for (int j = 8; j > 0; j--) {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
                _crc32Table[i] = crc;
            }
        }
        #endregion

        #region methods

        #region Encrypt
        /// <summary>
        /// 获取字符串的CRC32校验码（UTF-8）
        /// </summary>
        /// <param name="value">需要校验的字符串，null或"" 不会报错。</param>
        /// <returns>返回CRC32校验码</returns>
        /// <remarks>为了获得最好的兼容性，所以采用通用的UTF-8编码，如果需要用别的编码，请转为byte[]后调用另外的重载。</remarks>
        public static ulong Encrypt(string value) {
            return Encrypt(string.IsNullOrEmpty(value) ? null : System.Text.Encoding.UTF8.GetBytes(value), -1, -1);
        }
        /// <summary>
        /// 获取二进制数组的CRC32校验码
        /// </summary>
        /// <param name="buffer">为null或空数组时不会报错。</param>
        /// <returns>返回CRC32校验码</returns>
        public static ulong Encrypt(byte[] buffer) {
            return Encrypt(buffer, -1, -1);
        }
        /// <summary>
        /// 获取二进制数组的CRC32校验码
        /// </summary>
        /// <param name="buffer">为null或空数组时不会报错。</param>
        /// <param name="offset">从什么位置开始，-1表示从0开始</param>
        /// <param name="length">需要多长，-1表示剩下的长度。</param>
        /// <returns>返回CRC32校验码</returns>
        public static ulong Encrypt(byte[] buffer, int offset, int length) {
            ulong result = 0xffffffff;
            if (buffer!=null) {
                if(offset<0)
                    offset=0;
                if(length>0)
                    length= offset+length;
                else
                    length= buffer.Length;
                for (int i = offset; i < length; i++) {
                    result = (result >> 8) ^ _crc32Table[(result & 0xFF) ^ buffer[i]];
                }
            }
            return result;
        }

        /// <summary>
        /// 获取二进制数组的CRC32校验码
        /// </summary>
        /// <param name="stream">需要处理的流对象，null或不可读的流不会报错。</param>
        /// <returns>返回CRC32校验码</returns>
        public static ulong Encrypt(System.IO.Stream stream) {
            ulong result = 0xffffffff;
            if (stream != null && stream.CanRead) {
                int b = -1;
                while ((b = stream.ReadByte()) != -1) {
                    result = (result >> 8) ^ _crc32Table[(result & 0xFF) ^ (byte)b];
                }                    
            }
            return result;
        }
        #endregion

        #region CRC
        /// <summary>
        /// 获取字符串的CRC32校验码（UTF-8）
        /// </summary>
        /// <param name="value">需要校验的字符串，null或"" 不会报错。</param>
        /// <returns>返回CRC32校验码</returns>
        /// <remarks>为了获得最好的兼容性，所以采用通用的UTF-8编码，如果需要用别的编码，请转为byte[]后调用另外的重载。</remarks>
        [System.Obsolete("请改用 Symbol.Encryption.CRC32EncryptionHelper.Encrypt(string value)")]
        public static ulong CRC(string value) {
            return Encrypt(string.IsNullOrEmpty(value) ? null : System.Text.Encoding.UTF8.GetBytes(value), -1, -1);
        }
        /// <summary>
        /// 获取二进制数组的CRC32校验码
        /// </summary>
        /// <param name="buffer">为null或空数组时不会报错。</param>
        /// <returns>返回CRC32校验码</returns>
        [System.Obsolete("请改用 Symbol.Encryption.CRC32EncryptionHelper.Encrypt(byte[] buffer)")]
        public static ulong CRC(byte[] buffer) {
            return Encrypt(buffer, -1, -1);
        }

        /// <summary>
        /// 获取二进制数组的CRC32校验码
        /// </summary>
        /// <param name="buffer">为null或空数组时不会报错。</param>
        /// <param name="offset">从什么位置开始，-1表示从0开始</param>
        /// <param name="length">需要多长，-1表示剩下的长度。</param>
        /// <returns>返回CRC32校验码</returns>
        [System.Obsolete("请改用 Symbol.Encryption.CRC32EncryptionHelper.Encrypt(byte[] buffer, int offset, int length)")]
        public static ulong CRC(byte[] buffer, int offset, int length) {
            return Encrypt(buffer, offset, length);
        }
        /// <summary>
        /// 获取二进制数组的CRC32校验码
        /// </summary>
        /// <param name="stream">需要处理的流对象，null或不可读的流不会报错。</param>
        /// <returns>返回CRC32校验码</returns>
        [System.Obsolete("请改用 Symbol.Encryption.CRC32EncryptionHelper.Encrypt(System.IO.Stream stream)")]
        public static ulong CRC(System.IO.Stream stream) {
            return Encrypt(stream);
        }
        #endregion

        #endregion
    }
}