/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Symbol.Data {

    /// <summary>
    /// SQLite3数据库提供者
    /// </summary>
    public class SQLite3DatabaseProvider : DatabaseProvider {

        #region ctor
        /// <summary>
        /// 创建 SQLite3DatabaseProvider 的实例。
        /// </summary>
        public SQLite3DatabaseProvider() {
        }
        #endregion

        #region IDatabaseProvider 成员
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        public override IDbConnection CreateConnection(string connectionString) {
            return Symbol.Data.SQLite.SQLiteHelper.CreateConnection(connectionString);
        }
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        public override IDbConnection CreateConnection(object connectionOptions) {
            System.Data.Common.DbConnectionStringBuilder builder = Symbol.Data.SQLite.SQLiteHelper.CreateConnectionStringBuilder();
            Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>(connectionOptions);
            SetBuilderValue(builder, values, "file", "Data Source");
            SetBuilderValue(builder, values, "name", "Data Source");
            SetBuilderValue(builder, values, "memory", "Data Source",p=> {
                if (TypeExtensions.Convert(p, false))
                    return ":memory:";
                return p;
            });
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
            return new SQLite3DataContext(this, connection);
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
        class SQLite3DataContext : DataContext, IDataContext {


            #region ctor
            /// <summary>
            /// 创建 SQLite3DataContext 的实例
            /// </summary>
            /// <param name="provider">数据库提供者</param>
            /// <param name="connection">数据库连接</param>
            public SQLite3DataContext(IDatabaseProvider provider, IDbConnection connection) : base(provider, connection) {
            }
            #endregion


            #region methods

            #region Base
            /// <summary>
            /// 变更当前数据库（指定）。
            /// </summary>
            /// <param name="database">数据库名称。</param>
            public override void ChangeDatabase(string database) {
                
            }
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
            #region Command

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
                        string sql2 = "select last_insert_rowid() as [newId] from {table} limit 0,1";// Setting.NewIdSql;
                        if (!string.IsNullOrEmpty(tableName))
                            sql2 = StringExtensions.Replace(sql2, "{table}", tableName, true);
                        commandText2 += ";\r\n" + sql2;
                        //}
                    }
                    using (IDbCommand command = CreateCommand(commandText2, action, @params)) {
                        command.UpdatedRowSource = UpdateRowSource.None;
                        result = command.ExecuteScalar();
                        command.Parameters.Clear();
                        if ((result == null || result is System.DBNull) && commandText.IndexOf("insert ", System.StringComparison.OrdinalIgnoreCase) != -1) {
                            //if (Setting != null && !string.IsNullOrEmpty(Setting.NewIdSql)) {
                            command.CommandText = "select last_insert_rowid() as [newId] from {table} limit 0,1";// Setting.NewIdSql;
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
                DatabaseTableField item = list.Find(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
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
        internal class SelectCommandBuilder : Symbol.Data.SelectCommandBuilder, ISelectCommandBuilder {

            #region fields
            private bool _limitMode = false;

            #endregion

            #region ctor
            public SelectCommandBuilder(IDataContext dataContext, string tableName, string commandText):
                base(dataContext, tableName,commandText){
                _limitMode = true;
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
                    return _tableName;

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
                _limitMode = true;

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
                //orderbys = PreOffset(orderbys.Trim());
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
                if (name.IndexOfAny(new char[] { '.', '[', ']', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    return _tableName;

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

                    if (type.IsClass) {
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
                if (name.IndexOfAny(new char[] { '.', '[', ']', '(', ')', '=', ' ', ':', '-', '>', '<' }) > -1)
                    return name;
                if (System.Text.RegularExpressions.Regex.IsMatch(name, "^[0-9]+$"))
                    return name;
                if (string.Equals(name, "$self", System.StringComparison.OrdinalIgnoreCase))
                    return _tableName;

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
                    if (type.IsClass) {
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

namespace Symbol.Data.SQLite {


    #region SQLiteHelper
    /// <summary>
    /// SQLite辅助类
    /// </summary>
    public class SQLiteHelper {

        #region fields
        private static readonly bool[] _isDebug;
        private static readonly bool[] _inited;
        private static readonly System.Reflection.Assembly[] _assemblies;
        private static readonly Symbol.Collections.Generic.NameValueCollection<FastWrapper> _types;
        private static readonly Symbol.Collections.Generic.HashSet<string> _typeNames;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置是否为调试模式（调式模式会生成dll文件。）
        /// </summary>
        public static bool IsDebug {
            get { return _isDebug[0]; }
            set { _isDebug[0] = value; }
        }
        #endregion


        #region cctor
        static SQLiteHelper() {
            _isDebug = new bool[] { false };
            _inited = new bool[] { false };
            _assemblies = new System.Reflection.Assembly[1];
            _types = new Collections.Generic.NameValueCollection<FastWrapper>();
            _typeNames = new Collections.Generic.HashSet<string>();
        }
        #endregion

        #region methods

        #region LoadAssembly
        /// <summary>
        /// 初始化程序集System.Data.SQLite(自动检测目录x64 x86)
        /// </summary>
        public static bool LoadAssembly() {
            if (_assemblies[0] != null)
                return true;
            lock (_assemblies) {
                if (_assemblies[0] != null)
                    return true;
                System.Reflection.Assembly assembly;
                bool x64 = IntPtr.Size == 8;
                string runPath = AppHelper.MapPath("~/System.Data.SQLite.dll");
                foreach (string path in new string[] {
                        AppHelper.MapPath("~/"+(x64?"x64":"x86")+"/System.Data.SQLite.dll"),
                        AppHelper.MapPath("~/System.Data.SQLite."+ (x64 ? "x64" : "x86") + ".dll"),
                }) {
                    if (!System.IO.File.Exists(path))
                        continue;
                    AppHelper.CopyFile(path, runPath);
                    break;
                }
                if (System.IO.File.Exists(runPath)) {
                    assembly = System.Reflection.Assembly.LoadFile(runPath);
                    return LoadAssembly(assembly);
                }
                return false;
            }
        }
        /// <summary>
        /// 初始化程序集 System.Data.SQLite 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool LoadAssembly(System.Reflection.Assembly assembly) {
            if (_assemblies[0] != null)
                return true;
            if (assembly == null || assembly.GetName().Name != "System.Data.SQLite")
                return false;
            lock (_assemblies) {
                if (_assemblies[0] != null)
                    return true;
                _assemblies[0] = assembly;
                GetType("System.Data.SQLite.FunctionType");
                GetType("System.Data.SQLite.SQLiteFunctionAttribute");
                SQLiteConvert.Type = GetType("System.Data.SQLite.SQLiteConvert");
                GetType("System.Data.SQLite.SQLiteFunction");
                GetType("System.Data.SQLite.SQLiteFunctionEx");
                GetType("System.Data.SQLite.SQLiteConnectionStringBuilder");
                GetType("System.Data.SQLite.SQLiteConnection");
                GetType("System.Data.SQLite.CollationSequence");

                return true;
            }
        }
        #endregion
        #region GetType
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="name">类型名称</param>
        /// <returns></returns>
        public static FastWrapper GetType(string name) {
            if (string.IsNullOrEmpty(name))
                return null;
            FastWrapper wrapper;
            if (_types.TryGetValue(name, out wrapper))
                return wrapper;
            if (!LoadAssembly())
                return null;

            lock (_assemblies) {
                if (_types.TryGetValue(name, out wrapper))
                    return wrapper;
                try {
                    System.Type type = _assemblies[0].GetType(name);
                    if (type == null)
                        return null;
                    wrapper = new FastWrapper(type, false);
                    _types.Add(name, wrapper);
                    return wrapper;
                } catch {
                    return null;
                }
            }
        }
        #endregion

        #region ExistsFunction
        /// <summary>
        /// 检测函数是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ExistsFunction(string name) {
            if (string.IsNullOrEmpty(name))
                return false;
            if (!LoadAssembly())
                return false;
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteFunction");
            if (wrapper == null)
                return false;
            foreach (object item in (System.Collections.IEnumerable)wrapper.Get("_registeredFunctions")){
                string name2= (string)FastWrapper.Get(item,"Name");
                if (name == name2)
                    return true;
            }
            return false;
        }
        #endregion
        #region RegisterFunction
        /// <summary>
        /// 注册函数
        /// </summary>
        public static int RegisterFunction() {
            lock (_assemblies) {
                int count = 0;
                foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
                    count += RegisterFunction(assembly);
                }
                return count;
            }
        }
        /// <summary>
        /// 注册函数
        /// </summary>
        public static int RegisterFunction(System.Reflection.Assembly assembly) {
            if (assembly == null
#if !net20 && !net35
                || assembly.IsDynamic
#endif
                )
                return 0;
            lock (_assemblies) {
                int count = 0;
                foreach (System.Type type in assembly.GetExportedTypes()) {
                    if (RegisterFunction(type))
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// 注册函数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool RegisterFunction(System.Type type) {
            if (type == null)
                return false;
            if (!type.IsClass || type.IsAbstract || !type.IsPublic)
                return false;
            lock (_assemblies) {
                if (type.Assembly.GetName().Name == "System.Data.SQLite") {
                    if (!LoadAssembly(type.Assembly))
                        return false;
                    GetType("System.Data.SQLite.SQLiteFunction").MethodInvoke("RegisterFunction", type);
                    return true;
                }
                if (!LoadAssembly())
                    return false;
                FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteFunction");
                if (!TypeExtensions.IsInheritFrom(type, typeof(SQLiteFunction)))
                    return false;
                if (!_typeNames.Add(type.FullName))
                    return true;
                System.Type newType = GenerateFunctionType(type);
                if (newType == null)
                    return false;
                wrapper.MethodInvoke("RegisterFunction", newType);
                return true;
            }
        }
        #region GenerateFunctionType
        static System.Type GenerateFunctionType(System.Type functionType) {
            SQLiteFunctionAttribute attribute = AttributeExtensions.GetCustomAttribute<SQLiteFunctionAttribute>(functionType);
            if (attribute == null)
                return null;
            bool ex = TypeExtensions.IsInheritFrom(functionType, typeof(SQLiteFunctionEx)) || attribute.Type == FunctionTypes.Collation;
            FastWrapper baseType = GetType(ex ? "System.Data.SQLite.SQLiteFunctionEx" : "System.Data.SQLite.SQLiteFunction");


            System.Reflection.AssemblyName assemblyName = new System.Reflection.AssemblyName(functionType.Namespace + ".DynamicClass_" + functionType.Name);
            System.Reflection.Emit.AssemblyBuilderAccess accemblyBuilderAccess =
#if netcore
                      System.Reflection.Emit.AssemblyBuilderAccess.Run;
#else
                IsDebug
                    ? System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave
                    : System.Reflection.Emit.AssemblyBuilderAccess.Run;
#endif
            System.Reflection.Emit.AssemblyBuilder assembly =
#if netcore
                System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, accemblyBuilderAccess);
#else
                System.AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, accemblyBuilderAccess);
#endif
#if !netcore
            bool canSave = (accemblyBuilderAccess == System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave || accemblyBuilderAccess == System.Reflection.Emit.AssemblyBuilderAccess.Save);
#endif
            System.Reflection.Emit.ModuleBuilder module =
#if netcore
                      assembly.DefineDynamicModule(assemblyName.Name);
#else

                canSave
                    ? assembly.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll")
                    : assembly.DefineDynamicModule(assemblyName.Name);//, n.Name + ".dll");
#endif
            System.Reflection.Emit.TypeBuilder type = module.DefineType(
                assemblyName.Name + ".DynamicClass",
                System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Sealed | System.Reflection.TypeAttributes.AutoClass,
                baseType.Type,
                System.Type.EmptyTypes);

            {
                FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteFunctionAttribute");
                System.Reflection.PropertyInfo[] properties = new System.Reflection.PropertyInfo[] {
                    wrapper.Type.GetProperty("Name"),
                    wrapper.Type.GetProperty("Arguments"),
                    wrapper.Type.GetProperty("FuncType"),
                };
                System.Reflection.Emit.CustomAttributeBuilder attributeBuilder = new System.Reflection.Emit.CustomAttributeBuilder(wrapper.Type.GetConstructor(System.Type.EmptyTypes), new object[0],
                    properties, new object[] {
                        attribute.Name,
                        attribute.Arguments,
                        TypeExtensions.Convert(attribute.Type,GetType("System.Data.SQLite.FunctionType").Type),
                    });
                type.SetCustomAttribute(attributeBuilder);
            }
            System.Reflection.Emit.FieldBuilder _o = type.DefineField("_o", functionType, FieldAttributes.Private);

            {
                System.Reflection.Emit.ConstructorBuilder ctor = type.DefineConstructor(
                    System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.SpecialName | System.Reflection.MethodAttributes.RTSpecialName,
                    System.Reflection.CallingConventions.HasThis,
                    System.Type.EmptyTypes);
                System.Reflection.Emit.ILGenerator il = ctor.GetILGenerator();
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Call, baseType.Type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, System.Type.EmptyTypes, new System.Reflection.ParameterModifier[0]));
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Newobj, functionType.GetConstructor(System.Type.EmptyTypes));
                il.Emit(System.Reflection.Emit.OpCodes.Stfld, _o);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Ldfld, _o);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                if (attribute.Type == FunctionTypes.Collation) {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
                } else {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
                }
                il.Emit(System.Reflection.Emit.OpCodes.Callvirt, functionType.GetMethod("Init", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, new System.Type[] {
                    typeof(object),typeof(bool)
                }, null));
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Ret);
            }
            CreateMethodDelegate createMethod = (methodInfo, action) => {
                System.Reflection.ParameterInfo[] parameters = methodInfo.GetParameters();
                System.Type[] parameterTypes = new System.Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++) {
                    parameterTypes[i] = parameters[i].ParameterType;
                }
                System.Reflection.Emit.MethodBuilder method = type.DefineMethod(methodInfo.Name, (methodInfo.Attributes | MethodAttributes.NewSlot) ^ MethodAttributes.NewSlot, methodInfo.CallingConvention, methodInfo.ReturnType, parameterTypes);
                for (int i = 0; i < parameters.Length; i++) {
                    System.Reflection.Emit.ParameterBuilder parameter = method.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
                    if (parameters[i].IsOptional) {
                        if (parameters[i].ParameterType.IsValueType && parameters[i].DefaultValue == null)
                            continue;
                        parameter.SetConstant(parameters[i].DefaultValue);
                    }
                }
                System.Reflection.Emit.ILGenerator il = method.GetILGenerator();
                bool hasReturn = (methodInfo.ReturnType != typeof(void));
                System.Reflection.Emit.LocalBuilder @return = null;
                if (hasReturn) {
                    @return = il.DeclareLocal(methodInfo.ReturnType);
                }
                il.Emit(System.Reflection.Emit.OpCodes.Nop);
                il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                il.Emit(System.Reflection.Emit.OpCodes.Ldfld, _o);
                action(functionType.GetMethod(methodInfo.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), method, il);
                il.Emit(System.Reflection.Emit.OpCodes.Ret);
            };
            if (attribute.Type == FunctionTypes.Scalar) {
                createMethod(baseType.Type.GetMethod("Invoke"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Stloc_0);
                    System.Reflection.Emit.Label label = il.DefineLabel();
                    il.Emit(System.Reflection.Emit.OpCodes.Br_S, label);
                    il.MarkLabel(label);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                });
            } else if (attribute.Type == FunctionTypes.Collation) {
                createMethod(baseType.Type.GetMethod("Compare"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_2);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Stloc_0);
                    System.Reflection.Emit.Label label = il.DefineLabel();
                    il.Emit(System.Reflection.Emit.OpCodes.Br_S, label);
                    il.MarkLabel(label);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                });
            } else {
                createMethod(baseType.Type.GetMethod("Final"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Stloc_0);
                    System.Reflection.Emit.Label label = il.DefineLabel();
                    il.Emit(System.Reflection.Emit.OpCodes.Br_S, label);
                    il.MarkLabel(label);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
                });
                createMethod(baseType.Type.GetMethod("Step"), (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_2);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_3);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Nop);
                });
            }
            {
                System.Reflection.MethodInfo methodInfo_base = baseType.Type.GetMethod("Dispose", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new System.Type[] { typeof(bool) }, null);
                createMethod(methodInfo_base, (methodInfo, method, il) => {
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Callvirt, methodInfo);
                    il.Emit(System.Reflection.Emit.OpCodes.Nop);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
                    il.Emit(System.Reflection.Emit.OpCodes.Call, methodInfo_base);
                    il.Emit(System.Reflection.Emit.OpCodes.Nop);
                });
            }

#if netcore20
            var result = type.CreateTypeInfo();
#else
            var result = type.CreateType();
#endif
#if !netcore
            if (canSave) {
                assembly.Save(assemblyName.Name + ".dll");
            }
#endif
            return result;
        }
        delegate void CreateMethodDelegate(System.Reflection.MethodInfo methodInfo, MethodBuilderAction action);
        delegate void MethodBuilderAction(System.Reflection.MethodInfo methodInfo, System.Reflection.Emit.MethodBuilder method, System.Reflection.Emit.ILGenerator il);
        #endregion
        #endregion

        #region CreateFile
        /// <summary>
        /// 创建数据库文件。
        /// </summary>
        /// <param name="file">文件位置，如果已经存在将不会创建，请手动删除。</param>
        public static bool CreateFile(string file) {
            if (string.IsNullOrEmpty(file) || string.Equals(file,":memory:", System.StringComparison.OrdinalIgnoreCase))
                return false;
            if (System.IO.File.Exists(file))
                return true;
            try {
                System.IO.File.WriteAllBytes(file, new byte[0]);
                return true;
            } catch {
                return false;
            }
        }
        #endregion
        #region CreateConnectionStringBuilder
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnectionStringBuilder
        /// </summary>
        /// <returns></returns>
        public static System.Data.Common.DbConnectionStringBuilder CreateConnectionStringBuilder() {
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteConnectionStringBuilder");
            if (wrapper == null)
                return null;
            System.Data.Common.DbConnectionStringBuilder builder = FastWrapper.CreateInstance(wrapper.Type) as System.Data.Common.DbConnectionStringBuilder;
            FastWrapper.Set(builder, "Pooling", true);
            FastWrapper.Set(builder, "FailIfMissing", false);
            builder["journal mode"] = "Off";
            return builder;
        }
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnectionStringBuilder
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static System.Data.Common.DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString) {
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteConnectionStringBuilder");
            if (wrapper == null)
                return null;
            System.Data.Common.DbConnectionStringBuilder builder;
            if (string.IsNullOrEmpty(connectionString)) {
                builder = FastWrapper.CreateInstance(wrapper.Type) as System.Data.Common.DbConnectionStringBuilder;
            } else {
                builder = FastWrapper.CreateInstance(wrapper.Type, connectionString) as System.Data.Common.DbConnectionStringBuilder;
                string file = (string)FastWrapper.Get(builder, "DataSource");
                if(!string.IsNullOrEmpty(file) && !file.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
                    CreateFile(file);
            }
            FastWrapper.Set(builder, "Pooling", true);
            FastWrapper.Set(builder, "FailIfMissing", false);
            builder["journal mode"] = "Off";

            return builder;
        }
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnectionStringBuilder
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static System.Data.Common.DbConnectionStringBuilder CreateConnectionStringBuilder(string file, string password) {
            System.Data.Common.DbConnectionStringBuilder builder = CreateConnectionStringBuilder();
            if (!string.IsNullOrEmpty(file)) {
                CreateFile(file);
                FastWrapper.Set(builder, "DataSource", file);
            }
            FastWrapper.Set(builder, "Pooling", true);
            FastWrapper.Set(builder, "FailIfMissing", false);
            builder["journal mode"] = "Off";
            if (!string.IsNullOrEmpty(password)) {
                FastWrapper.Set(builder, "Password", password);
            }
            return builder;
        }
        #endregion
        #region CreateConnection
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnection
        /// </summary>
        /// <returns></returns>
        public static IDbConnection CreateConnection() {
            return CreateConnection(null);
        }
        /// <summary>
        /// 创建 System.Data.SQLite.SQLiteConnection
        /// </summary>
        /// <param name="connectionString">文件或连接字符串</param>
        /// <returns></returns>
        public static IDbConnection CreateConnection(string connectionString) {
            FastWrapper wrapper = GetType("System.Data.SQLite.SQLiteConnection");
            if (wrapper == null)
                return null;
            if (!_inited[0]) {
                lock (_inited) {
                    if (!_inited[0]) {
                        _inited[0] = true;
                        RegisterFunction();
                    }
                }
            }
            if (string.IsNullOrEmpty(connectionString)) {
                //connectionString = AppHelper.AppFile + ".db";
                return FastWrapper.CreateInstance(wrapper.Type) as IDbConnection;
            }
            bool byFile = (System.IO.File.Exists(connectionString) || connectionString.IndexOf('=') == -1);
            System.Data.Common.DbConnectionStringBuilder builder;
            if (byFile) {
                builder = CreateConnectionStringBuilder(connectionString, null);
            } else {
                builder = CreateConnectionStringBuilder(connectionString);
            }
            if (builder == null)
                return null;
            return FastWrapper.CreateInstance(wrapper.Type, builder.ConnectionString) as IDbConnection;
        }
        #endregion

        #endregion

    }
    #endregion
    #region SQLiteConvert
    /// <summary>
    /// SQLite转换器
    /// </summary>
    public class SQLiteConvert {
        #region fields
        private FastWrapper _wrapper;
        private static readonly FastWrapper[] _types = new FastWrapper[1];
        #endregion
        #region properties
        internal static FastWrapper Type { get { return _types[0]; } set { _types[0] = value; } }
        #endregion
        #region ctor
        internal SQLiteConvert(FastWrapper wrapper) {
            _wrapper = wrapper;
        }
        #endregion
        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] Split(string source, char separator) {
            return _types[0] == null ? new string[0] : (string[])_types[0].MethodInvoke("Split", source, separator);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ToBoolean(object source) {
            return _types[0] == null ? false : (bool)_types[0].MethodInvoke("ToBoolean", source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ToBoolean(string source) {
            return _types[0] == null ? false : (bool)_types[0].MethodInvoke("ToBoolean", source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceText"></param>
        /// <returns></returns>
        public static byte[] ToUTF8(string sourceText) {
            return _types[0] == null ? new byte[0] : (byte[])_types[0].MethodInvoke("ToUTF8", sourceText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativestring"></param>
        /// <param name="nativestringlen"></param>
        /// <returns></returns>
        public static string UTF8ToString(System.IntPtr nativestring, int nativestringlen) {
            return _types[0] == null ? "" : (string)_types[0].MethodInvoke("UTF8ToString", nativestring, nativestringlen);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="julianDay"></param>
        /// <returns></returns>
        public System.DateTime ToDateTime(double julianDay) {
            return _wrapper == null ? System.DateTime.Now : (System.DateTime)_wrapper.MethodInvoke("ToDateTime", julianDay);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateText"></param>
        /// <returns></returns>
        public System.DateTime ToDateTime(string dateText) {
            return _wrapper == null ? System.DateTime.Now : (System.DateTime)_wrapper.MethodInvoke("ToDateTime", dateText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ToJulianDay(System.DateTime value) {
            return _wrapper == null ? 0D : (double)_wrapper.MethodInvoke("ToJulianDay", value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public string ToString(System.DateTime dateValue) {
            return _wrapper == null ? "" : (string)_wrapper.MethodInvoke("ToString", dateValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativestring"></param>
        /// <param name="nativestringlen"></param>
        /// <returns></returns>
        public virtual string ToString(System.IntPtr nativestring, int nativestringlen) {
            return _wrapper == null ? "" : (string)_wrapper.MethodInvoke("ToString", nativestring, nativestringlen);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeValue"></param>
        /// <returns></returns>
        public byte[] ToUTF8(System.DateTime dateTimeValue) {
            return _wrapper == null ? new byte[0] : (byte[])_wrapper.MethodInvoke("ToUTF8", dateTimeValue);
        }
        #endregion
    }
    #endregion
    #region SQLiteFunctionAttribute
    /// <summary>
    /// 扩展函数特性类
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SQLiteFunctionAttribute : System.Attribute {

        #region properties
        /// <summary>
        /// 获取或设置参数个数。
        /// </summary>
        public int Arguments { get; set; }
        /// <summary>
        /// 获取或设置类型。
        /// </summary>
        public FunctionTypes Type { get; set; }
        /// <summary>
        /// 获取或设置名称。
        /// </summary>
        public string Name { get; set; }
        #endregion
    }
    #endregion
    #region FunctionTypes
    /// <summary>
    /// 扩展函数类型集。
    /// </summary>
    public enum FunctionTypes {
        /// <summary>
        /// 执行函数。
        /// </summary>
        [Const("执行函数")]
        Scalar = 0,
        /// <summary>
        /// 聚合函数。
        /// </summary>
        [Const("聚合函数")]
        Aggregate = 1,
        /// <summary>
        /// 排序规则。
        /// </summary>
        [Const("排序规则")]
        Collation = 2,
    }
    #endregion
    #region SQLiteFunction
    /// <summary>
    /// 排序规则类型集
    /// </summary>
    public enum CollationTypes {
        /// <summary>
        /// 自定义
        /// </summary>
        Custom,
        /// <summary>
        /// 二进制
        /// </summary>
        Binary,
        /// <summary>
        /// 未定义
        /// </summary>
        NoCase,
        /// <summary>
        /// 反序
        /// </summary>
        Reverse
    }
    /// <summary>
    /// 排序规则编码集
    /// </summary>
    public enum CollationEncodings {
        /// <summary>
        /// UTF-16-BE
        /// </summary>
        UTF16BE = 3,
        /// <summary>
        /// UTF-16-LE
        /// </summary>
        UTF16LE = 2,
        /// <summary>
        /// UTF-8
        /// </summary>
        UTF8 = 1
    }

    /// <summary>
    /// 排序规则
    /// </summary>
    public class CollationSequence {
        #region fields
        private FastWrapper _wraaper;
        #endregion

        #region properties
        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name {
            get {
                return (string)_wraaper.Get("Name");
            }
        }
        /// <summary>
        /// 获取类型
        /// </summary>
        public CollationTypes Type {
            get {
                return TypeExtensions.Convert<CollationTypes>(_wraaper.Get("Type"));
            }
        }
        /// <summary>
        /// 获取编码
        /// </summary>
        public CollationEncodings Encoding {
            get {
                return TypeExtensions.Convert<CollationEncodings>(_wraaper.Get("Encoding"));
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public CollationSequence(object o) {
            SQLiteHelper.LoadAssembly(o.GetType().Assembly);
            _wraaper = new FastWrapper(SQLiteHelper.GetType("System.Data.SQLite.CollationSequence").Type, false) { Instance = o };
        }
        #endregion

        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public int Compare(string s1, string s2) {
            return (int)_wraaper.MethodInvoke("Compare", s1, s2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public int Compare(char[] c1, char[] c2) {
            return (int)_wraaper.MethodInvoke("Compare", c1, c2);
        }
        #endregion
    }

    /// <summary>
    /// 扩展函数
    /// </summary>
    public abstract class SQLiteFunction {

        #region fields
        private SQLiteConvert _convert=null;
        private FastWrapper _fun=null;
        #endregion

        #region properties
        /// <summary>
        /// 获取转换器
        /// </summary>
        public SQLiteConvert SQLiteConvert {
            get {
                if (_convert == null) {
                    if (_fun == null)
                        return null;
                    object convert = _fun.Get("SQLiteConvert");
                    _convert = new SQLiteConvert(new FastWrapper(SQLiteConvert.Type.Type, false) { Instance = convert });
                }
                return _convert;
            }
        }
        #endregion

        #region ctor
        #endregion

        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ex"></param>
        public void Init(object o, bool ex) {
            SQLiteHelper.LoadAssembly(o.GetType().Assembly);
            _fun = new FastWrapper(SQLiteHelper.GetType(ex ? "System.Data.SQLite.SQLiteFunctionEx" : "System.Data.SQLite.SQLiteFunction").Type, false) { Instance = o };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public virtual int Compare(string param1, string param2) {
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextData"></param>
        /// <returns></returns>
        public virtual object Final(object contextData) {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual object Invoke(object[] args) {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="stepNumber"></param>
        /// <param name="contextData"></param>
        public virtual void Step(object[] args, int stepNumber, ref object contextData) {
        }
        #region GetCollationSequence
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected CollationSequence GetCollationSequence() {
            return new CollationSequence(_fun.Get("GetCollationSequence"));
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// 扩展函数（排序规则、聚合函数）
    /// </summary>
    public abstract class SQLiteFunctionEx : SQLiteFunction {
    }

    #endregion
    class TableSchemaHelper {
        delegate Symbol.Data.DatabaseTableField ParseFieldDelegate(string block, System.Collections.Generic.List<string> array);
        delegate Symbol.Data.DatabaseTableField NextBlockDelegate();
        public System.Collections.Generic.List<Symbol.Data.DatabaseTableField> ParseFieldsByCreate(string commandText) {
            System.Collections.Generic.List<Symbol.Data.DatabaseTableField> list = new System.Collections.Generic.List<Symbol.Data.DatabaseTableField>();
            if (string.IsNullOrEmpty(commandText)) {
                return list;
            }
            //create table 
            string tableName = FilterTableName(Symbol.Text.StringExtractHelper.StringsStartEnd(commandText, "create table ", "("));
            if (string.IsNullOrEmpty(tableName))
                return list;
            string body;
            int index;
            {
                string s = tableName;
                index = Symbol.Text.StringExtractHelper.StringIndexOf(commandText, ref s, false);
                index += s.Length;
                s = "(";
                index = Symbol.Text.StringExtractHelper.StringIndexOf(commandText, ref s, index, false);
                index++;
                body = commandText.Substring(index);
                index = 0;
            }
            ParseFieldDelegate parseField = (block, array) => {
                if (array.Count < 2)
                    return null;
                string name = FilterTableName(array[0]);
                string type = array[1];
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                    return null;
                string values = array.Count > 2 ? array[2] : "";
                Predicate<string> has = (p11) => {
                    return values.IndexOf(p11, StringComparison.OrdinalIgnoreCase) > -1;
                };
                Symbol.Data.DatabaseTableField field = new Symbol.Data.DatabaseTableField() {
                    TableName = tableName,
                    Position = list.Count + 1,
                    Exists = true,
                    Description = "",
                    Name = name,
                    Type = type,
                    IsIdentity = has("AUTOINCREMENT") || has("identity"),
                    IsPrimary = has("PRIMARY KEY"),
                    Nullable = !has("not null"),
                };
                if (type.StartsWith("decimal(", StringComparison.OrdinalIgnoreCase)
                    || type.StartsWith("number(", StringComparison.OrdinalIgnoreCase)
                    || type.StartsWith("money(", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = TypeExtensions.Convert<int>(Symbol.Text.StringExtractHelper.StringsStartEnd(type, "(", ","), 0);
                    field.Scale = TypeExtensions.Convert<int>(Symbol.Text.StringExtractHelper.StringsStartEnd(type, ",", ")"), 0);
                } else if (type.IndexOf('(') > -1) {
                    field.Type = type.Split('(')[0];
                    field.Length = TypeExtensions.Convert<int>(Symbol.Text.StringExtractHelper.StringsStartEnd(type, "(", ")"), 0);
                } else if (type.Equals("tinyint", StringComparison.OrdinalIgnoreCase)
                    || type.Equals("int8", StringComparison.OrdinalIgnoreCase)
                    || type.Equals("bit", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 1;
                } else if (type.Equals("smallint", StringComparison.OrdinalIgnoreCase) || type.Equals("int16", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 2;
                } else if (type.Equals("int", StringComparison.OrdinalIgnoreCase) || type.Equals("int32", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 4;
                } else if (type.Equals("bigint", StringComparison.OrdinalIgnoreCase)
                        || type.Equals("integer", StringComparison.OrdinalIgnoreCase)
                        || type.Equals("int64", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 8;
                } else if (type.Equals("real", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 4;
                } else if (type.Equals("float", StringComparison.OrdinalIgnoreCase) || type.Equals("double", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 8;
                }
                //if (has("default")) {
                //    //field.DefaultValue = Symbol.Text.StringExtractHelper.StringsStartEnd(values, "default", ")", " ");
                //}
                return field;
            };
            NextBlockDelegate nextBlock = () => {
                int blockCount = 0;
                bool finded = false;
                int index2 = index;
                System.Collections.Generic.List<string> array = new System.Collections.Generic.List<string>();
                for (int i = index; i < body.Length; i++) {
                    char c = body[i];
                    if (c == '(') {
                        blockCount++;
                        continue;
                    } else if (c == ')') {
                        if (blockCount == 0) {
                            string block = body.Substring(index, i - index);
                            if (finded) {
                                array.Add(body.Substring(index2, i - index2));
                            }
                            index = i + 1;
                            return parseField(block, array);
                        }
                        blockCount--;
                    } else if (c == ',') {
                        if (blockCount > 0)
                            continue;
                        string block = body.Substring(index, i - index);
                        if (finded) {
                            array.Add(body.Substring(index2, i - index2));
                        }
                        index = i + 1;
                        return parseField(block, array);
                    } else if (c == ' ') {
                        if (!finded) {
                            finded = true;
                        }
                        if (array.Count < 2) {
                            array.Add(body.Substring(index2, i - index2));
                            finded = false;
                            index2 = i + 1;
                        }

                    }
                }
                return null;
            };
            while (true) {
                Symbol.Data.DatabaseTableField field = nextBlock();
                if (field == null)
                    break;
                //Console.WriteLine(Symbol.Serialization.Json.ToString(field, false, true));
                list.Add(field);
            }
            return list;
        }
        string FilterTableName(string name) {
            if (string.IsNullOrEmpty(name))
                return null;
            if (name.IndexOf('.') > -1) {
                string[] lines = name.Split('.');
                name = lines[name.Length - 1];
            }
            name = name.Trim().Trim('[', ']', '(', ')', '{', '}', '"', '`', '\'');
            if (string.IsNullOrEmpty(name))
                return null;
            return name;
        }
    }

}
#region Symbol.Data.SQLite.Functions
namespace Symbol.Data.SQLite.Functions {
    using System;
#pragma warning disable CS1591

    #region getDate
    /// <summary>
    /// 获取当前日期和时间（与Sql Server的完全相同）。
    /// </summary>
    [SQLiteFunction(Name = "getDate", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate : SQLiteFunction {
        public override object Invoke(object[] args) {
            return DateTime.Now;
        }
    }
    #endregion
    #region getDate_long_js
    /// <summary>
    /// 获取当前日期和时间（js new Date().getTime() ）。
    /// </summary>
    [SQLiteFunction(Name = "getDate_long_js", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate_long_js : SQLiteFunction {
        public override object Invoke(object[] args) {
            return HttpUtility.JsTick();
        }
    }
    #endregion
    #region getDate_long
    /// <summary>
    /// 获取当前日期和时间（与Sql Server的完全相同）。
    /// </summary>
    [SQLiteFunction(Name = "getDate_long", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate_long : SQLiteFunction {
        public override object Invoke(object[] args) {
            return DateTime.Now.Ticks;
        }
    }
    #endregion
    #region getDate_long_binary
    /// <summary>
    /// 获取当前日期和时间（与Sql Server的完全相同）。
    /// </summary>
    [SQLiteFunction(Name = "getDate_long_binary", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class getDate_long_binary : SQLiteFunction {
        public override object Invoke(object[] args) {
            return DateTime.Now.ToBinary();
        }
    }
    #endregion
    #region getDate_long_value
    /// <summary>
    /// 获取当前日期和时间（与Sql Server的完全相同）。
    /// </summary>
    [SQLiteFunction(Name = "getDate_long_value", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class getDate_long_value : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d = TypeExtensions.Convert<DateTime?>(args[0]);
            if (d == null)
                return 0L;
            return d.Value.Ticks;
        }
    }
    #endregion
    #region getDateOffset
    /// <summary>
    /// 计算两个日期之间的相差小时数（带小数），若其中一个参数无法转为日期，将直接返回为0。
    /// </summary>
    [SQLiteFunction(Name = "getDateOffset", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class getDateOffset : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            DateTime? d2 = TypeExtensions.Convert<DateTime?>(args[1]);
            if (d1 == null || d2 == null)
                return 0D;
            TimeSpan t = d1.Value - d2.Value;
            return t.TotalHours;
        }
    }
    #endregion
    #region getDateOffset_days
    /// <summary>
    /// 计算两个日期之间的相差天数，若其中一个参数无法转为日期，将直接返回为0。
    /// </summary>
    [SQLiteFunction(Name = "getDateOffset_days", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class getDateOffset_days : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            DateTime? d2 = TypeExtensions.Convert<DateTime?>(args[1]);
            if (d1 == null || d2 == null)
                return 0;
            TimeSpan t = d1.Value.Date - d2.Value.Date;
            return (int)t.TotalDays;
        }
    }
    #endregion
    #region getDateOffset_minutes
    /// <summary>
    /// 计算两个日期之间的相差分钟数（带小数），若其中一个参数无法转为日期，将直接返回为0。
    /// </summary>
    [SQLiteFunction(Name = "getDateOffset_minutes", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class getDateOffset_minutes : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            DateTime? d2 = TypeExtensions.Convert<DateTime?>(args[1]);
            if (d1 == null || d2 == null)
                return 0D;
            TimeSpan t = d1.Value - d2.Value;
            return t.TotalMinutes;
        }
    }
    #endregion
    #region getDateOffset_seconds
    /// <summary>
    /// 计算两个日期之间的相差秒数（带小数），若其中一个参数无法转为日期，将直接返回为0。
    /// </summary>
    [SQLiteFunction(Name = "getDateOffset_seconds", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class getDateOffset_seconds : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            DateTime? d2 = TypeExtensions.Convert<DateTime?>(args[1]);
            if (d1 == null || d2 == null)
                return 0D;
            TimeSpan t = d1.Value - d2.Value;
            return t.TotalSeconds;
        }
    }
    #endregion
    #region toDate
    /// <summary>
    /// 强制转为日期类型
    /// </summary>
    [SQLiteFunction(Name = "toDate", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate : SQLiteFunction {
        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            return TypeExtensions.Convert<DateTime?>(args[0]);
        }
    }
    #endregion
    #region toDate_long
    /// <summary>
    /// 强制转为日期类型
    /// </summary>
    [SQLiteFunction(Name = "toDate_long", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate_long : SQLiteFunction {
        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            long? ticks = TypeExtensions.Convert<long?>(args[0]);
            if (ticks == null)
                return null;
            return new DateTime(ticks.Value);
        }
    }
    #endregion
    #region toDate_long_js
    /// <summary>
    /// 强制转为日期类型(js new Date(long); )
    /// </summary>
    [SQLiteFunction(Name = "toDate_long_js", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate_long_js : SQLiteFunction {
        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            long? ticks = TypeExtensions.Convert<long?>(args[0]);
            if (ticks == null)
                return null;
            return HttpUtility.FromJsTick(ticks.Value);
        }
    }
    #endregion
    #region toDate_long_binary
    /// <summary>
    /// 强制转为日期类型
    /// </summary>
    [SQLiteFunction(Name = "toDate_long_binary", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class toDate_long_binary : SQLiteFunction {

        public override object Invoke(object[] args) {
            if (args[0] == null)
                return null;
            long? binary = TypeExtensions.Convert<long?>(args[0]);
            if (binary == null)
                return null;
            return DateTime.FromBinary(binary.Value);
        }
    }
    #endregion
    #region newId
    /// <summary>
    /// 获取一个Guid（与Sql Server的完全相同）。
    /// </summary>
    [SQLiteFunction(Name = "newId", Arguments = 0, Type = FunctionTypes.Scalar)]
    public class newId : SQLiteFunction {
        public override object Invoke(object[] args) {
            return Guid.NewGuid().ToString("D");
        }
    }
    #endregion
    #region guidToString
    /// <summary>
    /// guid 转为字符串，32位char
    /// </summary>
    [SQLiteFunction(Name = "guidToString", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class guidToString : SQLiteFunction {
        public override object Invoke(object[] args) {
            Guid g;
            if (args[0] is Guid) {
                g = (Guid)args[0];
            } else {
                string p1 = args[0] as string;
                if (string.IsNullOrEmpty(p1))
                    return string.Empty;
                g = new Guid(p1);
            }
            return g.ToString("N");
        }
    }
    #endregion
    #region getLength
    /// <summary>
    /// 获取文本的长度，null或空文本将返回0 。
    /// </summary>
    [SQLiteFunction(Name = "getLength", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class getLength : SQLiteFunction {
        public override object Invoke(object[] args) {
            string p1 = TypeExtensions.Convert<string>(args[0]);
            if (string.IsNullOrEmpty(p1))
                return 0;
            return p1.Length;
        }
    }
    #endregion
    #region getDayNumber
    /// <summary>
    /// 将日期转换为数字格式：20130729，如果无法转换为日期类型，将直接返回0。
    /// </summary>
    [SQLiteFunction(Name = "getDayNumber", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class getDayNumber : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d = TypeExtensions.Convert<DateTime?>(args[0]);
            if (d == null)
                return 0;
            return DateTimeExtensions.ToDayNumber(d.Value);
        }
    }
    #endregion
    #region valuePaddingLeft
    ///// <summary>
    ///// 将数据左边追加一些特定字符，以保持指定的长度。
    ///// </summary>
    //[SQLiteFunction(Name = "valuePaddingLeft", Arguments = 3, Type = FunctionTypes.Scalar)]
    //public class valuePaddingLeft : SQLiteFunction {
    //    public override object Invoke(object[] args) {
    //        return StringExtensions.ValuePaddingLeft(args[0], TypeExtensions.Convert<int>(args[1], 0), TypeExtensions.Convert<char>(args[2]));
    //    }
    //}
    #endregion
    #region randomNext
    /// <summary>
    /// 产生一个随机数，第一个参数：最小值，第二个参数：最大值（始终会小于此值。），第三个参数：是否输出整数。
    /// </summary>
    [SQLiteFunction(Name = "randomNext", Arguments = 3, Type = FunctionTypes.Scalar)]
    public class randomNext : SQLiteFunction {
        public override object Invoke(object[] args) {
            double d1 = TypeExtensions.Convert<double>(args[0], 0D);
            double d2 = TypeExtensions.Convert<double>(args[1], 0D);
            if (d2 <= d1)
                d2 = d1 + 1D;
            if (TypeExtensions.Convert<bool>(args[2], false))
                return (long)(Math.Floor(new Random().NextDouble() * (d2 - d1) + d1));
            else
                return new Random().NextDouble() * (d2 - d1) + d1;
        }
    }
    #endregion
    #region notPeriod
    /// <summary>
    /// 判断指定的时间是否到到期，并返回指定的值。
    /// </summary>
    [SQLiteFunction(Name = "notPeriod", Arguments = 4, Type = FunctionTypes.Scalar)]
    public class notPeriod : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            DateTime? d2 = TypeExtensions.Convert<DateTime?>(args[1]);

            if (d1 == null || d2 == null)
                return TypeExtensions.Convert<bool?>(args[3]);
            if (d1 < d2)
                return TypeExtensions.Convert<bool?>(args[3]);
            return TypeExtensions.Convert<bool?>(args[2]);
        }
    }
    #endregion
    #region checkTime_minutes
    /// <summary>
    /// 判断指定的日期距离当前时间相差的分钟数是否在指定的范围内，如果其中一个参数无法被转换，将直接返回false。
    /// </summary>
    [SQLiteFunction(Name = "checkTime_minutes", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class checkTime_minutes : SQLiteFunction {
        public override object Invoke(object[] args) {
            DateTime? d1 = TypeExtensions.Convert<DateTime?>(args[0]);
            int? min = TypeExtensions.Convert<int?>(args[1]);
            if (d1 == null || min == null)
                return false;
            if ((DateTime.Now - d1.Value).TotalMinutes > min)
                return false;
            return true;
        }
    }
    #endregion
    #region all_bool_3
    /// <summary>
    /// 前三个参数与后三个参数均相等返回true，反之为false。支持1,-1,'yes','no','true','false','true,false'
    /// </summary>
    [SQLiteFunction(Name = "all_bool_3", Arguments = 6, Type = FunctionTypes.Scalar)]
    public class all_bool_3 : SQLiteFunction {

        public override object Invoke(object[] args) {
            if (TypeExtensions.Convert<bool>(args[0], false) == TypeExtensions.Convert<bool>(args[3], false)
                && TypeExtensions.Convert<bool>(args[1], false) == TypeExtensions.Convert<bool>(args[4], false)
                && TypeExtensions.Convert<bool>(args[2], false) == TypeExtensions.Convert<bool>(args[5], false))
                return true;
            return false;
        }
    }
    #endregion
    #region anyValue_2
    /// <summary>
    /// 求任意值，以文本方式判断，只要第一个参数文本长度大于0，就会返回第一个参数，反之是第二个参数。
    /// </summary>
    [SQLiteFunction(Name = "anyValue_2", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class anyValue_2 : SQLiteFunction {
        public override object Invoke(object[] args) {
            string p1 = TypeExtensions.Convert<string>(args[0]);
            if (!string.IsNullOrEmpty(p1))
                return p1;
            return args[1];
        }
    }
    #endregion
    #region anyValue_4
    /// <summary>
    /// 从第一个参数开始，只要发现文本长度大于0，就返回它，反之往后推。
    /// </summary>
    [SQLiteFunction(Name = "anyValue_4", Arguments = 4, Type = FunctionTypes.Scalar)]
    public class anyValue_4 : SQLiteFunction {
        public override object Invoke(object[] args) {
            string p1 = TypeExtensions.Convert<string>(args[0]);
            if (!string.IsNullOrEmpty(p1))
                return p1;
            p1 = TypeExtensions.Convert<string>(args[1]);
            if (!string.IsNullOrEmpty(p1))
                return p1;
            p1 = TypeExtensions.Convert<string>(args[2]);
            if (!string.IsNullOrEmpty(p1))
                return p1;

            return TypeExtensions.Convert<string>(args[3]);
        }
    }
    #endregion
    #region like_html_text
    /// <summary>
    /// 模糊匹配 html，并且只匹配纯文本，不按html代码来匹配。
    /// </summary>
    [SQLiteFunction(Name = "like_html_text", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class like_html_text : SQLiteFunction {
        public override object Invoke(object[] args) {
            string html = args[0] as string;
            string text = args[1] as string;
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text))
                return false;
            html = Symbol.Text.StringExtractHelper.ClearTag(html);
            if (string.IsNullOrEmpty(html))
                return false;
            return html.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
    #endregion
    #region json_get
    /// <summary>
    /// 从一个JSON文本中获取指定路径的数据。
    /// </summary>
    [SQLiteFunction(Name = "json_get", Arguments = 2, Type = FunctionTypes.Scalar)]
    public class json_get : SQLiteFunction {
        public override object Invoke(object[] args) {
            string html = args[0] as string;
            string text = args[1] as string;
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text))
                return null;
            object json = Symbol.Serialization.Json.Parse(html);
            return FastObject.Path(json, text);
        }
    }
    #endregion
    #region json_set
    /// <summary>
    /// 对JSON文本的指定路径进行赋值，并返回操作后的JSON文本。
    /// </summary>
    [SQLiteFunction(Name = "json_set", Arguments = 3, Type = FunctionTypes.Scalar)]
    public class json_set : SQLiteFunction {
        public override object Invoke(object[] args) {
            string html = args[0] as string;
            string text = args[1] as string;
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text))
                return null;
            object json = Symbol.Serialization.Json.Parse(html);
            FastObject.Path(json, text,args[2]);
            return Symbol.Serialization.Json.ToString(json);
        }
    }
    #endregion
    #region json_format
    /// <summary>
    /// 格式化json文本。
    /// </summary>
    [SQLiteFunction(Name = "json_format", Arguments = 1, Type = FunctionTypes.Scalar)]
    public class json_format : SQLiteFunction {
        public override object Invoke(object[] args) {
            string html = args[0] as string;
            return Symbol.Serialization.Json.Format(html);
        }
    }
    #endregion



#pragma warning restore CS1591

}
#endregion