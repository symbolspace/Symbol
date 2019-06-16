/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// 事务基类
    /// </summary>
    public abstract class Transaction : ITransaction {

        #region fields
        private IConnection _connection;
        #endregion

        #region properties
        /// <summary>
        /// 获取提供者。
        /// </summary>
        public IProvider Provider { get { return Connection?.Provider; } }

        /// <summary>
        /// 获取连接对象。
        /// </summary>
        public IConnection Connection { get { return ThreadHelper.InterlockedGet(ref _connection); } }

        /// <summary>
        /// 获取是否在事务中。
        /// </summary>
        public abstract bool Working { get; }


        #endregion

        #region ctor
        /// <summary>
        /// 创建Transaction实例。
        /// </summary>
        /// <param name="connection">连接对象。</param>
        public Transaction(IConnection connection) {
            _connection = connection;
        }
        #endregion

        #region methods

        /// <summary>
        /// 开始事务。
        /// </summary>
        public abstract void Begin();
        /// <summary>
        /// 提交事务。
        /// </summary>
        public abstract void Commit();
        /// <summary>
        /// 回滚事务。
        /// </summary>
        public abstract void Rollback();

        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public virtual void Dispose() {
            if (Working)
                Rollback();
            ThreadHelper.InterlockedSet(ref _connection, null);
        }

        #endregion
    }

}