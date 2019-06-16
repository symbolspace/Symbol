/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// 连接池。
    /// </summary>
    public class ConnectionPool : IConnectionPool {

        #region fields
        private IConnection _master;
        private System.Collections.Concurrent.ConcurrentStack<IConnection> _list;

        #endregion

        #region properties

        /// <summary>
        /// 获取提供者。
        /// </summary>
        public IProvider Provider { get { return Master?.Provider; } }

        /// <summary>
        /// 获取主连接。
        /// </summary>
        public IConnection Master { get { return ThreadHelper.InterlockedGet(ref _master); } }
        /// <summary>
        /// 获取数量。
        /// </summary>
        public int Count {
            get {
                var list = ThreadHelper.InterlockedGet(ref _list);
                return list == null ? 0 : list.Count;
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建ConnectionPool实例
        /// </summary>
        /// <param name="master">主连接。</param>
        public ConnectionPool(IConnection master) {
            _master = master;

            _list = new System.Collections.Concurrent.ConcurrentStack<IConnection>();
            if (!_master.MultipleActiveResultSets)
                Push(_master);
        }
        #endregion

        #region methods

        /// <summary>
        /// 从池中拿出一个连接对象。
        /// </summary>
        /// <returns>返回一个连接对象。</returns>
        public virtual IConnection Take() {
            var master = Master;
            if (master == null)
                return null;
            if (master.MultipleActiveResultSets)
                return master;
            var list = ThreadHelper.InterlockedGet(ref _list);
            IConnection connection;
            if (!list.TryPop(out connection)) {
                connection = master.Clone();
            }
            return connection;
        }
        /// <summary>
        /// 将连接对象放入池中。
        /// </summary>
        /// <param name="connection">连接对象，null直接忽略。</param>
        public virtual void Push(IConnection connection) {
            if (connection == null)
                return;
            connection?.Transaction?.Rollback();

            var master = Master;
            if (connection.MultipleActiveResultSets) {
                if( connection!= master)
                    connection.Dispose();
                return;
            }
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null) {
                connection.Dispose();
                return;
            }
            list.Push(connection);
        }

        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public virtual void Dispose() {
            var master = ThreadHelper.InterlockedSet(ref _master,null);
            master?.Dispose();

            var list = ThreadHelper.InterlockedSet(ref _list, null);
            if (list != null) {
                while (true) {
                    if (!list.TryPop(out IConnection connection)) {
                        break;
                    }
                    connection.Dispose();
                }
            }
        }

        #endregion

    }

}