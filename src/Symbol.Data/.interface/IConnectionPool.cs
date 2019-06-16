/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {


    /// <summary>
    /// 连接池接口。
    /// </summary>
    public interface IConnectionPool : System.IDisposable {


        /// <summary>
        /// 获取提供者。
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// 获取主连接。
        /// </summary>
        IConnection Master { get; }
        /// <summary>
        /// 获取数量。
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 从池中拿出一个连接对象。
        /// </summary>
        /// <returns>返回一个连接对象。</returns>
        IConnection Take();
        /// <summary>
        /// 将连接对象放入池中。
        /// </summary>
        /// <param name="connection">连接对象，null直接忽略。</param>
        void Push(IConnection connection);

    }

}