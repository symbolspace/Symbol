/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Collections.Generic;
using System.Data;

namespace Symbol.Data {


    /// <summary>
    /// 数据上下文接口。
    /// </summary>
    public interface IDataContext :
        IDataContextNoSQL,      //NoSQL接口
        IDisposable {

        #region properties
        /// <summary>
        /// 获取或设置待释放的对象列表。
        /// </summary>
        DisposableObjectList DisposableObjects { get; }
        /// <summary>
        /// 获取或设置数据绑定缓存对象。
        /// </summary>
        Binding.IDataBinderObjectCache DataBinderObjectCache { get; set; }

        /// <summary>
        /// 获取数据库提供者。
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// 获取连接池。
        /// </summary>
        IConnectionPool Connections { get; }

        /// <summary>
        /// 获取连接对象。
        /// </summary>
        IConnection Connection { get; }
        /// <summary>
        /// 获取事务对象。
        /// </summary>
        ITransaction Transaction { get; }

        /// <summary>
        /// 获取当前数据库连接字符串。
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 获取或设置查询执行超时时间。默认100秒。
        /// </summary>
        int CommandTimeout { get; set; }
        #endregion

        #region methods
        #region 事务
        /// <summary>
        /// 开启事务，自动创建事务对象。
        /// </summary>
        /// <returns>返回相关联的事务对象。</returns>
        void BeginTransaction();
        /// <summary>
        /// 提交事务，如果没有开启事务，调用后没有任何效果。
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 回滚事务，如果没有开启事务，调用后没有任何效果。
        /// </summary>
        void RollbackTransaction();
        #endregion
        #region 数据库
        /// <summary>
        /// 变更当前数据库（默认）。
        /// </summary>
        void ChangeDatabase();
        /// <summary>
        /// 变更当前数据库（指定）。
        /// </summary>
        /// <param name="database">数据库名称。</param>
        void ChangeDatabase(string database);
        #endregion

        #region 命令

        #region CreateCommand
        /// <summary>
        /// 创建原始命令执行器。
        /// </summary>
        /// <param name="commandText">查询语句文本。</param>
        /// <param name="action">附加操作回调，为null不影响。</param>
        /// <param name="params">参数列表，可以直接是值，也可以是CommandParameter类型。</param>
        /// <returns>返回命令执行器。</returns>
        ICommand CreateCommand(string commandText, Action<ICommand> action, params object[] @params);
        /// <summary>
        /// 创建原始命令执行器。
        /// </summary>
        /// <param name="commandText">查询语句文本。</param>
        /// <param name="action">附加操作回调，为null不影响。</param>
        /// <param name="params">参数列表，可以直接是值，也可以是CommandParameter类型。</param>
        /// <returns>返回命令执行器。</returns>
        ICommand CreateCommand(string commandText, Action<ICommand> action, IEnumerable<object> @params);
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>返回查询结果。</returns>
        object ExecuteScalar(string commandText, params object[] @params);
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可以对command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>返回查询结果。</returns>
        object ExecuteScalar(string commandText, Action<ICommand> action, params object[] @params);
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>返回查询结果。</returns>
        T ExecuteScalar<T>(string commandText, params object[] @params);
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回查询结果。</returns>
        T ExecuteScalar<T>(string commandText, object[] @params,T defaultValue) where T:struct;

        #endregion
        #region ExecuteNonQuery
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        int ExecuteNonQuery(string commandText, params object[] @params);
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可以对command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        int ExecuteNonQuery(string commandText, Action<ICommand> action, params object[] @params);
        #endregion
        #region ExecuteBlockQuery
        /// <summary>
        /// 批量执行命令
        /// </summary>
        /// <param name="command">命令（SQL）。</param>
        /// <param name="mulitFlag">多段命令分隔符。</param>
        /// <param name="changeDatabase">切换数据库标志。</param>
        void ExecuteBlockQuery(string command, string mulitFlag = "GO", string changeDatabase = "use ");
        #endregion
        #region ExecuteFunction
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <param name="name">函数名称，格式：[dbo].[fun1]</param>
        /// <param name="params">参数列表</param>
        /// <returns>返回此函数的执行结果</returns>
        object ExecuteFunction(string name, params object[] @params);
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <param name="name">函数名称，格式：[dbo].[fun1]</param>
        /// <param name="params">参数列表</param>
        /// <returns>返回此函数的执行结果</returns>
        T ExecuteFunction<T>(string name, params object[] @params);
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="name">函数名称，格式：[dbo].[fun1]</param>
        /// <param name="params">参数列表</param>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回此函数的执行结果</returns>
        T ExecuteFunction<T>(string name, object[] @params, T defaultValue) where T : struct;

        #endregion
        #region ExecuteStoredProcedure
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="name">存储过程名称，格式：[dbo].[Stored1]</param>
        /// <param name="params">参数列表，可以为 null。</param>
        /// <returns>返回存储过程的值。</returns>
        object ExecuteStoredProcedure(string name, object @params);
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="name">存储过程名称，格式：[dbo].[Stored1]</param>
        /// <param name="params">参数列表，可以为null；key以out_开头的，会自动识别为output类型；字符串类型的长度默认为255，可以写成out_3_name，表示长度为3，节省资源。</param>
        /// <returns>返回存储过程的值。</returns>
        object ExecuteStoredProcedure(string name, IDictionary<string, object> @params);
        #endregion

        #endregion

        #region 查询

        #region CreateQuery
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery(string commandText, params object[] @params);
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery(string commandText, Action<ICommand> action, params object[] @params);
        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery(Type type, string commandText, params object[] @params);
        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<object> CreateQuery(Type type, string commandText, Action<ICommand> action, params object[] @params);
        #endregion
        #region CreateQuery`1
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数，可以为null或不传，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<T> CreateQuery<T>(string commandText, params object[] @params);
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数，可以为null，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<T> CreateQuery<T>(string commandText, IEnumerable<object> @params);
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">回调，可以用command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数，可以为null，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<T> CreateQuery<T>(string commandText, Action<ICommand> action, params object[] @params);
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">回调，可以用command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数，可以为null，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        IDataQuery<T> CreateQuery<T>(string commandText, Action<ICommand> action, IEnumerable<object> @params);
        #endregion

        #endregion

        #region Builder

        #region CreateSelect
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        ISelectCommandBuilder CreateSelect(string tableName);
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="commandText">查询命令。</param>
        /// <returns>返回构造器对象。</returns>
        ISelectCommandBuilder CreateSelect(string tableName, string commandText);
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <typeparam name="T">任意可引用类型。</typeparam>
        /// <returns>返回构造器对象。</returns>
        ISelectCommandBuilder CreateSelect<T>() where T : class;
        #endregion
        #region CreateInsert
        /// <summary>
        /// 创建插入命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        IInsertCommandBuilder CreateInsert(string tableName);
        /// <summary>
        /// 创建插入命令构造器(自动填充数据)。
        /// </summary>
        /// <param name="model">实体对象,不能为null。</param>
        /// <exception cref="System.NullReferenceException">model不能为null。</exception>
        /// <returns>返回构造器对象。</returns>
        IInsertCommandBuilder CreateInsert(object model);
        #endregion
        #region CreateUpdate
        /// <summary>
        /// 创建更新命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        IUpdateCommandBuilder CreateUpdate(string tableName);
        /// <summary>
        /// 创建更新命令构造器(自动填充数据)。
        /// </summary>
        /// <param name="model">实体对象,不能为null。</param>
        /// <exception cref="System.NullReferenceException">model不能为null。</exception>
        /// <returns>返回构造器对象。</returns>
        IUpdateCommandBuilder CreateUpdate(object model);
        #endregion

        #endregion

        #region Schema

        #region 表空间
        /// <summary>
        /// 获取表空间的位置
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <returns></returns>
        string GetTableSpacePath(string name);
        /// <summary>
        /// 判断表空间是否存在。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <returns>返回判断结果。</returns>
        bool TableSpaceExists(string name);
        /// <summary>
        /// 创建表空间。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <param name="path">路径，为空将自动处理（默认与当前数据库同目录）。</param>
        bool TableSpaceCreate(string name, string path = null);
        /// <summary>
        /// 删除表空间。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        void TableSpaceDelete(string name);
        #endregion
        #region 表
        /// <summary>
        /// 创建表（仅用于简单的逻辑，复杂的创建语句请直接调用ExecuteNonQuery）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columns">列，每一个列请自行拼接好属性。</param>
        void TableCreate(string tableName, params string[] columns);
        /// <summary>
        /// 判断表是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <returns>返回判断结果。</returns>
        bool TableExists(string tableName);
        /// <summary>
        /// 判断表是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        bool TableExists(string tableName, string schemaName = "@default");
        /// <summary>
        /// 删除表。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        void TableDelete(string tableName);
        #endregion
        #region 字段
        /// <summary>
        /// 判断列（字段）是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <returns></returns>
        bool ColumnExists(string tableName, string columnName);
        /// <summary>
        /// 判断列（字段）是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns></returns>
        bool ColumnExists(string tableName, string columnName, string schemaName = "@default");
        /// <summary>
        /// 获取数据库中列（字段）的信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <returns>永远不会返回null。</returns>
        DatabaseTableField GetColumnInfo(string tableName, string columnName);
        /// <summary>
        /// 获取数据库中列（字段）的信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>永远不会返回null。</returns>
        DatabaseTableField GetColumnInfo(string tableName, string columnName, string schemaName = "@default");
        /// <summary>
        /// 获取数据库中表的所有列（字段）信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <returns></returns>
        System.Collections.Generic.List<DatabaseTableField> GetColumns(string tableName);
        /// <summary>
        /// 获取数据库中表的所有列（字段）信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns></returns>
        System.Collections.Generic.List<DatabaseTableField> GetColumns(string tableName, string schemaName = "@default");
        #endregion
        #region 函数
        /// <summary>
        /// 判断函数是否存在。
        /// </summary>
        /// <param name="functionName">函数名称，不带[]等符号。</param>
        /// <returns>返回判断结果。</returns>
        bool FunctionExists(string functionName);
        /// <summary>
        /// 判断函数是否存在。
        /// </summary>
        /// <param name="functionName">函数名称，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        bool FunctionExists(string functionName, string schemaName = "@default");
        #endregion
        #region 外键
        /// <summary>
        /// 创建外键关系。
        /// </summary>
        /// <param name="primaryKeyTableName">主键表名。</param>
        /// <param name="primaryKey">主键列名。</param>
        /// <param name="foreignKeyTableName">外键表名。</param>
        /// <param name="foreignKey">外键列名。</param>
        /// <param name="cascadeDelete">级联删除。</param>
        /// <param name="cascadeUpdate">级联更新。</param>
        void ForeignKeyCreate(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey, bool cascadeDelete = false, bool cascadeUpdate = false);
        /// <summary>
        /// 删除外键关系。
        /// </summary>
        /// <param name="primaryKeyTableName">主键表名。</param>
        /// <param name="primaryKey">主键列名。</param>
        /// <param name="foreignKeyTableName">外键表名。</param>
        /// <param name="foreignKey">外键列名。</param>
        void ForeignKeyDelete(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey);
        #endregion

        #endregion

        #endregion
    }
    /// <summary>
    /// 数据上下文执行回调。
    /// </summary>
    /// <typeparam name="T">任意类型</typeparam>
    /// <param name="dataContext">数据上下文。</param>
    /// <param name="data">数据。</param>
    /// <param name="error">异常对象。</param>
    public delegate void DataContextExecuteCallback<T>(IDataContext dataContext,T data, System.Exception error);


}