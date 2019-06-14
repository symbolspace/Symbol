/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {


    /// <summary>
    /// SqlServer 2005 数据上下文。
    /// </summary>
    public class SqlServer2005DataContext : SqlServerDataContext {

        #region ctor
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public SqlServer2005DataContext(IConnection connection) 
            : base(connection) {
        }
        #endregion


        #region methods

        #region CreateSelect
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override ISelectCommandBuilder CreateSelect(string tableName) {
            return new SqlServer2005SelectCommandBuilder(this, tableName, null);
        }
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="commandText">查询命令。</param>
        /// <returns>返回构造器对象。</returns>
        public override ISelectCommandBuilder CreateSelect(string tableName, string commandText) {
            return new SqlServer2005SelectCommandBuilder(this, tableName, commandText);
        }
        #endregion

        #endregion
    }

}

