/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;
using System.Reflection;

namespace Symbol.Data {

    /// <summary>
    /// SQLServer2005数据库提供者
    /// </summary>
    public class SqlServer2005DatabaseProvider : DatabaseProvider {


        #region ctor
        /// <summary>
        /// 创建 SqlServer2005DatabaseProvider 的实例。
        /// </summary>
        public SqlServer2005DatabaseProvider() {
        }
        #endregion

        #region IDatabaseProvider 成员
        /// <summary>
        /// 获取是否支持多查询。
        /// </summary>
        public override bool MultipleActiveResultSets {
            get {
                return true;
            }

        }

        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        public override IDbConnection CreateConnection(string connectionString) {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString) {
                MultipleActiveResultSets = true
            };
            return new System.Data.SqlClient.SqlConnection(builder.ConnectionString);
        }
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        public override IDbConnection CreateConnection(object connectionOptions) {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.MaxPoolSize = 1024;
            Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>(connectionOptions);
            SetBuilderValue(builder, values, "host", "Data Source", p => {
                string p10 = p as string;
                if (string.IsNullOrEmpty(p10))
                    return p;
                if (p10.IndexOf(':') > -1) {
                    return p10.Replace(':', ',');
                }
                return p;
            });
            SetBuilderValue(builder, values, "port", "Data Source", p => {
                string p10 = TypeExtensions.Convert<string>(p);
                if (string.IsNullOrEmpty(p10))
                    return null;
                builder.DataSource += "," + p10;
                return null;
            });
            SetBuilderValue(builder, values, "name", "Initial Catalog");
            SetBuilderValue(builder, values, "account", "User ID");
            SetBuilderValue(builder, values, "password", "Password");
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in values) {
                //builder[item.Key] = item.Value;
                SetBuilderValue(builder, item.Key, item.Value);
            }
            if (!builder.PersistSecurityInfo && !builder.IntegratedSecurity) {
                builder.PersistSecurityInfo = true;
            }
            builder.MultipleActiveResultSets = true;
            return new System.Data.SqlClient.SqlConnection(builder.ConnectionString);
        }

        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public override IDataContext CreateDataContext(IDbConnection connection) {
            return new SqlServer2005DataContext(this, connection);
        }
        #region PreName
        /// <summary>
        /// 对字段、通用名称进行预处理（语法、方言等）
        /// </summary>
        /// <param name="name">字段、通用名称</param>
        /// <returns>返回处理后的名称。</returns>
        public override string PreName(string name) {
            if (name.IndexOfAny(new char[] { '.', '[', ']', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                return name;
            if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                return name;
            return '[' + name + ']';
        }
        #endregion

        #endregion

        #region types
        class SqlServer2005DataContext : DataContext, IDataContext {

            #region ctor
            /// <summary>
            /// 创建 SqlServer2005DataContext 的实例
            /// </summary>
            /// <param name="provider">数据库提供者</param>
            /// <param name="connection">数据库连接</param>
            public SqlServer2005DataContext(IDatabaseProvider provider, IDbConnection connection):base(provider,connection) {
            }
            #endregion

            #region methods

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

            #region Command

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
                    if (commandText.IndexOf("insert ", System.StringComparison.OrdinalIgnoreCase) != -1) {
                        //if (Setting != null && Setting.MultiLine && !string.IsNullOrEmpty(Setting.NewIdSql)) {
                        string tableName = Text.StringExtractHelper.StringsStartEnd(commandText, "insert into ", new string[] { "(", " values" }, 0, false, false, false);
                        string sql2 = "select SCOPE_IDENTITY() as [newid]";// Setting.NewIdSql;
                        if (!string.IsNullOrEmpty(tableName))
                            sql2 = StringExtensions.Replace(sql2, "{table}", tableName, true);
                        commandText2 += "\r\n" + sql2;
                        //}
                    }
                    using (IDbCommand command = CreateCommand(commandText2, action, @params)) {
                        command.UpdatedRowSource = UpdateRowSource.None;
                        result = command.ExecuteScalar();
                        command.Parameters.Clear();
                        if ((result == null || result is System.DBNull) && commandText.IndexOf("insert ", System.StringComparison.OrdinalIgnoreCase) != -1) {
                            //if (Setting != null && !string.IsNullOrEmpty(Setting.NewIdSql)) {
                            command.CommandText = "select SCOPE_IDENTITY() as [newid]";// Setting.NewIdSql;
                            string tableName = Text.StringExtractHelper.StringsStartEnd(commandText, "insert into ", new string[] { "(", " values" }, 0, false, false, false);
                            if (!string.IsNullOrEmpty(tableName))
                                command.CommandText = StringExtensions.Replace(command.CommandText, "{table}", tableName, true);
                            result = command.ExecuteScalar();
                            //}
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
                                IDbDataParameter p = IDbCommandExtensions.AddParameter(command, '@' + key2, @params[key]);
                                if (isOut) {
                                    p.Direction = ParameterDirection.Output;
                                    if (p.DbType == DbType.String) {
                                        if (key2.IndexOf('_') != -1) {
                                            string[] ps = key2.Split('_');
                                            p.Size = TypeExtensions.Convert<int>(ps[0], 255);
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
                                IDbCommandExtensions.AddParameter(command, key, @params[key]);
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

            #endregion

            #region Schema

            #region GetTableSpacePath
            /// <summary>
            /// 获取表空间的位置
            /// </summary>
            /// <param name="name">名称，不带[]等符号。</param>
            /// <returns></returns>
            public override string GetTableSpacePath(string name) {
                if (string.IsNullOrEmpty(name))
                    return TypeExtensions.Convert<string>(ExecuteScalar("select top 1 [physical_name] from sys.database_files"));
                return TypeExtensions.Convert<string>(ExecuteScalar("select top 1 [physical_name] from sys.database_files where [name]=@p1", name));
            }

            #endregion
            #region TableSpaceExists
            /// <summary>
            /// 判断表空间是否存在。
            /// </summary>
            /// <param name="name">名称，不带[]等符号。</param>
            /// <returns>返回判断结果。</returns>
            public override bool TableSpaceExists(string name) {
                if (string.IsNullOrEmpty(name))
                    return false;
                return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from sys.database_files where [name]=@p1", name), false);
            }
            #endregion
            #region TableSpaceCreate
            /// <summary>
            /// 创建表空间。
            /// </summary>
            /// <param name="name">名称，不带[]等符号。</param>
            /// <param name="path">路径，为空将自动处理（默认与当前数据库同目录）。</param>
            public override bool TableSpaceCreate(string name, string path = null) {
                if (string.IsNullOrEmpty(name))
                    return false;
                if (TableSpaceExists(name))
                    return true;
                if (string.IsNullOrEmpty(path)) {
                    string dir = GetTableSpacePath(null);
                    path = System.IO.Path.GetDirectoryName(dir);
                    path = System.IO.Path.Combine(path, _databaseName + "_" + name + ".ndf");
                } else {
                    string dir = System.IO.Path.GetDirectoryName(path);
                    AppHelper.CreateDirectory(dir, false);
                }
                int initSize = 5120;
                ExecuteBlockQuery(string.Format(@"
GO
use [master]
GO
ALTER DATABASE [{0}] ADD FILEGROUP [{1}]
GO
ALTER DATABASE [{0}] ADD FILE (NAME = N'{1}', FILENAME = N'{2}', SIZE = {3}
                KB , FILEGROWTH = {3}
                KB ) TO FILEGROUP [{1}]
GO", _databaseName, name, path, initSize));
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
                ExecuteBlockQuery(string.Format(@"
USE [{0}]
GO
ALTER DATABASE [{0}]  REMOVE FILE [{1}]
GO
ALTER DATABASE [{0}] REMOVE FILEGROUP [{1}]
GO", _databaseName, name));
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
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat("create table [{0}](", tableName);
                if (columns != null) {
                    for (int i = 0; i < columns.Length; i++) {
                        if (i > 0)
                            sb.Append(',');
                        if (string.IsNullOrEmpty(columns[i]))
                            continue;
                        sb.Append(columns[i]);
                    }
                }
                sb.Append(')');
                ExecuteNonQuery(sb.ToString());
            }
            #endregion

            /// <summary>
            /// 判断表是否存在。
            /// </summary>
            /// <param name="tableName">表名，不带[]等符号。</param>
            /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
            /// <returns>返回判断结果。</returns>
            public override bool TableExists(string tableName, string schemaName = "@default") {
                if (string.IsNullOrEmpty(tableName))
                    return false;
                return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from sysobjects where id=object_id(@p1)", tableName), false);
            }

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
                return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from syscolumns where id=object_id(@p1) and name=@p2", tableName, columnName), false);
            }
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
                    item = CreateQuery<DatabaseTableField>(@"
select
     1 as [Exists]
     ,obj.name as TableName,c.name as [Name],t.name as [Type],c.colid as [Position]
     ,convert(bit,c.IsNullable)  as [Nullable]
     ,convert(bit,case when exists(select 1 from sysobjects where xtype='PK' and parent_obj=c.id and name in (
         select name from sysindexes where indid in(
             select indid from sysindexkeys where id = c.id and colid=c.colid))) then 1 else 0 end) 
                 as [IsPrimary]
     ,convert(bit,COLUMNPROPERTY(c.id,c.name,'IsIdentity')) as [IsIdentity]
     --,c.Length as [ByteLength] 
     ,COLUMNPROPERTY(c.id,c.name,'PRECISION') as [Length]
     ,isnull(COLUMNPROPERTY(c.id,c.name,'Scale'),0) as [Scale]
     ,ISNULL(CM.text,'') as [DefaultValue]
     ,isnull(ETP.value,'') AS [Description]
     --,ROW_NUMBER() OVER (ORDER BY C.name) AS [Row]
from syscolumns c
inner join systypes t on c.xusertype = t.xusertype 
left join sys.extended_properties ETP on ETP.major_id = c.id and ETP.minor_id = c.colid and ETP.name ='MS_Description' 
left join syscomments CM on c.cdefault=CM.id
left join sys.objects obj on obj.object_id=c.id
where obj.type='U' and c.id = object_id(@p1) and c.name=@p2
order by c.id,c.colid
                ", tableName, columnName).FirstOrDefault();
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
                return CreateQuery<DatabaseTableField>(@"
select
     1 as [Exists]
     ,obj.name as TableName,c.name as [Name],t.name as [Type],c.colid as [Position]
     ,convert(bit,c.IsNullable)  as [Nullable]
     ,convert(bit,case when exists(select 1 from sysobjects where xtype='PK' and parent_obj=c.id and name in (
         select name from sysindexes where indid in(
             select indid from sysindexkeys where id = c.id and colid=c.colid))) then 1 else 0 end) 
                 as [IsPrimary]
     ,convert(bit,COLUMNPROPERTY(c.id,c.name,'IsIdentity')) as [IsIdentity]
     --,c.Length as [ByteLength] 
     ,COLUMNPROPERTY(c.id,c.name,'PRECISION') as [Length]
     ,isnull(COLUMNPROPERTY(c.id,c.name,'Scale'),0) as [Scale]
     ,ISNULL(CM.text,'') as [DefaultValue]
     ,isnull(ETP.value,'') AS [Description]
     --,ROW_NUMBER() OVER (ORDER BY C.name) AS [Row]
from syscolumns c
inner join systypes t on c.xusertype = t.xusertype 
left join sys.extended_properties ETP on ETP.major_id = c.id and ETP.minor_id = c.colid and ETP.name ='MS_Description' 
left join syscomments CM on c.cdefault=CM.id
left join sys.objects obj on obj.object_id=c.id
where obj.type='U' and c.id = object_id(@p1)
order by c.id,c.colid
                ", tableName).ToList();
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
                return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from sysobjects where xtype='fn' and name=@p1", functionName), false);
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
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat(
                    "alter table [{0}] add CONSTRAINT [{1}] FOREIGN KEY ([{2}]) REFERENCES [{3}]([{4}])",
                    foreignKeyTableName,
                    ForeignKey(primaryKeyTableName, primaryKey, foreignKeyTableName, foreignKey),
                    foreignKey,
                    primaryKeyTableName,
                    primaryKey);
                if (cascadeDelete)
                    sb.Append(" ON DELETE CASCADE");
                if (cascadeUpdate)
                    sb.Append(" ON UPDATE CASCADE");
                ExecuteNonQuery(sb.ToString());
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
                ExecuteNonQuery(string.Format("alter table {0} DROP CONSTRAINT {1}", foreignKeyTableName, key));
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
 
        class SelectCommandBuilder : Symbol.Data.SelectCommandBuilder {

            #region fields
            private bool _limitMode = false;
            private bool _sql2012 = false;
            #endregion

            #region ctor
            public SelectCommandBuilder(IDataContext dataContext, string tableName, string commandText) 
                : base(dataContext, tableName, commandText) {
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
                if (name.IndexOfAny(new char[] { '.', '[', ']', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    name = TableName;
                return '[' + name + ']';
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
                if (!top) {
                    int ix = commandText.IndexOf("limit ", System.StringComparison.OrdinalIgnoreCase);
                    if (ix != -1) {
                        _limitMode = true;
                        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(commandText, "limit\\s*(\\d+),(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        SkipCount = TypeExtensions.Convert<int>(match.Groups[1].Value, 0);
                        TakeCount = TypeExtensions.Convert<int>(match.Groups[2].Value, 0);
                        commandText = commandText.Replace(match.Value, "");
                    }
                }
                j += " from ".Length;//推进到form 后面
                i = commandText.IndexOf(" where ", j, System.StringComparison.OrdinalIgnoreCase);//尝试找到where
                if (i != -1) {
                    PaseWhereBefore(commandText.Substring(j, i - j));//分析WhereBefore
                    i += " where ".Length;
                    j = commandText.IndexOf(" order by", i, System.StringComparison.OrdinalIgnoreCase);
                    if (j == -1) {
                        j = commandText.IndexOf("\norder by", i, System.StringComparison.OrdinalIgnoreCase);
                    } 
                    if (j == -1) {
                        j = commandText.Length;
                    } else {
                        ParseOrderBy(commandText.Substring(j + " order by".Length));
                    }
                    ParseWhere(commandText.Substring(i, j - i));
                } else {
                    int j2 = commandText.IndexOf(" order by", j, System.StringComparison.OrdinalIgnoreCase);
                    if (j == -1) {
                        j = commandText.IndexOf("\norder by", i, System.StringComparison.OrdinalIgnoreCase);
                    } 
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
                bool b = false;
                int i = -1;
                if (text[0] == '(') {
                    int x = text.IndexOf(')', 1);
                    if (x > -1) {
                        b = true;
                        _tableName = text.Substring(0, x + 1);
                        text = text.Substring(x + 1);
                        i = 0;
                    }
                }
                if (!b) {
                    int i1 = text.IndexOf(' ');
                    int i2 = text.IndexOf("]");
                    i = System.Math.Max(i1, i2);
                    if (i == -1) {
                        _tableName = text;
                        return;
                    }
                    i++;
                    _tableName = text.Substring(0, i).Trim();
                }
                if (!IsCustomTable)
                    _tableName = _tableName.Trim('[', ']');
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
                orderbys = PreOffset(orderbys.Trim());
                if (string.IsNullOrEmpty(orderbys))
                    return;

                System.Collections.Generic.ICollectionExtensions.AddRange(_orderbys, orderbys.Split(','));
            }
            string PreOffset(string orderbys) {
                if (string.IsNullOrEmpty(orderbys))
                    return null;
                string regex = "offset[\\s\\S]+(?<offset>[0-9]+)[\\s\\S]+rows[\\s\\S]+fetch[\\s\\S]+next[\\s\\S]+(?<next>[0-9]+)[\\s\\S]+rows[\\s\\S]+only[\\s\\S;]*|offset[\\s\\S]+(?<offset>[0-9]+)[\\s\\S]+rows[\\s\\S;]*";
                //string regex2 = "offset[\\s\\S]+([0-9]+)[\\s\\S]+rows[\\s\\S;]*";
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(orderbys, regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //if (!match.Success) {
                //    match = System.Text.RegularExpressions.Regex.Match(orderbys, regex2, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //}
                if (!match.Success) {
                    return orderbys;
                }
                _sql2012 = true;
                SkipCount = TypeExtensions.Convert<int>(match.Groups["offset"].Value, 0);
                int takeCount = TypeExtensions.Convert<int>(match.Groups["next"].Value, 0);
                if (takeCount > 0) {
                    TakeCount = takeCount;
                }
                //if (match.Groups.Count > 2) {

                //}
                return orderbys.Replace(match.Value, "");
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
                if (_limitMode) {
                    if (SkipCount > 0 || TakeCount > 0) {
                        builder.AppendFormat(" limit {0},{1}", SkipCount, TakeCount);
                    }
                } else {
                    if (_sql2012) {
                        if (TakeCount < 1 && SkipCount < 1)
                            return;
                        if (_orderbys.Count == 0) {
                            builder.AppendLine(" order by 1 ");
                        }
                        if (TakeCount < 1) {
                            builder.AppendFormat(" offset {0} rows", SkipCount);
                            return;
                        }

                        builder.AppendFormat(" offset {0} rows fetch next {1} rows only", SkipCount < 0 ? 0 : SkipCount, TakeCount);
                    } else {
                        if (SkipCount == 0)
                            return;
                        builder.Insert(0, "select * from( select row_number() over(order by [t_Row_Number]) as [Row_Number],* from (\r\n\r\n");
                        builder.Append("\r\n) t ) tt where [Row_Number]>").Append(SkipCount);
                    }
                }
            }
            /// <summary>
            /// 构造select脚本
            /// </summary>
            /// <param name="builder">构造缓存。</param>
            protected override void BuildSelect(System.Text.StringBuilder builder) {
                builder.AppendLine(" select ");
                if (!_limitMode && !_sql2012) {
                    int top = TakeCount + SkipCount;
                    if (top > 0) {
                        builder.Append("    top ").Append(top).AppendLine(" ");
                    }
                }

                if (!_limitMode && !_sql2012) {
                    if (SkipCount > 0) {
                        builder.AppendLine("    [t_Row_Number]=0,");
                    }
                }
                BuildSelectFields(builder);
                BuildFrom(builder);
            }
            #endregion

            /// <summary>
            /// 匹配操作符预处理。
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            protected override string MatchOperatorPre(string value) {
                if (value == "!=")
                    return "<>";
                return base.MatchOperatorPre(value);
            }

            #endregion
        }
        class InsertCommandBuilder : Symbol.Data.InsertCommandBuilder {

            #region ctor
            public InsertCommandBuilder(IDataContext dataContext, string tableName)
                :base(dataContext, tableName){
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
                if (name.IndexOfAny(new char[] { '.', '[', ']', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    name = TableName;
                return '[' + name + ']';
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
                if (!isJson) {
                    if (propertyDescriptor.PropertyType.IsClass && propertyDescriptor.PropertyType != typeof(string))
                        isJson = true;
                }
                if (isJson) {
                    commandParameter.Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true);
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
#if NETDNX
                    var typeInfo = type.GetTypeInfo();
#else
                    var typeInfo = type;
#endif
                    if (typeInfo.IsClass) {
                        CommandParameter p = new CommandParameter() {
                            Name = key,
                            RealType = typeof(string),
                            Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true),
                        };
                        _fields[key] = p;
                    }
                }
            }
            #endregion

            #endregion
        }
        class UpdateCommandBuilder : Symbol.Data.UpdateCommandBuilder {

            #region ctor
            public UpdateCommandBuilder(IDataContext dataContext, string tableName)
                :base(dataContext, tableName){
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
                if (name.IndexOfAny(new char[] { '.', '[', ']', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    name = TableName;
                return '[' + name + ']';
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
                if (!isJson) {
                    if (propertyDescriptor.PropertyType.IsClass && propertyDescriptor.PropertyType != typeof(string))
                        isJson = true;
                }
                if (isJson) {
                    commandParameter.Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true);
                }
            }
            #endregion

            #region DateTimeNowGrammar
            /// <summary>
            /// Like 语法
            /// </summary>
            /// <returns></returns>
            protected override string DateTimeNowGrammar() {
                return "getDate()";
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
#if NETDNX
                    var typeInfo = type.GetTypeInfo();
#else
                    var typeInfo = type;
#endif
                    if (typeInfo.IsClass) {
                        CommandParameter p = new CommandParameter() {
                            Name = key,
                            RealType = typeof(string),
                            Value = value == null ? null : Symbol.Serialization.Json.ToString(value, true),
                        };
                        _fields[key] = p;
                    }
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

