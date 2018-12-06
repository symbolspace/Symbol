/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Web {
    /// <summary>
    /// 提供对客户端已上载的单独文件的访问。
    /// </summary>
    public class HttpPostedFile :
        System.MarshalByRefObject,
        IHttpPostedFile,
        System.IDisposable {

        #region fields
        private System.IO.Stream _inputStream = null;
        private string _filename = null;
        private string _contentType = null;
        private int _contentLength = -1;
        private InputStreamGetter _inputStreamGetter;
        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpPostedFile实例。
        /// </summary>
        /// <param name="inputStream">一个 System.IO.Stream 对象，该对象指向一个上载文件，以准备读取该文件的内容。</param>
        /// <param name="filename">客户端上的文件的完全限定名称（需要注意的是，有些移动设备故意不提交文件名。）。</param>
        /// <param name="contentLength">获取上载文件的大小（以字节为单位）。</param>
        /// <param name="contentType">客户端发送的文件的 MIME 内容类型。比如png图片，可能会是image/png。</param>
        public HttpPostedFile(System.IO.Stream inputStream, string filename, int contentLength, string contentType) {
            _inputStream = inputStream;
            _filename = filename;
            _contentLength = contentLength;
            _contentType = contentType;
        }
        /// <summary>
        /// 创建HttpPostedFile实例。
        /// </summary>
        /// <param name="inputStreamGetter">一个 System.IO.Stream 对象，该对象指向一个上载文件，以准备读取该文件的内容。</param>
        /// <param name="filename">客户端上的文件的完全限定名称（需要注意的是，有些移动设备故意不提交文件名。）。</param>
        /// <param name="contentLength">获取上载文件的大小（以字节为单位）。</param>
        /// <param name="contentType">客户端发送的文件的 MIME 内容类型。比如png图片，可能会是image/png。</param>
        public HttpPostedFile(InputStreamGetter inputStreamGetter, string filename, int contentLength, string contentType) {
            _inputStreamGetter = inputStreamGetter;
            _filename = filename;
            _contentLength = contentLength;
            _contentType = contentType;
        }
        #endregion
        #region IHttpPostedFile 成员

        /// <summary>
        /// 获取上载文件的大小（以字节为单位）。
        /// </summary>
        /// <returns>文件长度（以字节为单位）。</returns>
        public int ContentLength {
            get { return _contentLength; }
        }

        /// <summary>
        /// 获取客户端发送的文件的 MIME 内容类型。比如png图片，可能会是image/png。
        /// </summary>
        /// <returns>上载文件的 MIME 内容类型。</returns>
        public string ContentType {
            get { return _contentType; }
        }

        /// <summary>
        /// 获取客户端上的文件的完全限定名称（需要注意的是，有些移动设备故意不提交文件名。）。
        /// </summary>
        /// <returns>客户端的文件的名称，包含目录路径。</returns>
        public string FileName {
            get { return _filename; }
        }

        /// <summary>
        /// 获取一个 System.IO.Stream 对象，该对象指向一个上载文件，以准备读取该文件的内容。
        /// </summary>
        /// <returns>指向文件的 System.IO.Stream。</returns>
        public System.IO.Stream InputStream {
            get {
                lb_retry:
                var value = System.Threading.Interlocked.CompareExchange(ref _inputStream, null, null);
                if (value == null) {
                    System.Threading.Interlocked.Exchange(ref _inputStream, _inputStreamGetter());
                    goto lb_retry;
                }
                return value;
            }
        }

        /// <summary>
        /// 保存上载文件的内容。
        /// </summary>
        /// <param name="filename">保存的文件的名称（必须是绝对路径，不能是网址。）。</param>
        public void SaveAs(string filename) {
            AppHelper.DeleteFile(filename);
            System.IO.StreamExtensions.ToFile(_inputStream, filename);
            //System.IO.File.WriteAllBytes(filename, _inputStream.ToArray());
        }

        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 
        /// </summary>
        ~HttpPostedFile() {
            Dispose(false);
        }
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        void Dispose(bool dispsing) {
            if (dispsing) {
                System.Threading.Interlocked.Exchange(ref _inputStreamGetter, null);
                var stream = System.Threading.Interlocked.Exchange(ref _inputStream, null);
                if(stream!=null){
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                _filename = null;
                //System.GC.Collect(0);
                //System.GC.Collect();
            }
        }

        #endregion

        #region types
        /// <summary>
        /// 文件流获取委托。
        /// </summary>
        /// <returns>文件流。</returns>
        public delegate System.IO.Stream InputStreamGetter();
        #endregion
    }
}