/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// SqlServer 命令
    /// </summary>
    public class SqlServerCommand : AdoCommand {


        #region ctor
        /// <summary>
        /// 创建SqlServerCommand实例。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        public SqlServerCommand(IDataContext dataContext)
            : base(dataContext) {
        }
        #endregion

        #region methods


        /// <summary>
        /// 创建参数列表。
        /// </summary>
        /// <returns></returns>
        protected override ICommandParameterList CreateCommandParameterList() {
            return new SqlServerCommandParameterList(DataContext?.Provider);
        }

        #region ExecuteScalar

        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="dbCommandCache">DbCommandCache对象。</param>
        /// <returns>返回查询结果。</returns>
        protected override object ExecuteScalar(AdoCommandCache dbCommandCache) {
            var dbCommand = dbCommandCache.DbCommand;
            bool insert = dbCommand.CommandText.IndexOf("insert ", System.StringComparison.OrdinalIgnoreCase) > -1;
            if (insert) {
                dbCommand.CommandText += "\r\nselect SCOPE_IDENTITY() as [newid]";
            }
            var result = base.ExecuteScalar(dbCommandCache);
            if (insert) {
                if (result == null || result is System.DBNull) {
                    dbCommand.CommandText = "select SCOPE_IDENTITY() as [newid]";
                    result = base.ExecuteScalar(dbCommandCache);
                }
            }
            return result;
        }
        #endregion



        #endregion

    }

}

