/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.IO;

namespace Symbol.IO.Compression {
    /// <summary>
    /// Gzip辅助类
    /// </summary>
    public static class GzipHelper {

        #region methods

        #region Compress
        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="buffer">需要处理的数据。</param>
        /// <param name="isDeflate">是否为Deflate算法。</param>
        /// <returns>返回处理后的数据。</returns>
        public static byte[] Compress(byte[] buffer, bool isDeflate = false) {
            using (System.IO.Stream inStream = new System.IO.MemoryStream(buffer)) {
                using (System.IO.MemoryStream outStream = Compress(inStream, isDeflate)) {
                    return outStream.ToArray();
                }
            }
        }
        /// <summary>
        /// 压缩到流对象。
        /// </summary>
        /// <param name="stream">一个可读取的流对象。</param>
        /// <param name="isDeflate">是否为Deflate算法。</param>
        /// <returns>返回压缩后的流对象。</returns>
        public static MemoryStream Compress(Stream stream, bool isDeflate = false) {
            MemoryStream result = null;
            bool isSuccess = false;
            try {
                //if (stream.CanSeek) {
                //    result = new MemoryStream((int)stream.Length);
                //} else {
                    result = new MemoryStream();
                //}
                using (Stream compressStream = isDeflate ? (System.IO.Stream)new System.IO.Compression.DeflateStream(result,System.IO.Compression.CompressionMode.Compress,true) : new System.IO.Compression.GZipStream(result,System.IO.Compression.CompressionMode.Compress,true)) {
                    System.IO.StreamExtensions.CopyTo(stream, compressStream);
                    compressStream.Flush();
                    compressStream.Close();
                }
                result.Position = 0;
                isSuccess = true;
            } finally {
                if (!isSuccess && result != null) {
                    result.Close();
                    result.Dispose();
                    result = null;
                }
            }
            return result;
        }
        #endregion

        #region Decompress
        /// <summary>
        /// 解压缩数据。
        /// </summary>
        /// <param name="buffer">需要处理的数据。</param>
        /// <param name="isDeflate">是否为Deflate算法。</param>
        /// <returns>返回处理后的数据。</returns>
        public static byte[] Decompress(byte[] buffer, bool isDeflate = false) {
            using (System.IO.Stream inStream = new System.IO.MemoryStream(buffer)) {
                using (System.IO.MemoryStream outStream = Decompress(inStream, isDeflate)) {
                    return outStream.ToArray();
                }
            }
        }
        /// <summary>
        /// 解压缩到流对象。
        /// </summary>
        /// <param name="stream">一个可读取的流对象。</param>
        /// <param name="isDeflate">是否为Deflate算法。</param>
        /// <returns>返回解压缩后的流对象。</returns>
        public static MemoryStream Decompress(Stream stream, bool isDeflate = false) {
            MemoryStream result = null;
            bool isSuccess = false;
            try {
                //if (stream.CanSeek) {
                //    result = new MemoryStream((int)stream.Length);
                //} else {
                    result = new MemoryStream();
                //}
                using (Stream compressStream = isDeflate ? (System.IO.Stream)new System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Decompress, true) : new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress, true)) {
                    System.IO.StreamExtensions.CopyTo(compressStream, result);
                    result.Position = 0;
                    compressStream.Close();
                }
                isSuccess = true;
            } finally {
                if (!isSuccess && result != null) {
                    result.Close();
                    result.Dispose();
                    result = null;
                }
            }
            return result;
        }
        #endregion

        #endregion
    }
}
