/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 命令
    /// </summary>
    public class PostgreSQLCommand : AdoCommand {


        #region ctor
        /// <summary>
        /// 创建PostgreSQLCommand实例。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        public PostgreSQLCommand(IDataContext dataContext)
            : base(dataContext) {
        }
        #endregion

        #region methods


        /// <summary>
        /// 创建参数列表。
        /// </summary>
        /// <returns></returns>
        protected override ICommandParameterList CreateCommandParameterList() {
            return new PostgreSQLCommandParameterList(DataContext?.Provider);
        }

        #region ExecuteScalar

        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="dbCommandCache">DbCommandCache对象。</param>
        /// <returns>返回查询结果。</returns>
        protected override object ExecuteScalar(AdoCommandCache dbCommandCache) {
            var dbCommand = dbCommandCache.DbCommand;
            if(    dbCommand.CommandText.IndexOf("insert ", System.StringComparison.OrdinalIgnoreCase) > -1
                && dbCommand.CommandText.IndexOf("returning", System.StringComparison.OrdinalIgnoreCase) == -1) {
                dbCommand.CommandText += "returning *";
            }
            return base.ExecuteScalar(dbCommandCache);
        }
        #endregion



        #endregion

    }

}

