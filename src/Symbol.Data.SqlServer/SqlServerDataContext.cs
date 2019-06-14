/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {


    /// <summary>
    /// SqlServer 数据上下文。
    /// </summary>
    public abstract class SqlServerDataContext : AdoDataContext {

        #region ctor
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public SqlServerDataContext(IConnection connection) 
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
            return new SqlServerCommand(this);
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
                path = System.IO.Path.Combine(path, Connection?.OriginalDatabaseName + "_" + name + ".ndf");
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
GO", Connection?.OriginalDatabaseName, name, path, initSize));
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
GO", Connection?.OriginalDatabaseName, name));
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
        /// 判断表是否存在（指定架构）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        public override bool TableExists(string tableName, string schemaName = "@default") {
            if (string.IsNullOrEmpty(tableName))
                return false;
            return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from sysobjects where id=object_id(@p1)", tableName), false);
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
            return TypeExtensions.Convert<bool>(ExecuteScalar("select 1 from syscolumns where id=object_id(@p1) and name=@p2", tableName, columnName), false);
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
            ExecuteNonQuery(string.Format("alter table {0} DROP CONSTRAINT {1}", foreignKeyTableName, key));
        }
        #endregion


        #endregion

        #region Builder

       
        #region CreateInsert
        /// <summary>
        /// 创建插入命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IInsertCommandBuilder CreateInsert(string tableName) {
            return new SqlServerInsertCommandBuilder(this, tableName);
        }
        #endregion
        #region CreateUpdate
        /// <summary>
        /// 创建更新命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public override IUpdateCommandBuilder CreateUpdate(string tableName) {
            return new SqlServerUpdateCommandBuilder(this, tableName);
        }
        #endregion

        #endregion

        #endregion
    }

}

