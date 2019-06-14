/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// SqlServer 插入命令构造器
    /// </summary>
    public class SqlServerInsertCommandBuilder : Symbol.Data.InsertCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建SqlServerInsertCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public SqlServerInsertCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion
    
    }
}

