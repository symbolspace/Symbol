/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;
using System.Reflection;

namespace Symbol.Data {

    /// <summary>
    /// Mysql数据库提供者(5.x)
    /// </summary>
    public class MysqlDatabaseProvider : DatabaseProvider {

        #region fields
        private static readonly System.Collections.Generic.Dictionary<string, System.Type> _types;
        #endregion

        #region cctor
        static MysqlDatabaseProvider() {
            _types = new System.Collections.Generic.Dictionary<string, System.Type>();
            _types.Add("MySql.Data.MySqlClient.MySqlConnection", null);
            _types.Add("MySql.Data.MySqlClient.MySqlConnectionStringBuilder", null);
            _types.Add("MySql.Data.MySqlClient.MySqlDbType", null);
        }
        /// <summary>
        /// 获取 MySql.Data.MySqlClient.MySqlConnection 类型
        /// </summary>
        /// <param name="typeFullName">类型全名。</param>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetType(string typeFullName, bool @throw = false) {
            if (string.IsNullOrEmpty(typeFullName))
                return null;
            System.Type type = _types[typeFullName];
            if (type == null) {
                string typeName = typeFullName + ", Mysql.Data";
                type = FastWrapper.GetWarpperType(typeName, "Mysql.Data.dll");
                if (type == null && @throw)
                    CommonException.ThrowTypeLoad(typeName);
                _types[typeFullName] = type;
            }
            return type;
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建 MysqlDatabaseProvider 的实例。
        /// </summary>
        public MysqlDatabaseProvider() {
        }
        #endregion

        #region IDatabaseProvider 成员
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        public override IDbConnection CreateConnection(string connectionString) {
            System.Type type = GetConnectionType(true);
            return (IDbConnection)FastWrapper.CreateInstance(type, connectionString);// new MySql.Data.MySqlClient.MySqlConnection(connectionString);
        }
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        public override IDbConnection CreateConnection(object connectionOptions) {
            System.Data.Common.DbConnectionStringBuilder builder = FastWrapper.CreateInstance<System.Data.Common.DbConnectionStringBuilder>(GetType("MySql.Data.MySqlClient.MySqlConnectionStringBuilder", true));
            builder["pooling"] = true;
            //builder["MaxPoolSize"] = 1024;
            //builder["Port"] = 5432;
            builder["Charset"] = "utf8";
            Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>(connectionOptions);
            SetBuilderValue(builder, "SslMode", "None");
            SetBuilderValue(builder, values, "port", "Port");
            SetBuilderValue(builder, values, "host", "Data Source", p=> {
                string p10 = p as string;
                if (string.IsNullOrEmpty(p10))
                    return p;
                if (p10.IndexOf(':') > -1) {
                    string[] pair = p10.Split(':');
                    SetBuilderValue(builder, "Port", pair[1]);
                    return pair[0];
                }
                return p;
            });
            SetBuilderValue(builder, values, "name", "Database");
            SetBuilderValue(builder, values, "account", "User ID");
            SetBuilderValue(builder, values, "password", "Password");
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in values) {
                //builder[item.Key] = item.Value;
                SetBuilderValue(builder, item.Key, item.Value);
            }
            return CreateConnection(builder.ConnectionString);
        }
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public override IDataContext CreateDataContext(IDbConnection connection) {
            return new MysqlDataContext(this, connection);
        }

        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        public override string PreName(string name) {
            if (name.IndexOfAny(new char[] { '`', '.', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                return name;
            if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                return name;
            if (name.StartsWith("[") && name.EndsWith("]"))
                name = name.Substring(1, name.Length - 2);
            return '`' + name + '`';
        }
        #endregion

        #endregion

        #region methods


        #region GetConnectionType
        private static readonly System.Type[] _connectionTypes = new System.Type[1];
        /// <summary>
        /// 获取 MySql.Data.MySqlClient.MySqlConnection 类型
        /// </summary>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetConnectionType(bool @throw=false) {
            return GetType("MySql.Data.MySqlClient.MySqlConnection", @throw);
        }
        #endregion
        #region GetDbType
        /// <summary>
        /// 获取 Npgsql.NpgsqlConnection 类型
        /// </summary>
        /// <param name="throw">是否报错</param>
        /// <returns></returns>
        public static System.Type GetDbType(bool @throw=false) {
            return GetType("MySql.Data.MySqlClient.MySqlDbType", @throw);
        }
        #endregion

        #endregion

        #region types
        class MysqlDataContext : DataContext, IDataContext {


            #region ctor
            /// <summary>
            /// 创建 MysqlDataContext 的实例
            /// </summary>
            /// <param name="provider">数据库提供者</param>
            /// <param name="connection">数据库连接</param>
            public MysqlDataContext(IDatabaseProvider provider, IDbConnection connection) : base(provider, connection) {
            }
            #endregion


            #region methods

            #region Command

            #region PreParameter
            /// <summary>
            /// 预处理参数
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public override object PreParameter(string parameterName, object value) {
                if (value == null || value is IDataParameter)
                    return value;

                object param2 = value;
                if (value != null) {
                    System.Type type = value.GetType();
                    System.Type dbType = GetDbType();
                    if (TypeExtensions.IsAnonymousType(type) || TypeExtensions.IsInheritFrom(type, typeof(System.Collections.Generic.IDictionary<string, object>))) {
                        CommandParameter p = new CommandParameter() {
                            Name = parameterName,
                            RealType = typeof(object),
                            Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true),
                            Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                                MySqlDbType = TypeExtensions.Convert("Json", dbType),
                            })
                        };
                        param2 = p;
                    } else if (type.IsEnum) {
                        CommandParameter p = new CommandParameter() {
                            Name = parameterName,
                            RealType = typeof(long),
                            Value = TypeExtensions.Convert<long>(value),
                        };
                        param2 = p;
                    } else if (type.IsArray && type.GetElementType() == typeof(string)) {
                        CommandParameter p = new CommandParameter() {
                            Name = parameterName,
                            RealType = typeof(string[]),
                            Value = value,
                            Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                                MySqlDbType = TypeExtensions.Convert("Json", dbType),
                            })
                        };
                        param2 = p;
                    }
                }
                return param2;
            }
            #endregion

            #region ExecuteScalar
            
            /// <summary>
            /// 执行查询，并返回查询的第一条记录的第一个列。
            /// </summary>
            /// <param name="commandText">查询语句</param>
            /// <param name="action">可以对command对象进行操作，这发生在处理@params之前。</param>
            /// <param name="params">参数列表，可以为null或不填。</param>
            /// <returns>返回查询结果。</returns>
            public override object ExecuteScalar(string commandText, System.Action<IDbCommand> action, params object[] @params) {
                lock (this) {
                    object result = null;
                    string commandText2 = commandText;
                    bool insert = commandText.IndexOf("insert ", System.StringComparison.OrdinalIgnoreCase) > -1;
                    using (IDbCommand command = CreateCommand(commandText2, action, @params)) {
                        command.UpdatedRowSource = UpdateRowSource.None;
                        result = command.ExecuteScalar();
                        if(insert)
                            result= FastWrapper.Get(command, "LastInsertedId");
                        if (result is System.DBNull) {
                            result = null;
                        }
                    }
                    return result;
                }
            }
            #endregion
            #region ExecuteFunction
            /// <summary>
            /// 调用函数
            /// </summary>
            /// <param name="name">函数名称，格式：[dbo].[fun1]</param>
            /// <param name="params">参数列表</param>
            /// <returns>返回此函数的执行结果</returns>
            public override object ExecuteFunction(string name, params object[] @params) {
                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name");
                string body = "select " + name;
                body += '(';
                if (@params != null) {
                    for (int i = 0; i < @params.Length; i++) {
                        if (i > 0)
                            body += ',';
                        body += "@p" + (i + 1);
                    }
                }
                body += ')';
                return ExecuteScalar(body, @params);
            }
            #endregion
            #region ExecuteStoredProcedure
            /// <summary>
            /// 调用存储过程
            /// </summary>
            /// <param name="name">存储过程名称，格式：[dbo].[Stored1]</param>
            /// <param name="params">参数列表，可以为null；key以out_开头的，会自动识别为output类型；字符串类型的长度默认为255，可以写成out_3_name，表示长度为3，节省资源。</param>
            /// <returns>返回存储过程的值。</returns>
            public override object ExecuteStoredProcedure(string name, System.Collections.Generic.IDictionary<string, object> @params) {
                if (string.IsNullOrEmpty(name))
                    throw new System.ArgumentNullException("name");
                object result = null;

                using (IDbCommand command = CreateCommand(name, null, null)) {
                    command.CommandType = CommandType.StoredProcedure;
                    System.Collections.Generic.Dictionary<string, string> outKeys = null;
                    if (@params != null) {
                        //for (var i = 0; i < pairs.Length; i++) {
                        //    string key =(string) pairs[i];
                        //    if (key[0] != '@')
                        //        key = '@' + key;
                        //    object value = null;
                        //    if (i + 1 < pairs.Length) {
                        //        i++;
                        //        value = pairs[i];
                        //    }

                        //    param=command.AddParameter(key, value);

                        //    //if (param.DbType == DbType.String)
                        //    //    param.Size = 255; 
                        //}

                        outKeys = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
                        //IDbDataParameter param;
                        foreach (string key in @params.Keys) {
                            if (@params[key] is IDataParameter) {
                                command.Parameters.Add(@params[key]);
                                continue;
                            }
                            if (key[0] != '@') {
                                string key2 = key;
                                bool isOut = false;
                                if (key.StartsWith("out_", System.StringComparison.OrdinalIgnoreCase)) {
                                    key2 = key.Substring(4);
                                    outKeys.Add(key, key2);
                                    isOut = true;
                                }
                                IDbDataParameter p = IDbCommandExtensions.AddParameter(command,'@' + key2, @params[key]);
                                if (isOut) {
                                    p.Direction = ParameterDirection.Output;
                                    if (p.DbType == DbType.String) {
                                        if (key2.IndexOf('_') != -1) {
                                            string[] ps = key2.Split('_');
                                            p.Size = TypeExtensions.Convert<int>(ps[0],255);
                                            outKeys.Remove(key);
                                            key2 = ps[1];
                                            outKeys.Add(key, key2);
                                            p.ParameterName = '@' + key2;
                                        } else {
                                            p.Size = 255;
                                        }
                                    }
                                    //Console.WriteLine("{0}=[DbType={1},Size={2}]", p.ParameterName, p.DbType, p.Size);
                                }
                            } else {
                                IDbCommandExtensions.AddParameter(command,key, @params[key]);
                            }
                        }
                    }
                    IDbDataParameter returnValue = IDbCommandExtensions.AddParameter(command);
                    returnValue.Direction = ParameterDirection.ReturnValue;


                    command.UpdatedRowSource = UpdateRowSource.None;
                    command.ExecuteNonQuery();
                    if (outKeys != null) {
                        foreach (System.Collections.Generic.KeyValuePair<string, string> item in outKeys) {
                            @params[item.Key] = ((IDbDataParameter)command.Parameters['@' + item.Value]).Value;
                        }
                    }
                    result = returnValue.Value;
                }

                return result;
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
                if(command.IndexOf("{db.name}", System.StringComparison.OrdinalIgnoreCase)>-1)
                    command = StringExtensions.Replace(command, "{db.name}", _databaseName, true);
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
                    name = _databaseName;
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
            protected override IDataQuery<object> CreateQuery(IDbCommand command, System.Type type) {
                return new DataQuery<object>(this, command, type);
            }
            /// <summary>
            /// 创建一个查询
            /// </summary>
            /// <typeparam name="T">任意类型</typeparam>
            /// <param name="command">命令对象</param>
            protected override IDataQuery<T> CreateQuery<T>(IDbCommand command) {
                return new DataQuery<T>(this, command);
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
            public override bool TableExists(string tableName,string schemaName="@default") {
                if (string.IsNullOrEmpty(tableName))
                    return false;
                if (string.IsNullOrEmpty(schemaName) || string.Equals(schemaName, "@default", System.StringComparison.OrdinalIgnoreCase))
                    schemaName = _databaseName;
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
                    schemaName = _databaseName;
                return TypeExtensions.Convert<bool>(ExecuteScalar("SELECT 1 FROM  information_schema.COLUMNS WHERE TABLE_SCHEMA=@p1 AND table_name=@p2 AND COLUMN_NAME=@p3;", schemaName, tableName,columnName), false);
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
                        schemaName = _databaseName;
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
                    schemaName = _databaseName;
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
            public static string ForeignKey(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey) {
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
                ExecuteNonQuery(string.Format("drop index \"fki_{0}\"",  key));
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
                return new SelectCommandBuilder(this, tableName, null);
            }
            /// <summary>
            /// 创建查询命令构造器。
            /// </summary>
            /// <param name="tableName">表名。</param>
            /// <param name="commandText">查询命令。</param>
            /// <returns>返回构造器对象。</returns>
            public override ISelectCommandBuilder CreateSelect(string tableName, string commandText) {
                return new SelectCommandBuilder(this, tableName, commandText);
            }
            #endregion
            #region CreateInsert
            /// <summary>
            /// 创建插入命令构造器。
            /// </summary>
            /// <param name="tableName">表名。</param>
            /// <returns>返回构造器对象。</returns>
            public override IInsertCommandBuilder CreateInsert(string tableName) {
                return new InsertCommandBuilder(this, tableName);
            }
            #endregion
            #region CreateUpdate
            /// <summary>
            /// 创建更新命令构造器。
            /// </summary>
            /// <param name="tableName">表名。</param>
            /// <returns>返回构造器对象。</returns>
            public override IUpdateCommandBuilder CreateUpdate(string tableName) {
                return new UpdateCommandBuilder(this, tableName);
            }
            #endregion

            #endregion

            #endregion
        }

        class SelectCommandBuilder : Symbol.Data.SelectCommandBuilder, ISelectCommandBuilder {


            #region ctor
            public SelectCommandBuilder(IDataContext dataContext, string tableName, string commandText) 
                :base(dataContext, tableName,commandText){
            }
            #endregion

            #region methods

            #region PreName
            /// <summary>
            /// 对字段、通用名称进行预处理（语法、方言等）
            /// </summary>
            /// <param name="name">字段、通用名称</param>
            /// <returns>返回处理后的名称。</returns>
            public override string PreName(string name) {
                if (name.IndexOfAny(new char[] { '`', '.', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (name.StartsWith("[") && name.EndsWith("]"))
                    name = name.Substring(1, name.Length - 2);
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    return _tableName;
                return '`' + name + '`';
            }
            #endregion

            #region Parse
            /// <summary>
            /// 解析命令脚本。
            /// </summary>
            /// <param name="commandText">命令脚本。</param>
            protected override void Parse(string commandText) {
                commandText = StringExtensions.Replace(
                                StringExtensions.Replace(
                                    commandText, "select*", "select *", true),
                                "*from ", " * from", true);

                int i = commandText.IndexOf("select ", System.StringComparison.OrdinalIgnoreCase);
                if (i == -1)//没有select ，无效
                    throw new System.InvalidOperationException("没有“select ”：" + commandText);
                //if (i != 0)
                //    SelectBefore = commandText.Substring(0, i);
                i += "select ".Length;
                int j = commandText.IndexOf(" from", i, System.StringComparison.OrdinalIgnoreCase);
                if (j == -1)//没有from
                    throw new System.InvalidOperationException("没有“ from ”：" + commandText);
                bool top = ParseFields(commandText.Substring(i, j - i));//取出列
                //if (!top) {
                {
                    int ix = commandText.IndexOf("limit ", System.StringComparison.OrdinalIgnoreCase);
                    if (ix != -1) {
                        //_limitMode = true;
                        var match = System.Text.RegularExpressions.Regex.Match(commandText, "limit\\s*(\\d+),(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (match.Success) {
                            SkipCount = TypeExtensions.Convert<int>(match.Groups[1].Value, 0);
                            TakeCount = TypeExtensions.Convert<int>(match.Groups[2].Value, 0);
                            commandText = commandText.Replace(match.Value, "");
                        } else {
                            match = System.Text.RegularExpressions.Regex.Match(commandText, "limit\\s*(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (match.Success) {
                                TakeCount = TypeExtensions.Convert<int>(match.Groups[1].Value, 0);
                                commandText = commandText.Replace(match.Value, "");
                            }
                        }
                    }

                }
               
                j += " from ".Length;//推进到form 后面
                i = commandText.IndexOf(" where ", j, System.StringComparison.OrdinalIgnoreCase);//尝试找到where
                if (i != -1) {
                    PaseWhereBefore(commandText.Substring(j, i - j));//分析WhereBefore
                    i += " where ".Length;
                    j = commandText.IndexOf(" order by", i, System.StringComparison.OrdinalIgnoreCase);
                    if (j == -1) {
                        j = commandText.Length;
                    } else {
                        ParseOrderBy(commandText.Substring(j + " order by".Length));
                    }
                    ParseWhere(commandText.Substring(i, j - i));
                } else {
                    int j2 = commandText.IndexOf(" order by", j, System.StringComparison.OrdinalIgnoreCase);
                    if (j2 != -1) {
                        PaseWhereBefore(commandText.Substring(j, j2 - j));//分析WhereBefore
                        ParseOrderBy(commandText.Substring(j2 + " order by".Length));
                    } else {
                        PaseWhereBefore(commandText.Substring(j));
                    }
                }
            }
            void PaseWhereBefore(string text) {
                if (string.IsNullOrEmpty(text))
                    return;
                int i1 = text.IndexOf(' ');
                int i2 = text.IndexOf("\"");
                if (i2 == -1) {
                    i2 = text.IndexOf("]");
                }
                int i = System.Math.Max(i1, i2);
                if (i == -1) {
                    _tableName = text;
                    return;
                }
                i++;
                int j = text.IndexOf('"', i);
                if (j == -1) {
                    _tableName = text.Substring(0, i).Trim();
                } else {
                    _tableName = text.Substring(i,j-i).Trim();
                    i = j + 1;
                }
                if (!IsCustomTable)
                    _tableName = _tableName.Trim('[', ']','"');
                if (i == text.Length)
                    return;
                _whereBefores.Add(text.Substring(i));
            }
            private bool ParseFields(string fields) {
                fields = fields.Trim();
                if (string.IsNullOrEmpty(fields))
                    return false;
                int i = fields.IndexOf("top ", System.StringComparison.OrdinalIgnoreCase);
                bool top = false;
                if (i != -1) {
                    i += "top ".Length;
                    int j = fields.IndexOf(' ', i);
                    if (j != -1) {
                        TakeCount = TypeExtensions.Convert<int>(fields.Substring(i, j - i), 0);
                        top = true;
                        fields = fields.Substring(j + 1);
                    } else {
                        fields = fields.Substring(i);
                    }
                }
                System.Collections.Generic.ICollectionExtensions.AddRange(_fields, fields.Split(','));
                return top;
            }
            private void ParseWhere(string expressions) {
                Where(WhereOperators.And, expressions);
            }
            private void ParseOrderBy(string orderbys) {
                orderbys = orderbys.Trim();
                if (string.IsNullOrEmpty(orderbys))
                    return;
                System.Collections.Generic.ICollectionExtensions.AddRange(_orderbys, orderbys.Split(','));
            }
            #endregion

            #region BuilderCommandText

            /// <summary>
            /// 构造命令脚本。
            /// </summary>
            /// <returns>返回命令脚本。</returns>
            protected override string BuilderCommandText() {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                BuildSelect(builder);
                BuildWhereBefore(builder);
                BuildWhere(builder);
                BuildOrderBy(builder);
                BuildSkip(builder);
                return builder.ToString();
            }
            void BuildSkip(System.Text.StringBuilder builder) {
                if (SkipCount > 0 || TakeCount > 0) {
                    builder.AppendFormat(" limit {0},{1}", SkipCount, TakeCount);
                }
            }
            
            #endregion

            #endregion
        }
        class InsertCommandBuilder : Symbol.Data.InsertCommandBuilder {

            #region ctor
            public InsertCommandBuilder(IDataContext dataContext, string tableName)
                : base(dataContext, tableName) {
            }
            #endregion

            #region methods

            #region PreName
            /// <summary>
            /// 对字段、通用名称进行预处理（语法、方言等）
            /// </summary>
            /// <param name="name">字段、通用名称</param>
            /// <returns>返回处理后的名称。</returns>
            public override string PreName(string name) {
                if (name.IndexOfAny(new char[] { '`', '.', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (name.StartsWith("[") && name.EndsWith("]"))
                    name = name.Substring(1, name.Length - 2);
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    return _tableName;
                return '`' + name + '`';
            }
            #endregion

            #region PreFieldValueWrapper
            /// <summary>
            /// 预处理：字段值包装处理
            /// </summary>
            /// <param name="propertyDescriptor">反射对象。</param>
            /// <param name="value">值。</param>
            /// <param name="commandParameter">参数对象。</param>
            protected override void PreFieldValueWrapper(System.ComponentModel.PropertyDescriptor propertyDescriptor, object value, CommandParameter commandParameter) {
                System.Reflection.PropertyInfo propertyInfo = propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name);
                bool isJson = TypeExtensions.Convert<bool>(ConstAttributeExtensions.Const(propertyInfo, "SaveAsJson"), false);
                {
                    System.Type dbType = GetDbType();
                    string dbTypeValue = null;
                    string dataTypeName = FastObject.Path(Symbol.Serialization.Json.Parse(
                        ConstAttributeExtensions.Const(propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name), "TableField")
                        ), "Type") as string;
                    if (!string.IsNullOrEmpty(dataTypeName)) {
                        if (string.Equals(dataTypeName, "jsonb", System.StringComparison.OrdinalIgnoreCase)) {
                            isJson = true;
                            dbTypeValue = "Json";// MysqlTypes.MySqlDbType .Jsonb;
                        } else if (string.Equals(dataTypeName, "json", System.StringComparison.OrdinalIgnoreCase)) {
                            isJson = true;
                            dbTypeValue = "Json";// MysqlTypes.MySqlDbType .Json;
                        }
                    }
                    if (dbType != null && dbTypeValue != null) {
                        commandParameter.Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                            MySqlDbType = TypeExtensions.Convert(dbTypeValue, dbType),
                        });
                    }
                }

                if (!isJson) {
                    if (propertyDescriptor.PropertyType.IsClass && propertyDescriptor.PropertyType != typeof(string))
                        isJson = true;
                }
                if (isJson) {
                    commandParameter.Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true);
                }

                if (propertyDescriptor.PropertyType.IsEnum) {
                    commandParameter.RealType = typeof(long);
                    commandParameter.Value = TypeExtensions.Convert<long>(value);
                } else if (propertyDescriptor.PropertyType.IsArray && propertyDescriptor.PropertyType.GetElementType() == typeof(string)) {
                    System.Type dbType = GetDbType();
                    commandParameter.Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                        MySqlDbType = TypeExtensions.Convert("Json", dbType),
                    });
                }
            }
            #endregion

            #region PreValues
            /// <summary>
            /// 预处理参数列表。
            /// </summary>
            protected override void PreValues() {
                base.PreValues();
                foreach (string key in _fields.AllKeys) {
                    object value = _fields[key];
                    if (value == null || value is CommandParameter)
                        continue;
                    System.Type type = value.GetType();
                    if (!type.IsClass || type == typeof(string)) {
                        continue;
                    }
                    CommandParameter p = new CommandParameter() {
                        Name = key,
                        RealType = typeof(string),
                        Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true),
                        Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                            MySqlDbType = TypeExtensions.Convert("JSON", GetDbType()),
                        })
                    };
                    _fields[key] = p;
                }
            }
            #endregion

            #endregion
        }
        class UpdateCommandBuilder : Symbol.Data.UpdateCommandBuilder {

            #region ctor
            public UpdateCommandBuilder(IDataContext dataContext, string tableName)
                : base(dataContext, tableName) {
            }
            #endregion

            #region methods

            #region PreName
            /// <summary>
            /// 对字段、通用名称进行预处理（语法、方言等）
            /// </summary>
            /// <param name="name">字段、通用名称</param>
            /// <returns>返回处理后的名称。</returns>
            public override string PreName(string name) {
                if (name.IndexOfAny(new char[] { '`', '.', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (name.StartsWith("[") && name.EndsWith("]"))
                    name = name.Substring(1, name.Length - 2);
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    return _tableName;
                return '`' + name + '`';
            }
            #endregion

            #region PreFieldValueWrapper
            /// <summary>
            /// 预处理：字段值包装处理
            /// </summary>
            /// <param name="propertyDescriptor">反射对象。</param>
            /// <param name="value">值。</param>
            /// <param name="commandParameter">参数对象。</param>
            protected override void PreFieldValueWrapper(System.ComponentModel.PropertyDescriptor propertyDescriptor, object value, CommandParameter commandParameter) {
                System.Reflection.PropertyInfo propertyInfo = propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name);
                bool isJson = TypeExtensions.Convert<bool>(ConstAttributeExtensions.Const(propertyInfo, "SaveAsJson"), false);
                {
                    System.Type dbType = GetDbType();
                    string dbTypeValue = null;
                    string dataTypeName = FastObject.Path(Symbol.Serialization.Json.Parse(
                        ConstAttributeExtensions.Const(propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name), "TableField")
                        ), "Type") as string;
                    if (!string.IsNullOrEmpty(dataTypeName)) {
                        if (string.Equals(dataTypeName, "jsonb", System.StringComparison.OrdinalIgnoreCase)) {
                            isJson = true;
                            dbTypeValue = "Json";// MysqlTypes.MySqlDbType .Jsonb;
                        } else if (string.Equals(dataTypeName, "json", System.StringComparison.OrdinalIgnoreCase)) {
                            isJson = true;
                            dbTypeValue = "Json";// MysqlTypes.MySqlDbType .Json;
                        }
                    }
                    if (dbType != null && dbTypeValue != null) {
                        commandParameter.Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                            MySqlDbType = TypeExtensions.Convert(dbTypeValue, dbType),
                        });
                    }
                }

                if (!isJson) {
                    if (propertyDescriptor.PropertyType.IsClass && propertyDescriptor.PropertyType != typeof(string))
                        isJson = true;
                }
                if (isJson) {
                    commandParameter.Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true);
                }
                if (propertyDescriptor.PropertyType.IsEnum) {
                    commandParameter.RealType = typeof(long);
                    commandParameter.Value = TypeExtensions.Convert<long>(value);
                } else if (propertyDescriptor.PropertyType.IsArray && propertyDescriptor.PropertyType.GetElementType() == typeof(string)) {
                    System.Type dbType = GetDbType();
                    commandParameter.Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                        MySqlDbType = TypeExtensions.Convert("Json", dbType),
                    });
                }
            }
            #endregion

            #region DateTimeNowGrammar
            /// <summary>
            /// Like 语法
            /// </summary>
            /// <returns></returns>
            protected override string DateTimeNowGrammar() {
                return "now()";
            }
            #endregion

            #region PreValues
            /// <summary>
            /// 预处理参数列表。
            /// </summary>
            protected override void PreValues() {
                base.PreValues();
                foreach (string key in _fields.AllKeys) {
                    object value = _fields[key];
                    if (value == null || value is CommandParameter)
                        continue;
                    System.Type type = value.GetType();
                    if (!type.IsClass || type == typeof(string)) {
                        continue;
                    }
                    CommandParameter p = new CommandParameter() {
                        Name = key,
                        RealType = typeof(string),
                        Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true),
                        Properties = Symbol.Collections.Generic.NameValueCollection<object>.As(new {
                            MySqlDbType = TypeExtensions.Convert("JSON", GetDbType()),
                        })
                    };
                    _fields[key] = p;
                }
            }
            #endregion

            #region CreateSelect
            /// <summary>
            /// 创建Select命令构造器。
            /// </summary>
            /// <returns></returns>
            protected override ISelectCommandBuilder CreateSelect() {
                return new SelectCommandBuilder(_dataContext, _tableName, "");
            }
            #endregion


            #endregion
        }
        #endregion
    }
}

