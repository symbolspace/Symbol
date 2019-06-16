/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 插入命令构造器
    /// </summary>
    public class PostgreSQLInsertCommandBuilder : Symbol.Data.InsertCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建PostgreSQLInsertCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public PostgreSQLInsertCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

    }
}

