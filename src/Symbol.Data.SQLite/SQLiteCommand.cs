/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// SQLite 命令
    /// </summary>
    public class SQLiteCommand : AdoCommand {


        #region ctor
        /// <summary>
        /// 创建SQLiteCommand实例。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        public SQLiteCommand(IDataContext dataContext)
            : base(dataContext) {
        }
        #endregion

        #region methods


        /// <summary>
        /// 创建参数列表。
        /// </summary>
        /// <returns></returns>
        protected override ICommandParameterList CreateCommandParameterList() {
            return new SQLiteCommandParameterList(DataContext?.Provider);
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
                string tableName = Symbol.Text.StringExtractHelper.StringsStartEnd(dbCommand.CommandText, "insert into ", new string[] { "(", " values" }, 0, false, false, false);
                if (!string.IsNullOrEmpty(tableName))
                    dbCommand.CommandText += $";\r\nselect last_insert_rowid() as [newId] from {tableName} limit 0,1";
            }
            var result = base.ExecuteScalar(dbCommandCache);
            if (insert) {
                if (result == null || result is System.DBNull) {
                    string tableName = Symbol.Text.StringExtractHelper.StringsStartEnd(dbCommand.CommandText, "insert into ", new string[] { "(", " values" }, 0, false, false, false);
                    if (!string.IsNullOrEmpty(tableName)) {
                        dbCommand.CommandText = $"select last_insert_rowid() as [newId] from {tableName} limit 0,1";
                        result = base.ExecuteScalar(dbCommandCache);
                    }
                }
            }
            return result;
        }
        #endregion



        #endregion

    }

}

