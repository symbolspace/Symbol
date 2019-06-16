/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 连接
    /// </summary>
    public class AdoConnection : Connection, IAdoConnection {

        #region fields
        private IDbConnection _connection;
        private string _connectionString;
        private string _databaseName;
        private IAdoTransaction _transaction;
        #endregion

        #region properties
        /// <summary>
        /// 获取Ado连接对象。
        /// </summary>
        public IDbConnection DbConnection { get { return ThreadHelper.InterlockedGet(ref _connection);  } }
        /// <summary>
        /// 获取Ado事务对象。
        /// </summary>
        public IDbTransaction DbTransaction { get { return ThreadHelper.InterlockedGet(ref _transaction)?.DbTransaction; } }

        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        public override bool Connected {
            get {
                var value = DbConnection;
                if (value == null)
                    return false;
                switch (value.State) {
                    case ConnectionState.Open:
                    case ConnectionState.Executing:
                    case ConnectionState.Fetching:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 获取连接字符串。
        /// </summary>
        public override string ConnectionString { get { return _connectionString; } }
        /// <summary>
        /// 获取超时时间（秒）。
        /// </summary>
        public override int Timeout {
            get {
                var connection = DbConnection;
                return connection == null ? 0 : connection.ConnectionTimeout;
            }
        }
        /// <summary>
        /// 获取数据库名称。
        /// </summary>
        public override string DatabaseName {
            get { return DbConnection?.Database; }
        }
        /// <summary>
        /// 获取原数据库名称。
        /// </summary>
        public override string OriginalDatabaseName { get { return _databaseName; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建AdoConnection实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        /// <param name="connection">连接对象。</param>
        /// <param name="connectionString">连接字符串</param>
        public AdoConnection(IProvider provider, IDbConnection connection, string connectionString) 
            :base(provider) {
            _connection = connection;
            _connectionString = string.IsNullOrEmpty(connectionString) ? connection.ConnectionString : connectionString;
            _databaseName = connection.Database;
        }
        #endregion

        #region methods

        /// <summary>
        /// 打开连接。
        /// </summary>
        public override void Open() {
            if (!Connected)
                DbConnection?.Open();
        }
        /// <summary>
        /// 关闭连接。
        /// </summary>
        public override void Close() {
            var connection = DbConnection;
            if (connection == null)
                return;
            if (Connected || connection.State == ConnectionState.Connecting)
                connection?.Close();
        }

        #region ChangeDatabase
        /// <summary>
        /// 变更当前数据库（默认）。
        /// </summary>
        public override void ChangeDatabase() {
            ChangeDatabase(_databaseName);
        }
        /// <summary>
        /// 变更当前数据库（指定）。
        /// </summary>
        /// <param name="database">数据库名称。</param>
        public override void ChangeDatabase(string database) {
            if (string.IsNullOrEmpty(database))
                return;
            Open();
            DbConnection?.ChangeDatabase(database);
        }
        #endregion


        /// <summary>
        /// 创建事务对象。
        /// </summary>
        /// <returns>返回事务对象。</returns>
        protected override ITransaction CreateTranscation() {
            var _transaction= new AdoTransaction(this);
            return _transaction;
        }

        /// <summary>
        /// 克隆一个新连接。
        /// </summary>
        public override IConnection Clone() {
            return Provider?.CreateConnection(_connectionString);
        }

        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public override void Dispose() {
            Close();
            var connection = ThreadHelper.InterlockedSet(ref _connection, null);
            connection?.Dispose();
            ThreadHelper.InterlockedSet(ref _transaction, null);
            _databaseName = null;
            _connectionString = null;
            base.Dispose();
        }

        #endregion
    }

}