/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据库提供者接口
    /// </summary>
    public interface IProvider {

        /// <summary>
        /// 获取方言对象。
        /// </summary>
        IDialect Dialect { get; }

        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        IConnection CreateConnection(string connectionString);
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数，兼容string/object/ConnectionOptions。</param>
        /// <returns>返回数据库连接。</returns>
        IConnection CreateConnection(object connectionOptions);
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        IConnection CreateConnection(ConnectionOptions connectionOptions);
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        IDataContext CreateDataContext(IConnection connection);
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据上下文。</returns>
        IDataContext CreateDataContext(string connectionString);
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionOptions">连接参数，兼容string/object/ConnectionOptions。</param>
        /// <returns>返回数据上下文。</returns>
        IDataContext CreateDataContext(object connectionOptions);
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据上下文。</returns>
        IDataContext CreateDataContext(ConnectionOptions connectionOptions);

        /// <summary>
        /// 创建方言。
        /// </summary>
        /// <returns>返回方言对象。</returns>
        IDialect CreateDialect();

    }

}
