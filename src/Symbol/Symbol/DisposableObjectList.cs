/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol {
    /// <summary>
    /// 可释放对象列表（线程安全，顺序释放）。
    /// </summary>
    /// <remarks>列表自身已释放时，追加的对象会被立即释放。</remarks>
    public class DisposableObjectList : System.IDisposable {

        #region fields
        private int _disposed = 0;
        private System.Collections.Concurrent.ConcurrentQueue<System.IDisposable> _list;
        private int _count = 0;
        #endregion

        #region properties

        /// <summary>
        /// 获取未释放数量。
        /// </summary>
        public int Count {
            get {
                var value = System.Threading.Interlocked.CompareExchange(ref _count, -1, -1);
                return value;
            }
        }
        /// <summary>
        /// 获取列表自身是否释放。
        /// </summary>
        public bool IsDisposed {
            get {
                var value = System.Threading.Interlocked.CompareExchange(ref _disposed, -1, -1);
                return value == 1;
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建 DisposableObjectList 实例。
        /// </summary>
        public DisposableObjectList() {
            _list = new System.Collections.Concurrent.ConcurrentQueue<System.IDisposable>();
        }
        #endregion

        #region methods

        #region Add
        /// <summary>
        /// 添加一个可释放对象。
        /// </summary>
        /// <param name="item">为null自动忽略，如果自身已经释放，将立即释放对象。</param>
        /// <returns></returns>
        public DisposableObjectList Add(System.IDisposable item) {
            if (item != null) {
                var list = System.Threading.Interlocked.CompareExchange(ref _list, null, null);
                if (IsDisposed || list == null) {
                    item.Dispose();
                } else {
                    list.Enqueue(item);
                    System.Threading.Interlocked.Increment(ref _count);
                }
            }
            return this;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 释放对象占用的资源（包括子对象）。
        /// </summary>
        public void Dispose() {
            var state = System.Threading.Interlocked.CompareExchange(ref _disposed, 1, 0);
            if (state == 1)
                return;
            var list = System.Threading.Interlocked.CompareExchange(ref _list, null, null);
            if (list != null) {
                Dispose_Body(list);
            }
            System.Threading.Interlocked.Exchange(ref _count, 0);
        }
        void Dispose_Body(System.Collections.Concurrent.ConcurrentQueue<System.IDisposable> list) {
            while (list.TryDequeue(out System.IDisposable item)) {
                item.Dispose();
            }
        }
        #endregion

        #endregion

    }


}
