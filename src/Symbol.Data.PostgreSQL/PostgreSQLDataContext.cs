/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {


    /// <summary>
    /// PostgreSQL 数据上下文。
    /// </summary>
    public class PostgreSQLDataContext : AdoDataContext {

        #region ctor
        /// <summary>
        /// 创建 MysqlDataContext 的实例
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public PostgreSQLDataContext(IConnection connection) 
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
            return new PostgreSQLCommand(this);
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

        #region TableSpaceExists
        /// <summary>
        /// 判断表空间是否存在。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <returns>返回判断结果。</returns>
        public override bool TableSpaceExists(string name) {
            if (string.IsNullOrEmpty(name))
                return false;
            name = Connection?.DatabaseName + "_" + name;
            return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from pg_tablespace where spcname=@p1;", name), false);
        }
        #endregion
        #region TableSpaceCreate
        /// <summary>
        /// 创建表空间。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <param name="path">路径。</param>
        public override bool TableSpaceCreate(string name, string path = null) {
            if (string.IsNullOrEmpty(name))
                return false;
            if (TableSpaceExists(name))
                return true;
            if (string.IsNullOrEmpty(path)) {
                //string dir = GetTableSpacePath(null);
                //path = System.IO.Path.GetDirectoryName(dir);
                //path = System.IO.Path.Combine(path, _connection.Database + "_" + name + ".ndf");
            } else {
                string dir = System.IO.Path.GetDirectoryName(path);
                AppHelper.CreateDirectory(dir, false);
            }
            ExecuteBlockQuery(string.Format("CREATE TABLESPACE \"{0}_{1}\" LOCATION '{2}';", Connection?.DatabaseName, name, path));
            return TableSpaceExists(name);
        }
        #endregion
        #region TableSpaceDelete
        /// <summary>
        /// 删除表空间。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        public override void TableSpaceDelete(string name) {
            if (string.IsNullOrEmpty(name))
                return;
            if (!TableSpaceExists(name))
                return;
            ExecuteBlockQuery(string.Format("drop TABLESPACE \"{0}_{1}\";", Connection?.DatabaseName, name));
        }
        #endregion

        #region TableCreate
        /// <summary>
        /// 创建表（仅用于简单的逻辑，复杂的创建语句请直接调用ExecuteNonQuery）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columns">列，每一个列请自行拼接好属性。</param>
        public override void TableCreate(string tableName, params string[] columns) {
            string columnText = string.Empty;
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendFormat("create table \"{0}\"(", tableName);
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

        /// <summary>
        /// 判断表是否存在（指定架构）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，默认public</param>
        /// <returns>返回判断结果。</returns>
        public override bool TableExists(string tableName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName))
                return false;
            if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                schemaName = "public";
            return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from information_schema.tables where table_schema=@p1 and  table_name=@p2;", schemaName, tableName), false);
        }
        #region ColumnExists
        /// <summary>
        /// 判断列（字段）是否存在（指定架构）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，默认public</param>
        /// <returns></returns>
        public override bool ColumnExists(string tableName, string columnName, string schemaName = "default") {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
                return false;
            if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                schemaName = "public";
            return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from information_schema.columns where table_schema=@p1 and table_name=@p2 and column_name=@p3;", schemaName, tableName, columnName), false);
        }
        #endregion
        #region GetColumnInfo
        /// <summary>
        /// 获取数据库中列（字段）的信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，默认public</param>
        /// <returns>不存在将new一个，并且Exists为false。</returns>
        public override DatabaseTableField GetColumnInfo(string tableName, string columnName, string schemaName = "@default") {
            DatabaseTableField item = null;
            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnName)) {
                if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                    schemaName = "public";
                item = CreateQuery<DatabaseTableField>(@"
SELECT
    1::boolean as ""Exists"",
    col.table_name as ""TableName"",
    col.column_name as ""Name"",
    col.data_type as ""Type"",
    col.ordinal_position as ""Position"",
    col.is_nullable as ""Nullable"",
    exists(select 1 from pg_constraint as cst where cst.conname ~*('^pk_' || col.table_name || '_' || col.column_name || '$')) as ""IsPrimary"",
    (not col.column_default is null and col.column_default ~*'^nextval') as ""IsIdentity"",
    (case when attr.attlen = -1 then coalesce(coalesce(col.character_maximum_length, col.numeric_precision), -1) else attr.attlen end) as ""Length"",
    col.numeric_scale as ""Scale"",
    col.column_default as ""DefaultValue"",
    des.description as ""Description""
FROM information_schema.columns col
left join pg_class as cls on cls.relname = col.table_name
LEFT JOIN pg_description des ON des.objoid = cls.oid AND col.ordinal_position = des.objsubid
left join pg_attribute as attr on attr.attrelid = cls.oid and attr.attname = col.column_name
WHERE
    table_schema = @p1
    AND table_name = @p2 and  lower(col.column_name)=lower(@p3)
ORDER BY
    col.table_name,col.ordinal_position;
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
        /// <param name="schemaName">架构名称，默认public</param>
        /// <returns></returns>
        public override System.Collections.Generic.List<DatabaseTableField> GetColumns(string tableName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName))
                return new System.Collections.Generic.List<DatabaseTableField>();
            if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                schemaName = "public";
            return CreateQuery<DatabaseTableField>(@"
SELECT
    1::boolean as ""Exists"",
    col.table_name as ""TableName"",
    col.column_name as ""Name"",
    col.data_type as ""Type"",
    col.ordinal_position as ""Position"",
    col.is_nullable as ""Nullable"",
    exists(select 1 from pg_constraint as cst where cst.conname ~*('^pk_' || col.table_name || '_' || col.column_name || '$')) as ""IsPrimary"",
    (not col.column_default is null and col.column_default ~*'^nextval') as ""IsIdentity"",
    (case when attr.attlen = -1 then coalesce(coalesce(col.character_maximum_length, col.numeric_precision), -1) else attr.attlen end) as ""Length"",
    col.numeric_scale as ""Scale"",
    col.column_default as ""DefaultValue"",
    des.description as ""Description""
FROM information_schema.columns col
left join pg_class as cls on cls.relname = col.table_name
LEFT JOIN pg_description des ON des.objoid = cls.oid AND col.ordinal_position = des.objsubid
left join pg_attribute as attr on attr.attrelid = cls.oid and attr.attname = col.column_name
WHERE
    table_schema = @p1
    AND table_name = @p2
ORDER BY
    col.table_name,col.ordinal_position;
                ", schemaName, tableName).ToList();
        }
        #endregion
        #region FunctionExists
        /// <summary>
        /// 判断函数是否存在。
        /// </summary>
        /// <param name="functionName">函数名称，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，默认public</param>
        /// <returns>返回判断结果。</returns>
        public override bool FunctionExists(string functionName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(functionName))
                return false;
            if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                schemaName = "public";
            return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from pg_proc where proname=@p1", functionName), false);
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
        static string ForeignKey(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey) {
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
            return new PostgreSQLSelectCommandBuilder(this, tableName, null);
        }
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="commandText">查询命令。</param>
        /// <returns>返回构造器对象。</returns>
        public override ISelectCommandBuilder CreateSelect(string tableName, string commandText) {
            return new PostgreSQLSelectCommandBuilder(this, tableName, commandText);
        }
        #endregion
        #region CreateInsert
        /// <summary>
        /// 创建插入命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IInsertCommandBuilder CreateInsert(string tableName) {
            return new PostgreSQLInsertCommandBuilder(this, tableName);
        }
        #endregion
        #region CreateUpdate
        /// <summary>
        /// 创建更新命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IUpdateCommandBuilder CreateUpdate(string tableName) {
            return new PostgreSQLUpdateCommandBuilder(this, tableName);
        }
        #endregion

        #endregion

        #endregion
    }

}

