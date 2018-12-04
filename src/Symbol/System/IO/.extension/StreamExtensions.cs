/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace System.IO {
    /// <summary>
    /// Stream扩展类。
    /// </summary>
    public static class StreamExtensions {

        #region methods

        #region ToArray
        /// <summary>
        /// 将流转换为二进制数组
        /// </summary>
        /// <param name="stream">需要处理的流</param>
        /// <returns>返回流的数据。</returns>
        /// <remarks>注意：此操作不会自动关闭传入的流（参数：stream）。</remarks>
        public static byte[] ToArray(
#if !net20
            this 
#endif
            Stream stream) {
            return ToArray(stream, false);
        }
        /// <summary>
        /// 将流转换为二进制数组
        /// </summary>
        /// <param name="stream">需要处理的流</param>
        /// <param name="byStart">是否从头开始读取数据，若流不支持Seek，将从当前位置读取</param>
        /// <returns>返回流的数据。</returns>
        /// <remarks>注意：此操作不会自动关闭传入的流（参数：stream）。</remarks>
        public static byte[] ToArray(
#if !net20
            this 
#endif
            Stream stream, bool byStart) {
            if (stream is MemoryStream)
                return ((MemoryStream)stream).ToArray();

            byte[] result = null;
            using (MemoryStream memoryStream = new MemoryStream()) {
                CopyTo(stream, memoryStream, byStart);
                result = memoryStream.ToArray();
            }
            return result;
        }
        #endregion

        #region CopyTo
        /// <summary>
        /// 复制流到另一个流中
        /// </summary>
        /// <param name="fromStream">被复制的流</param>
        /// <param name="toStream">目标流</param>
        /// <remarks>注意：此操作不会自动关闭传入的流（参数：fromStream，toStream）。</remarks>
        public static void CopyTo(
#if !net20
            this 
#endif
            Stream fromStream, Stream toStream) {
            CopyTo(fromStream, toStream, false);
        }
        /// <summary>
        /// 复制流到另一个流中
        /// </summary>
        /// <param name="fromStream">被复制的流</param>
        /// <param name="toStream">目标流</param>
        /// <param name="byStart">是否从头开始读取数据，若流不支持Seek，将从当前位置读取</param>
        /// <remarks>注意：此操作不会自动关闭传入的流（参数：fromStream，toStream）。</remarks>
        public static void CopyTo(
#if !net20
            this 
#endif
            Stream fromStream, Stream toStream, bool byStart) {
            long oldPosition = 0;
            if (fromStream.CanSeek) {
                oldPosition = fromStream.Position;
                if (byStart)
                    fromStream.Position = 0;
            }

            byte[] buffer = null;
            int size = 0;
            try {
                buffer = new byte[1024];

                while ((size = fromStream.Read(buffer, 0, buffer.Length)) > 0) {
                    toStream.Write(buffer, 0, size);
                }
            } finally {
                buffer = null;
            }
            if (fromStream.CanSeek)
                fromStream.Position = oldPosition;
        }
        #endregion

        #region ToFile
        /// <summary>
        /// 保存流到文件，从流的当前位置读取
        /// </summary>
        /// <param name="stream">要保存的流</param>
        /// <param name="path">保存的位置</param>
        /// <remarks>注意：此操作不会自动关闭传入的流（参数：stream）。</remarks>
        public static void ToFile(
#if !net20
            this 
#endif
            Stream stream, string path) {
            ToFile(stream, path, false);
        }
        /// <summary>
        /// 保存流到文件
        /// </summary>
        /// <param name="stream">要保存的流</param>
        /// <param name="path">保存的位置</param>
        /// <param name="byStart">是否从头开始读取数据，若流不支持Seek，将从当前位置读取</param>
        /// <remarks>注意：此操作不会自动关闭传入的流（参数：stream）。</remarks>
        public static void ToFile(
#if !net20
            this 
#endif
            Stream stream, string path, bool byStart) {
            ThreadHelper.ParallelLock("file", path).Block(() => {
                AppHelper.DeleteFile(path);
                using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    CopyTo(stream, fileStream, byStart);
                }
            });
        }
        #endregion

        #endregion

    }
}