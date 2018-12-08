/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace System.IO {
    /// <summary>
    /// 一个临时可写的流
    /// </summary>
    public class ActionStreamWriter : System.IO.Stream {


        #region fields
        private WriteAction _writeAction = null;
        private FlushAction _flushAction = null;
        #endregion

        #region properties
        /// <summary>
        /// 是否可读，永远返回false。
        /// </summary>
        public override bool CanRead { get { return false; } }
        /// <summary>
        /// 是否可定位，永远返回false。
        /// </summary>
        public override bool CanSeek { get { return false; } }
        /// <summary>
        /// 是否可写，永远返回true。
        /// </summary>
        public override bool CanWrite { get { return true; } }
        /// <summary>
        /// 未实现。
        /// </summary>
        public override long Length {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// 未实现。
        /// </summary>
        public override long Position {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建ActionStreamWriter的实例。
        /// </summary>
        /// <param name="writeAction">写入时调用的委托。</param>
        /// <param name="flushAction">flush调用的委托。</param>
        public ActionStreamWriter(WriteAction writeAction,FlushAction flushAction=null) {
            _writeAction = writeAction;
            _flushAction = flushAction;
        }
        #endregion

        #region methods

        /// <summary>
        /// 空白代码。
        /// </summary>
        public override void Flush() {
            _flushAction?.Invoke();
        }
        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count) {
            Symbol.CommonException.ThrowNotImplemented();
            return -1;
        }
        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            Symbol.CommonException.ThrowNotImplemented();
            return -1L;
        }
        /// <summary>
        /// 未实现。
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value) {
            Symbol.CommonException.ThrowNotImplemented();
        }
        #region Write
        /// <summary>
        /// 将缓存数据写入到流中，实际调用的是构造函数中的action委托。
        /// </summary>
        /// <param name="buffer">缓冲数据</param>
        /// <param name="offset">偏移位置</param>
        /// <param name="count">数据长度</param>
        public override void Write(byte[] buffer, int offset, int count) {
            _writeAction(buffer, offset, count);
        }
        #endregion

        #endregion

        #region types
        /// <summary>
        /// 写入时调用的委托
        /// </summary>
        /// <param name="buffer">缓冲数据</param>
        /// <param name="offset">偏移位置</param>
        /// <param name="count">数据长度</param>
        public delegate void WriteAction(byte[] buffer, int offset, int count);
        /// <summary>
        /// Flush调用的委托。
        /// </summary>
        public delegate void FlushAction();
        #endregion

    }
}