/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
 
namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 更新命令构造器
    /// </summary>
    public class PostgreSQLUpdateCommandBuilder : Symbol.Data.UpdateCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建PostgreSQLUpdateCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public PostgreSQLUpdateCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

      
    }
}

