/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
 
namespace Symbol.Data {

    /// <summary>
    /// SqlServer 更新命令构造器
    /// </summary>
    public class SqlServerUpdateCommandBuilder : Symbol.Data.UpdateCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建SqlServerUpdateCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public SqlServerUpdateCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

      
    }
}

