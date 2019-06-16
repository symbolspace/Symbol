/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// 连接基类
    /// </summary>
    public abstract class Connection : IConnection {

        #region fields
        private IProvider _provider;
        private ITransaction _transaction;
        #endregion

        #region properties
        /// <summary>
        /// 获取提供者。
        /// </summary>
        public IProvider Provider { get { return ThreadHelper.InterlockedGet(ref _provider); } }

        /// <summary>
        /// 获取是否支持多个活动结果集。
        /// </summary>
        public virtual bool MultipleActiveResultSets { get { return false; } }
        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        public abstract bool Connected { get; }
        /// <summary>
        /// 获取连接字符串。
        /// </summary>
        public abstract string ConnectionString { get; }
        /// <summary>
        /// 获取超时时间（秒）。
        /// </summary>
        public abstract int Timeout { get; }
        /// <summary>
        /// 获取数据库名称。
        /// </summary>
        public abstract string DatabaseName { get; }
        /// <summary>
        /// 获取原数据库名称。
        /// </summary>
        public abstract string OriginalDatabaseName { get; }

        /// <summary>
        /// 获取事务对象。
        /// </summary>
        public virtual ITransaction Transaction {
            get {
                var value = ThreadHelper.InterlockedGet(ref _transaction);
                if (value == null) {
                    value = _transaction = CreateTranscation();
                    ThreadHelper.InterlockedSet(ref _transaction, value);
                }
                return value;
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建Connection实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        public Connection(IProvider provider) {
            _provider = provider;
        }
        #endregion

        #region methods

        /// <summary>
        /// 打开连接。
        /// </summary>
        public abstract void Open();
        /// <summary>
        /// 关闭连接。
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 变更当前数据库（默认）。
        /// </summary>
        public abstract void ChangeDatabase();
        /// <summary>
        /// 变更当前数据库（指定）。
        /// </summary>
        /// <param name="database">数据库名称。</param>
        public abstract void ChangeDatabase(string database);

        /// <summary>
        /// 创建事务对象。
        /// </summary>
        /// <returns>返回事务对象。</returns>
        protected abstract ITransaction CreateTranscation();

        /// <summary>
        /// 克隆一个新连接。
        /// </summary>
        public abstract IConnection Clone();

        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public virtual void Dispose() {
            var transcaction = ThreadHelper.InterlockedSet(ref _transaction, null); 
            transcaction?.Dispose();

            ThreadHelper.InterlockedSet(ref _provider, null);
        }

        #endregion
    }

}