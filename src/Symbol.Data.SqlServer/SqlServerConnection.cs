/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// ADO.NET 连接
    /// </summary>
    public class SqlServerConnection : AdoConnection {


        #region properties
        /// <summary>
        /// 获取是否支持多个活动结果集。
        /// </summary>
        public override bool MultipleActiveResultSets { get { return true; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建AdoConnection实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        /// <param name="connection">连接对象。</param>
        /// <param name="connectionString">连接字符串</param>
        public SqlServerConnection(IProvider provider, IDbConnection connection, string connectionString) 
            :base(provider , connection , connectionString) {
        }
        #endregion

    }

}