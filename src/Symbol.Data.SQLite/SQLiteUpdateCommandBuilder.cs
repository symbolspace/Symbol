/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
 
namespace Symbol.Data {

    /// <summary>
    /// SQLite 更新命令构造器
    /// </summary>
    public class SQLiteUpdateCommandBuilder : Symbol.Data.UpdateCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建SQLiteUpdateCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public SQLiteUpdateCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

      
    }
}

