/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
 
namespace Symbol.Data {

    /// <summary>
    /// MySql 更新命令构造器
    /// </summary>
    public class MySqlUpdateCommandBuilder : Symbol.Data.UpdateCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建MySqlUpdateCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public MySqlUpdateCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

      
    }
}

