/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// MySql 命令
    /// </summary>
    public class MySqlCommand : AdoCommand {


        #region ctor
        /// <summary>
        /// 创建MySqlCommand实例。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        public MySqlCommand(IDataContext dataContext)
            : base(dataContext) {
        }
        #endregion

        #region methods


        /// <summary>
        /// 创建参数列表。
        /// </summary>
        /// <returns></returns>
        protected override ICommandParameterList CreateCommandParameterList() {
            return new MySqlCommandParameterList(DataContext?.Provider);
        }

        #region ExecuteScalar

        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="dbCommandCache">DbCommandCache对象。</param>
        /// <returns>返回查询结果。</returns>
        protected override object ExecuteScalar(AdoCommandCache dbCommandCache) {
            var result = base.ExecuteScalar(dbCommandCache);
            var dbCommand = dbCommandCache.DbCommand;
            bool insert = dbCommand.CommandText.IndexOf("insert ", System.StringComparison.OrdinalIgnoreCase) > -1;
            if (insert)
                result = FastWrapper.Get(dbCommand, "LastInsertedId");
            return result;
        }
        #endregion



        #endregion

    }

}

