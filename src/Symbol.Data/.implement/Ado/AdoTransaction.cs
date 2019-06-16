/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 事务。
    /// </summary>
    public class AdoTransaction : Transaction {

        #region fields
        private IDbTransaction _transaction;
        private AdoConnection _connection;
        #endregion

        #region properties
        /// <summary>
        /// 获取是否在事务中。
        /// </summary>
        public override bool Working { get { return ThreadHelper.InterlockedGet(ref _transaction) != null; } }
        /// <summary>
        /// 获取Ado事务对象。
        /// </summary>
        public IDbTransaction DbTransaction { get { return ThreadHelper.InterlockedGet(ref _transaction); } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建AdoTransaction实例。
        /// </summary>
        /// <param name="connection">连接对象。</param>
        public AdoTransaction(AdoConnection connection)
            : base(connection) {
            _connection = connection;
        }
        #endregion

        #region methods

        /// <summary>
        /// 开始事务。
        /// </summary>
        public override void Begin() {
            if (Working)
                return;
            var connection = ThreadHelper.InterlockedGet(ref _connection);
            var transaction = ThreadHelper.InterlockedGet(ref _transaction);
            if (connection == null)
                return;
            if (transaction == null) {
                transaction = connection.DbConnection.BeginTransaction();
                ThreadHelper.InterlockedSet(ref _transaction, transaction);
            }
        }
        /// <summary>
        /// 提交事务。
        /// </summary>
        public override void Commit() {
            if (!Working)
                return;
            var transaction = ThreadHelper.InterlockedSet(ref _transaction, null);
            try {
                transaction?.Commit();
                transaction?.Dispose();
            } catch {
                if (transaction != null)
                    ThreadHelper.InterlockedSet(ref _transaction, transaction);
                throw;
            }
        }
        /// <summary>
        /// 回滚事务。
        /// </summary>
        public override void Rollback() {
            if (!Working)
                return;
            if (!Working)
                return;
            var transaction = ThreadHelper.InterlockedSet(ref _transaction, null);
            try {
                transaction?.Rollback();
                transaction?.Dispose();
            } catch {
                if (transaction != null)
                    ThreadHelper.InterlockedSet(ref _transaction, transaction);
                throw;
            }
        }

        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public override void Dispose() {
            base.Dispose();
            ThreadHelper.InterlockedSet(ref _connection, null);
            ThreadHelper.InterlockedSet(ref _transaction, null)?.Dispose();
        }

        #endregion
    }

}