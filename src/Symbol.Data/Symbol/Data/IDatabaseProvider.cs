/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据库提供者接口
    /// </summary>
    public interface IDatabaseProvider {

        /// <summary>
        /// 获取是否支持单个连接中多个查询。
        /// </summary>
        bool MultipleActiveResultSets { get; }

        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        IDbConnection CreateConnection(string connectionString);
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        IDbConnection CreateConnection(object connectionOptions);
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        IDataContext CreateDataContext(IDbConnection connection);
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据上下文。</returns>
        IDataContext CreateDataContext(string connectionString);
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据上下文。</returns>
        IDataContext CreateDataContext(object connectionOptions);


        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string name);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">包含多级名称，如db.test.abc</param>
        /// <param name="spliter">多级分割符，如“.”</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string pairs, string spliter);
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="pairs">多级名称，如[ "db", "test", "abc" ]</param>
        /// <returns>返回处理后的名称。</returns>
        string PreName(string[] pairs);
        #endregion

    }

}
