/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {


    /// <summary>
    /// MySql 数据上下文。
    /// </summary>
    public class MySqlDataContext : AdoDataContext {

        #region ctor
        /// <summary>
        /// 创建 MysqlDataContext 的实例
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public MySqlDataContext(IConnection connection) 
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
            return new MySqlCommand(this);
        }


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
            builder.AppendFormat("create table `{0}`(", tableName);
            if (columns != null) {
                for (int i = 0; i < columns.Length; i++) {
                    if (i > 0)
                        builder.Append(',');
                    if (string.IsNullOrEmpty(columns[i]))
                        continue;
                    builder.Append(columns[i]);
                }
            }
            builder.Append(") DEFAULT CHARSET=utf8;");
            ExecuteNonQuery(builder.ToString());
        }
        #endregion

        /// <summary>
        /// 判断表是否存在（指定架构）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        public override bool TableExists(string tableName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName))
                return false;
            if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                schemaName = Connection?.DatabaseName;
            return TypeExtensions.Convert<bool>(ExecuteScalar("SELECT 1 FROM information_schema.TABLES WHERE table_schema=@p1 and table_name =@p2;", schemaName, tableName), false);
        }

        #region ColumnExists
        /// <summary>
        /// 判断列（字段）是否存在（指定架构）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns></returns>
        public override bool ColumnExists(string tableName, string columnName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
                return false;
            if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                schemaName = Connection?.DatabaseName;
            return TypeExtensions.Convert<bool>(ExecuteScalar("SELECT 1 FROM  information_schema.COLUMNS WHERE TABLE_SCHEMA=@p1 AND table_name=@p2 AND COLUMN_NAME=@p3;", schemaName, tableName, columnName), false);
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
            DatabaseTableField item = null;
            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnName)) {
                if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                    schemaName = Connection?.DatabaseName;
                item = CreateQuery<DatabaseTableField>(@"
SELECT 1 as `Exists`,`TABLE_NAME` as `TableName`,`COLUMN_NAME` as `Name`,
`DATA_TYPE` as `Type`,
`ORDINAL_POSITION` as `Position`,`IS_NULLABLE` as `Nullable`,
(`COLUMN_KEY` like 'PRI') as `IsPrimary`,
(`EXTRA` like 'auto_increment') as `IsIdentity`,
(case when `CHARACTER_MAXIMUM_LENGTH` is null or `CHARACTER_MAXIMUM_LENGTH`=0 then `NUMERIC_PRECISION` else `CHARACTER_MAXIMUM_LENGTH` end) as `Length`,
`NUMERIC_SCALE` as `Scale`,
`COLUMN_COMMENT` as 'Description',
`COLUMN_DEFAULT` as `DefaultValue`
FROM  information_schema.COLUMNS
where TABLE_SCHEMA=@p1 AND table_name=@p2 AND COLUMN_NAME=@p3
order by `Position`,`Name`;
                    ", schemaName, tableName, columnName).FirstOrDefault();
            }
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
            if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                schemaName = Connection?.DatabaseName;
            return CreateQuery<DatabaseTableField>(@"
SELECT 1 as `Exists`,`TABLE_NAME` as `TableName`,`COLUMN_NAME` as `Name`,
`DATA_TYPE` as `Type`,
`ORDINAL_POSITION` as `Position`,`IS_NULLABLE` as `Nullable`,
(`COLUMN_KEY` like 'PRI') as `IsPrimary`,
(`EXTRA` like 'auto_increment') as `IsIdentity`,
(case when `CHARACTER_MAXIMUM_LENGTH` is null or `CHARACTER_MAXIMUM_LENGTH`=0 then `NUMERIC_PRECISION` else `CHARACTER_MAXIMUM_LENGTH` end) as `Length`,
`NUMERIC_SCALE` as `Scale`,
`COLUMN_COMMENT` as 'Description',
`COLUMN_DEFAULT` as `DefaultValue`
FROM  information_schema.COLUMNS
where TABLE_SCHEMA=@p1 AND table_name=@p2
order by `Position`,`Name`;
                ", schemaName, tableName).ToList();
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
            return !string.IsNullOrEmpty(TypeExtensions.Convert<string>(ExecuteScalar("show function status like @p1;", functionName)));
        }
        //show procedure status like 'myproc';
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
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            string fkKey = ForeignKey(primaryKeyTableName, primaryKey, foreignKeyTableName, foreignKey);
            builder.AppendFormat(
                "alter table \"{0}\" add CONSTRAINT \"{1}\" FOREIGN KEY (\"{2}\") REFERENCES \"{3}\" (\"{4}\")",
                foreignKeyTableName,
                fkKey,
                foreignKey,
                primaryKeyTableName,
                primaryKey);
            if (cascadeUpdate) {
                builder.Append(" ON UPDATE CASCADE");
            } else {
                builder.Append(" ON UPDATE NO ACTION");
            }
            if (cascadeDelete) {
                builder.Append(" ON DELETE CASCADE");
            } else {
                builder.Append(" ON DELETE NO ACTION");
            }
            builder.AppendLine(";");
            builder.AppendFormat("CREATE INDEX \"fki_{0}\" ON \"{1}\"(\"{2}\");", fkKey, primaryKeyTableName, primaryKey);
            ExecuteNonQuery(builder.ToString());
            /*
             * alter table "table_a" add CONSTRAINT "fkname" FOREIGN KEY ("table_bId") REFERENCES "table_b" ("id") on update no action on delete CASCADE
             */
        }
        #endregion
        string ForeignKey(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey) {
            string result = string.Format("FK_{0}_{1}__{2}_{3}", primaryKeyTableName, primaryKey, foreignKeyTableName, foreignKey);
            return Symbol.Encryption.MD5EncryptionHelper.Encrypt(result);
        }
        #region ForeignKeyDelete
        /// <summary>
        /// 删除外键关系。
        /// </summary>
        /// <param name="primaryKeyTableName">主键表名。</param>
        /// <param name="primaryKey">主键列名。</param>
        /// <param name="foreignKeyTableName">外键表名。</param>
        /// <param name="foreignKey">外键列名。</param>
        public override void ForeignKeyDelete(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey) {
            string key = ForeignKey(primaryKeyTableName, primaryKey, foreignKeyTableName, foreignKey);
            ExecuteNonQuery(string.Format("alter table \"{0}\" DROP CONSTRAINT \"{1}\"", foreignKeyTableName, key));
            ExecuteNonQuery(string.Format("drop index \"fki_{0}\"", key));
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
            return new MySqlSelectCommandBuilder(this, tableName, null);
        }
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="commandText">查询命令。</param>
        /// <returns>返回构造器对象。</returns>
        public override ISelectCommandBuilder CreateSelect(string tableName, string commandText) {
            return new MySqlSelectCommandBuilder(this, tableName, commandText);
        }
        #endregion
        #region CreateInsert
        /// <summary>
        /// 创建插入命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IInsertCommandBuilder CreateInsert(string tableName) {
            return new MySqlInsertCommandBuilder(this, tableName);
        }
        #endregion
        #region CreateUpdate
        /// <summary>
        /// 创建更新命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IUpdateCommandBuilder CreateUpdate(string tableName) {
            return new MySqlUpdateCommandBuilder(this, tableName);
        }
        #endregion

        #endregion

        #endregion
    }

}

