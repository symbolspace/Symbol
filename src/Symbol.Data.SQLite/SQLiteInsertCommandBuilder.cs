/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// SQLite 插入命令构造器
    /// </summary>
    public class SQLiteInsertCommandBuilder : Symbol.Data.InsertCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建SQLiteInsertCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public SQLiteInsertCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

    }
}

