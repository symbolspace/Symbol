/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Net {
    /// <summary>
    /// PeekSocket的Peeked事件信息类
    /// </summary>
    public class PeekSocketPeekedEventArgs : System.EventArgs, System.IDisposable {

        #region properties
        /// <summary>
        /// 缓冲数据
        /// </summary>
        public byte[] Buffer { get; private set; }
        /// <summary>
        /// 当前数据包是否为流模式。
        /// </summary>
        public bool Streaming { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// 创建PeekSocketPeekedEventArgs的实例。
        /// </summary>
        /// <param name="buffer">缓冲数据</param>
        /// <param name="streaming">是否为流模式。</param>
        public PeekSocketPeekedEventArgs(byte[] buffer, bool streaming = false) {
            Buffer = buffer;
            Streaming = streaming;
        }
        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose() {
            Buffer = null;
            System.GC.SuppressFinalize(this);
            //GC.Collect(0);
            //GC.Collect();
        }

        #endregion
    }

}
