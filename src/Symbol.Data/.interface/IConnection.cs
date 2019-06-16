/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.Data {
    /// <summary>
    /// 连接接口
    /// </summary>
    public interface IConnection
        : System.IDisposable {
        /// <summary>
        /// 获取提供者。
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// 获取是否支持多个活动结果集。
        /// </summary>
        bool MultipleActiveResultSets { get; }
        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// 获取连接字符串。
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// 获取超时时间（秒）。
        /// </summary>
        int Timeout { get; }
        /// <summary>
        /// 获取数据库名称。
        /// </summary>
        string DatabaseName { get; }
        /// <summary>
        /// 获取原数据库名称。
        /// </summary>
        string OriginalDatabaseName { get; }
        /// <summary>
        /// 获取事务对象。
        /// </summary>
        ITransaction Transaction { get; }

        /// <summary>
        /// 打开连接。
        /// </summary>
        void Open();
        /// <summary>
        /// 关闭连接。
        /// </summary>
        void Close();

        /// <summary>
        /// 变更当前数据库（默认）。
        /// </summary>
        void ChangeDatabase();
        /// <summary>
        /// 变更当前数据库（指定）。
        /// </summary>
        /// <param name="database">数据库名称。</param>
        void ChangeDatabase(string database);


        /// <summary>
        /// 克隆一个新连接。
        /// </summary>
        IConnection Clone();
    }

}