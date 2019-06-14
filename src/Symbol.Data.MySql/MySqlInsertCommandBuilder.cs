/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// MySql 插入命令构造器
    /// </summary>
    public class MySqlInsertCommandBuilder : Symbol.Data.InsertCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建MySqlInsertCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public MySqlInsertCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

    }
}

