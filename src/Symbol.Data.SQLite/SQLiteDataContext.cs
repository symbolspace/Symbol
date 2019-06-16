/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {


    /// <summary>
    /// SQLite 数据上下文。
    /// </summary>
    public class SQLiteDataContext : AdoDataContext {

        #region ctor
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public SQLiteDataContext(IConnection connection) 
            : base(connection) {
        }
        #endregion


        #region methods

        #region Command
        /// <summary>
        /// 创建命令对象。
        /// </summary>
        /// <returns>返回创建的命令对象。</returns>
        protected override ICommand CreateCommand() {
            return new SQLiteCommand(this);
        }

        #region Base
        /// <summary>
        /// 变更当前数据库（指定）。
        /// </summary>
        /// <param name="database">数据库名称。</param>
        public override void ChangeDatabase(string database) {

        }
        #endregion


        #region ExecuteBlockQuery
        /// <summary>
        /// 批量执行命令
        /// </summary>
        /// <param name="command">命令（SQL）。</param>
        /// <param name="mulitFlag">多段命令分隔符。</param>
        /// <param name="changeDatabase">切换数据库标志。</param>
        public override void ExecuteBlockQuery(string command, string mulitFlag = "GO", string changeDatabase = "use ") {
            if (string.IsNullOrEmpty(command))
                return;
            if (string.IsNullOrEmpty(mulitFlag))
                mulitFlag = "GO";
            if (command.IndexOf("{db.name}", System.StringComparison.OrdinalIgnoreCase) > -1)
                command = StringExtensions.Replace(command, "{db.name}", Connection?.OriginalDatabaseName, true);
            if (command.IndexOf("{db.path}", System.StringComparison.OrdinalIgnoreCase) > -1)
                command = StringExtensions.Replace(command, "{db.path}", GetTableSpaceDirectory(), true);

            string[] lines = command.Split(new string[] { mulitFlag }, System.StringSplitOptions.RemoveEmptyEntries);
            try {
                for (int i = 0; i < lines.Length; i++) {
                    if (string.IsNullOrEmpty(lines[i]))
                        continue;
                    if (lines[i].StartsWith(changeDatabase, System.StringComparison.OrdinalIgnoreCase)) {
                        OnExecuteBlockQuery_ChangeDatabase(lines[i], changeDatabase);
                        ExecuteNonQuery("--" + lines[i]);
                    } else {
                        ExecuteNonQuery(lines[i]);
                    }
                }
            } finally {
                ChangeDatabase();
            }
        }
        void OnExecuteBlockQuery_ChangeDatabase(string line, string changeDatabase) {
            int i = line.IndexOf(changeDatabase, System.StringComparison.OrdinalIgnoreCase);
            i += changeDatabase.Length;
            int j = line.IndexOf(';', i);
            if (j == -1) {
                j = line.IndexOf(']', i);
            }
            if (j == -1) {
                j = line.IndexOf('"', i + 1);
            }
            if (j == -1) {
                j = line.IndexOf('\r', i);
            }
            if (j == -1) {
                j = line.IndexOf('\n', i);
            }
            if (j == -1) {
                j = line.Length;
                //return;
            }
            string name = line.Substring(i, j - i);
            name = name.Trim(' ', ';', '[', ']', '"', '\r', '\n').Trim();
            if (string.Equals(name, "{db.name}", System.StringComparison.OrdinalIgnoreCase))
                name = Connection?.OriginalDatabaseName;
            ChangeDatabase(name);
        }
        #endregion

        #endregion

        #region Query

        #region CreateQuery
        /// <summary>
        /// 创建一个查询
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="type">成员类型</param>
        protected override IDataQuery<object> CreateQuery(ICommand command, System.Type type) {
            return new AdoDataQuery<object>(this, command, type);
        }
        /// <summary>
        /// 创建一个查询
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="command">命令对象</param>
        protected override IDataQuery<T> CreateQuery<T>(ICommand command) {
            return new AdoDataQuery<T>(this, command, typeof(T));
        }


        #endregion

        #endregion

        #region Schema

        #region TableCreate
        /// <summary>
        /// 创建表（仅用于简单的逻辑，复杂的创建语句请直接调用ExecuteNonQuery）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columns">列，每一个列请自行拼接好属性。</param>
        public override void TableCreate(string tableName, params string[] columns) {
            string columnText = string.Empty;
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendFormat("create table [{0}](", tableName);
            if (columns != null) {
                for (int i = 0; i < columns.Length; i++) {
                    if (i > 0)
                        builder.Append(',');
                    if (string.IsNullOrEmpty(columns[i]))
                        continue;
                    builder.Append(columns[i]);
                }
            }
            builder.Append(')');
            ExecuteNonQuery(builder.ToString());
        }
        #endregion
        #region TableExists
        /// <summary>
        /// 判断表是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        public override bool TableExists(string tableName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName))
                return false;
            return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from sqlite_master where [name]=@p1", tableName), false);
        }
        #endregion
        #region ColumnExists
        /// <summary>
        /// 判断列（字段）是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns></returns>
        public override bool ColumnExists(string tableName, string columnName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
                return false;
            return GetColumnInfo(tableName, columnName) != null;
        }
        #endregion
        #region GetColumnInfo
        /// <summary>
        /// 获取数据库中列（字段）的信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>永远不会返回null。</returns>
        public override DatabaseTableField GetColumnInfo(string tableName, string columnName, string schemaName = "@default") {
            System.Collections.Generic.List<DatabaseTableField> list = GetColumns(tableName);
            DatabaseTableField item = list.Find(p => string.Equals(p.Name, columnName, System.StringComparison.OrdinalIgnoreCase));
            if (item == null) {
                item = new DatabaseTableField() {
                    Exists = false,
                    TableName = tableName,
                    Name = columnName,
                    Nullable = true,
                };
            }
            return item;
        }
        #endregion
        #region GetColumns
        /// <summary>
        /// 获取数据库中表的所有列（字段）信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns></returns>
        public override System.Collections.Generic.List<DatabaseTableField> GetColumns(string tableName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName))
                return new System.Collections.Generic.List<DatabaseTableField>();
            string sql = TypeExtensions.Convert<string>(ExecuteScalar("select [sql] from [sqlite_master] where [name]=@p1", tableName));
            SQLite.TableSchemaHelper helper = new SQLite.TableSchemaHelper();
            return helper.ParseFieldsByCreate(sql);
        }
        #endregion
        #region FunctionExists
        /// <summary>
        /// 判断函数是否存在。
        /// </summary>
        /// <param name="functionName">函数名称，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        public override bool FunctionExists(string functionName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(functionName))
                return false;
            return Symbol.Data.SQLite.SQLiteHelper.ExistsFunction(functionName);
        }
        #endregion
        #region ForeignKeyCreate
        /// <summary>
        /// 创建外键关系。
        /// </summary>
        /// <param name="primaryKeyTableName">主键表名。</param>
        /// <param name="primaryKey">主键列名。</param>
        /// <param name="foreignKeyTableName">外键表名。</param>
        /// <param name="foreignKey">外键列名。</param>
        /// <param name="cascadeDelete">级联删除。</param>
        /// <param name="cascadeUpdate">级联更新。</param>
        public override void ForeignKeyCreate(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey, bool cascadeDelete = false, bool cascadeUpdate = false) {
            CommonException.ThrowNotSupported("sqlite3 外键需要写在表创建代码中");
        }
        #endregion
        #region ForeignKeyDelete
        /// <summary>
        /// 删除外键关系。
        /// </summary>
        /// <param name="primaryKeyTableName">主键表名。</param>
        /// <param name="primaryKey">主键列名。</param>
        /// <param name="foreignKeyTableName">外键表名。</param>
        /// <param name="foreignKey">外键列名。</param>
        public override void ForeignKeyDelete(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey) {
            CommonException.ThrowNotSupported("sqlite3 外键需要写在表创建代码中");
        }
        #endregion

        #endregion

        #region Builder

        #region CreateSelect
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override ISelectCommandBuilder CreateSelect(string tableName) {
            return new SQLiteSelectCommandBuilder(this, tableName, null);
        }
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="commandText">查询命令。</param>
        /// <returns>返回构造器对象。</returns>
        public override ISelectCommandBuilder CreateSelect(string tableName, string commandText) {
            return new SQLiteSelectCommandBuilder(this, tableName, commandText);
        }
        #endregion
        #region CreateInsert
        /// <summary>
        /// 创建插入命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IInsertCommandBuilder CreateInsert(string tableName) {
            return new SQLiteInsertCommandBuilder(this, tableName);
        }
        #endregion
        #region CreateUpdate
        /// <summary>
        /// 创建更新命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IUpdateCommandBuilder CreateUpdate(string tableName) {
            return new SQLiteUpdateCommandBuilder(this, tableName);
        }
        #endregion

        #endregion

        #endregion
    }

}

