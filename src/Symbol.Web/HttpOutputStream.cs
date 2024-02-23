/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Web {
    /// <summary>
    /// Http输出流包装类。
    /// </summary>
    public class HttpOutputStream : System.IO.Stream {

        #region fields
        private WriteDelegate _writeAction;
        private FlushDelegate _flushAction;
        #endregion

        #region properties
        /// <summary>
        /// 获取是否可以读取流。
        /// </summary>
        public override bool CanRead {
            get { return false; }
        }
        /// <summary>
        /// 获取是否可以Seek。
        /// </summary>
        public override bool CanSeek {
            get { return false; }
        }
        /// <summary>
        /// 获取是否可以写入流。
        /// </summary>
        public override bool CanWrite {
            get { return true; }
        }
        /// <summary>
        /// 不支持
        /// </summary>
        public override long Length {
            get {
                throw new NotSupportedException();
            }
        }
        /// <summary>
        /// 不支持
        /// </summary>
        public override long Position {
            get {
                throw new NotSupportedException();
            }

            set {
                throw new NotSupportedException();
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建HttpOutputStream实例。
        /// </summary>
        /// <param name="writeAction">写入流委托。</param>
        /// <param name="flushAction">Flush流委托。</param>
        public HttpOutputStream(WriteDelegate writeAction, FlushDelegate flushAction) {
            _writeAction = writeAction;
            _flushAction = flushAction;
        }
        #endregion

        #region methods
        /// <summary>
        /// 释放由 System.IO.Stream 占用的非托管资源，还可以另外再释放托管资源。
        /// </summary>
        /// <param name="disposing">为 true，则释放托管资源和非托管资源；为 false，则仅释放非托管资源。</param>
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                _writeAction = null;
                _flushAction = null;
            }
        }

        #region Flush
        /// <summary>
        /// 清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// </summary>
        public override void Flush() {
            _flushAction?.Invoke();
        }
        #endregion
        /// <summary>
        /// 写入流。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count) {
            if (_writeAction != null)
                _writeAction(buffer, offset, count);
        }
        /// <summary>
        /// 不支持
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 不支持
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 不支持
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value) {
            throw new NotSupportedException();
        }
        #endregion

        #region types
        /// <summary>
        /// Flush流委托。
        /// </summary>
        public delegate void FlushDelegate();
        /// <summary>
        /// 写入流委托。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public delegate void WriteDelegate(byte[] buffer, int offset, int count);
        #endregion

    }
}