/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.Data {

    /// <summary>
    /// 事务接口。
    /// </summary>
    public interface ITransaction : System.IDisposable {

        /// <summary>
        /// 获取提供者。
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// 获取连接对象。
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// 获取是否在事务中。
        /// </summary>
        bool Working { get; }

        /// <summary>
        /// 开始事务。
        /// </summary>
        void Begin();
        /// <summary>
        /// 提交事务。
        /// </summary>
        void Commit();
        /// <summary>
        /// 回滚事务。
        /// </summary>
        void Rollback();

    }
}