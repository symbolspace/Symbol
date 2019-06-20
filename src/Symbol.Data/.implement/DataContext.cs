/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {
    /// <summary>
    /// 数据上下文基类
    /// </summary>
    public abstract class DataContext : IDataContext {

        #region fields
        private Binding.IDataBinderObjectCache _dataBinderObjectCache;

        private DisposableObjectList _disposableObjectList;
        private IConnectionPool _connections;


        #endregion

        #region properties
        /// <summary>
        /// 获取或设置待释放的对象列表。
        /// </summary>
        public DisposableObjectList DisposableObjects { get { return ThreadHelper.InterlockedGet(ref _disposableObjectList); } }
        /// <summary>
        /// 获取或设置数据绑定缓存对象。
        /// </summary>
        public Binding.IDataBinderObjectCache DataBinderObjectCache {
            get { return ThreadHelper.InterlockedGet(ref _dataBinderObjectCache); }
            set {
                ThreadHelper.InterlockedSet(ref _dataBinderObjectCache, value);
                if (value != null)
                    DisposableObjects?.Add(value);
            }
        }

        /// <summary>
        /// 获取数据库提供者。
        /// </summary>
        public IProvider Provider {
            get { return Connections?.Provider; }
        }
        /// <summary>
        /// 获取连接池。
        /// </summary>
        public IConnectionPool Connections { get { return ThreadHelper.InterlockedGet(ref _connections); } }

        /// <summary>
        /// 获取连接对象。
        /// </summary>
        public IConnection Connection { get { return Connections?.Master; } }
        /// <summary>
        /// 获取事务对象。
        /// </summary>
        public ITransaction Transaction { get { return Connection?.Transaction; } }

        /// <summary>
        /// 获取当前数据库连接字符串。
        /// </summary>
        public string ConnectionString {
            get { return Connection?.ConnectionString; }
        }

        /// <summary>
        /// 获取或设置查询执行超时时间。默认100秒。
        /// </summary>
        public int CommandTimeout { get; set; }


        #endregion

        #region ctor
        /// <summary>
        /// 创建 DataContext 的实例
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public DataContext(IConnection connection) {
            CommandTimeout = 100;
            _disposableObjectList = new DisposableObjectList();
            _connections = CreateConnectionPool(connection);
            _disposableObjectList.Add(_connections);
        }
        #endregion

        #region methods

        /// <summary>
        /// 创建连接池对象。
        /// </summary>
        /// <param name="master">主连接对象。</param>
        /// <returns>返回连接池对象。</returns>
        protected virtual IConnectionPool CreateConnectionPool(IConnection master) {
            return new ConnectionPool(master);
        }

        #region Base
        #region 事务
        /// <summary>
        /// 开启事务，自动创建事务对象。
        /// </summary>
        public virtual void BeginTransaction() {
            Transaction?.Begin();
        }
        /// <summary>
        /// 提交事务，如果没有开启事务，调用后没有任何效果。
        /// </summary>
        public virtual void CommitTransaction() {
            Transaction?.Commit();
        }
        /// <summary>
        /// 回滚事务，如果没有开启事务，调用后没有任何效果。
        /// </summary>
        public virtual void RollbackTransaction() {
            Transaction?.Rollback();
        }
        #endregion
        #region ChangeDatabase
        /// <summary>
        /// 变更当前数据库（默认）。
        /// </summary>
        public virtual void ChangeDatabase() {
            Connection?.ChangeDatabase();
        }
        /// <summary>
        /// 变更当前数据库（指定）。
        /// </summary>
        /// <param name="database">数据库名称。</param>
        public virtual void ChangeDatabase(string database) {
            Connection?.ChangeDatabase(database);
        }
        #endregion

        #endregion

        #region Command

        #region CreateCommand
        /// <summary>
        /// 创建原始命令执行器。
        /// </summary>
        /// <param name="commandText">查询语句文本。</param>
        /// <param name="action">附加操作回调，为null不影响。</param>
        /// <param name="params">参数列表，可以直接是值，也可以是CommandParameter类型。</param>
        /// <returns>返回命令执行器。</returns>
        public virtual ICommand CreateCommand(string commandText, System.Action<ICommand> action, params object[] @params) {
            return CreateCommand(commandText, action, (System.Collections.Generic.IEnumerable<object>)@params);
        }
        /// <summary>
        /// 创建原始命令执行器。
        /// </summary>
        /// <param name="commandText">查询语句文本。</param>
        /// <param name="action">附加操作回调，为null不影响。</param>
        /// <param name="params">参数列表，可以直接是值，也可以是CommandParameter类型。</param>
        /// <returns>返回命令执行器。</returns>
        public virtual ICommand CreateCommand(string commandText, System.Action<ICommand> action, System.Collections.Generic.IEnumerable<object> @params) {
            CommonException.CheckArgumentNull(commandText, "commandText");

            var command = CreateCommand();
            command.Parameters.AddRange(@params);
            command.Text = commandText;
            command.Timeout = CommandTimeout;
            action?.Invoke(command);
            return command;
        }
        /// <summary>
        /// 创建命令对象。
        /// </summary>
        /// <returns>返回创建的命令对象。</returns>
        protected abstract ICommand CreateCommand();
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>返回查询结果。</returns>
        public object ExecuteScalar(string commandText, params object[] @params) {
            using (var command = CreateCommand(commandText, null, @params)) {
                return command.ExecuteScalar();
            }
        }
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可以对command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>返回查询结果。</returns>
        public object ExecuteScalar(string commandText, System.Action<ICommand> action, params object[] @params) {
            using (var command = CreateCommand(commandText, action, @params)) {
                return command.ExecuteScalar();
            }
        }
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>返回查询结果。</returns>
        public T ExecuteScalar<T>(string commandText, params object[] @params) {
            using (var command = CreateCommand(commandText, null, @params)) {
                return command.ExecuteScalar<T>();
            }
        }
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回查询结果。</returns>
        public virtual T ExecuteScalar<T>(string commandText, object[] @params, T defaultValue) where T : struct {
            using (var command = CreateCommand(commandText, null, @params)) {
                return command.ExecuteScalar<T>(defaultValue);
            }
        }
        #endregion
        #region ExecuteNonQuery
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        public virtual int ExecuteNonQuery(string commandText, params object[] @params) {
            using (var command = CreateCommand(commandText, null, @params)) {
                return command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可以对command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数列表，可以为null或不填。</param>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        public virtual int ExecuteNonQuery(string commandText, System.Action<ICommand> action, params object[] @params) {
            using (var command = CreateCommand(commandText, action, @params)) {
                return command.ExecuteNonQuery();
            }
        }
        #endregion
        #region ExecuteBlockQuery
        /// <summary>
        /// 批量执行命令
        /// </summary>
        /// <param name="command">命令（SQL）。</param>
        /// <param name="mulitFlag">多段命令分隔符。</param>
        /// <param name="changeDatabase">切换数据库标志。</param>
        public abstract void ExecuteBlockQuery(string command, string mulitFlag = "GO", string changeDatabase = "use ");
        #endregion

        #region ExecuteFunction
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <param name="name">函数名称，格式：[dbo].[fun1]</param>
        /// <param name="params">参数列表</param>
        /// <returns>返回此函数的执行结果</returns>
        public virtual object ExecuteFunction(string name, params object[] @params) {
            using (var command = CreateCommand(name, null, @params)) {
                return command.ExecuteFunction();
            }
        }
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <param name="name">函数名称，格式：[dbo].[fun1]</param>
        /// <param name="params">参数列表</param>
        /// <returns>返回此函数的执行结果</returns>
        public virtual T ExecuteFunction<T>(string name, params object[] @params) {
            using (var command = CreateCommand(name, null, @params)) {
                return command.ExecuteFunction<T>();
            }
        }
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="name">函数名称，格式：[dbo].[fun1]</param>
        /// <param name="params">参数列表</param>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回此函数的执行结果</returns>
        public virtual T ExecuteFunction<T>(string name, object[] @params, T defaultValue) where T : struct {
            using (var command = CreateCommand(name, null, @params)) {
                return command.ExecuteFunction<T>(defaultValue);
            }
        }

        #endregion
        #region ExecuteStoredProcedure
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="name">存储过程名称，格式：[dbo].[Stored1]</param>
        /// <param name="params">参数列表，可以为 null。</param>
        /// <returns>返回存储过程的值。</returns>
        public virtual object ExecuteStoredProcedure(string name, object @params) {
            System.Collections.Generic.IDictionary<string, object> values = null;
            if (@params != null) {
                @values = new Symbol.Collections.Generic.NameValueCollection<object>(@params);
            }
            return ExecuteStoredProcedure(name, @values);
        }
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="name">存储过程名称，格式：[dbo].[Stored1]</param>
        /// <param name="params">参数列表，可以为null；key以out_开头的，会自动识别为output类型；字符串类型的长度默认为255，可以写成out_3_name，表示长度为3，节省资源。</param>
        /// <returns>返回存储过程的值。</returns>
        public virtual object ExecuteStoredProcedure(string name, System.Collections.Generic.IDictionary<string, object> @params) {
            using (var command = CreateCommand(name, null, @params)) {
                return command.ExecuteStoredProcedure();
            }
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
        protected abstract IDataQuery<object> CreateQuery(ICommand command, System.Type type);
        /// <summary>
        /// 创建一个查询
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="command">命令对象</param>
        protected abstract IDataQuery<T> CreateQuery<T>(ICommand command);
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery(string commandText, params object[] @params) {
            return CreateQuery(commandText, null, @params);
        }

        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery(string commandText, System.Action<ICommand> action, params object[] @params) {
            //lock (this) {
            ICommand command = CreateCommand(commandText, action, @params);
            var q = CreateQuery(command, null);
            q.DataBinderObjectCache = DataBinderObjectCache;
            DisposableObjects?.Add(q);
            return q;
            //}
        }
        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery(System.Type type, string commandText, params object[] @params) {
            return CreateQuery(type, commandText, null, @params);
        }

        /// <summary>
        /// 创建一个普通查询
        /// </summary>
        /// <param name="type">成员类型，可以模拟出泛型的感觉。</param>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">可对command对象进行操作，这发生在处理@params之前</param>
        /// <param name="params">参数列表，可以为null或不传。</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<object> CreateQuery(System.Type type, string commandText, System.Action<ICommand> action, params object[] @params) {
            //lock (this) {
            ICommand command = CreateCommand(commandText, action, @params);
            var q = CreateQuery(command, type);
            q.DataBinderObjectCache = DataBinderObjectCache;
            DisposableObjects?.Add(q);
            return q;
            //}
        }
        #endregion
        #region CreateQuery`1
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数，可以为null或不传，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<T> CreateQuery<T>(string commandText, params object[] @params) {
            return CreateQuery<T>(commandText, null, @params);
        }
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="params">参数，可以为null，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<T> CreateQuery<T>(string commandText, System.Collections.Generic.IEnumerable<object> @params) {
            return CreateQuery<T>(commandText, null, @params);
        }
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">回调，可以用command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数，可以为null，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<T> CreateQuery<T>(string commandText, System.Action<ICommand> action, params object[] @params) {
            return CreateQuery<T>(commandText, action, (System.Collections.Generic.IEnumerable<object>)@params);
        }
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="action">回调，可以用command对象进行操作，这发生在处理@params之前。</param>
        /// <param name="params">参数，可以为null，自动以@p1开始</param>
        /// <returns>返回一个数据查询对象，不遍历和读取数据，它不会产生数据查询行为。</returns>
        public virtual IDataQuery<T> CreateQuery<T>(string commandText, System.Action<ICommand> action, System.Collections.Generic.IEnumerable<object> @params) {
            //lock (this) {
            ICommand command = CreateCommand(commandText, action, @params);
            var q = CreateQuery<T>(command);
            q.DataBinderObjectCache = DataBinderObjectCache;
            DisposableObjects?.Add(q);

            return q;
            //}
        }

        #endregion

        #endregion

        #region NoSQL

        #region FindAll
        /// <summary>
        /// 创建一个普通查询，遍历的成员类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        public virtual IDataQuery<object> FindAll(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Query(condition, queryFilter)
                       .Sort(sort);
                return CreateQuery(builder.CommandText, builder.Parameters);
            }
        }

        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        public virtual IDataQuery<T> FindAll<T>(object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) where T : class {
            using (ISelectCommandBuilder builder = CreateSelect<T>()) {
                builder.Query(condition, queryFilter)
                       .Sort(sort);
                return CreateQuery<T>(builder.CommandText, builder.Parameters);
            }
        }
        /// <summary>
        /// 创建一个泛型查询
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        public virtual IDataQuery<T> FindAll<T>(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Query(condition, queryFilter)
                       .Sort(sort);
                return CreateQuery<T>(builder.CommandText, builder.Parameters);
            }
        }
        #endregion
        #region Find
        /// <summary>
        /// 查询一条数据，默认类型为IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        public virtual object Find(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Query(condition, queryFilter)
                       .Sort(sort);
                if (builder.WhereCommandText.Length == 0)
                    return null;
                return CreateQuery(builder.CommandText, builder.Parameters).FirstOrDefault();
            }
        }
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        public virtual T Find<T>(object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) where T : class {
            using (ISelectCommandBuilder builder = CreateSelect<T>()) {
                builder.Query(condition, queryFilter)
                       .Sort(sort);
                if (builder.WhereCommandText.Length == 0)
                    return default(T);
                using (var q = CreateQuery<T>(builder.CommandText, builder.Parameters)) {
                    return q.FirstOrDefault();
                }
            }
        }
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">任意类型，如果为系统基础类型，比如string，会自动拿取查询第一列</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="sort">排序规则</param>
        /// <param name="queryFilter">过滤器</param>
        /// <returns></returns>
        public virtual T Find<T>(string collectionName, object condition = null, object sort = null, CommandQueryFilterDelegate queryFilter = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Query(condition, queryFilter)
                       .Sort(sort);
                if (builder.WhereCommandText.Length == 0)
                    return default(T);
                using (var q = CreateQuery<T>(builder.CommandText, builder.Parameters)) {
                    return q.FirstOrDefault();
                }
            }
        }

        #endregion
        #region Exists
        /// <summary>
        /// 是否存在(select 1 xxxxx)
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual bool Exists(string collectionName, object condition = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Select("1")
                       .Query(condition);
                return TypeExtensions.Convert(ExecuteScalar(builder.CommandText, builder.Parameters), false);
            }
        }
        /// <summary>
        /// 是否存在(select 1 xxxxx)
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual bool Exists<TEntity>(object condition = null) where TEntity : class {
            return Exists(typeof(TEntity).Name, condition);
        }
        #endregion

        #region Count
        /// <summary>
        /// 求数量
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual long Count(string collectionName, object condition = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Count()
                       .Query(condition);
                using (var q = CreateQuery<long>(builder.CommandText, builder.Parameters)) {
                    return q.FirstOrDefault();
                }
            }
        }
        /// <summary>
        /// 求数量
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual long Count<TEntity>(object condition = null) where TEntity : class {
            return Count(typeof(TEntity).Name, condition);
        }
        #endregion
        #region Sum
        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Sum<TResult>(string collectionName, string field, object condition = null) where TResult : struct {
            CommonException.CheckArgumentNull(collectionName, "collectionName");
            CommonException.CheckArgumentNull(field, "field");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Sum(field)
                       .Query(condition);
                using (var q = CreateQuery<TResult>(builder.CommandText, builder.Parameters)) {
                    return q.FirstOrDefault();
                }
            }
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Sum<TEntity, TResult>(string field, object condition = null) where TEntity : class where TResult : struct {
            return Sum<TResult>(typeof(TEntity).Name, field, condition);
        }
        #endregion
        #region Min
        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Min<TResult>(string collectionName, string field, object condition = null)  {
            CommonException.CheckArgumentNull(collectionName, "collectionName");
            CommonException.CheckArgumentNull(field, "field");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Min(field)
                       .Query(condition);
                using (var q = CreateQuery<TResult>(builder.CommandText, builder.Parameters)) {
                    return q.FirstOrDefault();
                }
            }
        }
        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Min<TEntity, TResult>(string field, object condition = null) where TEntity : class  {
            return Min<TResult>(typeof(TEntity).Name, field, condition);
        }
        #endregion
        #region Max
        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Max<TResult>(string collectionName, string field, object condition = null)  {
            CommonException.CheckArgumentNull(collectionName, "collectionName");
            CommonException.CheckArgumentNull(field, "field");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Max(field)
                       .Query(condition);
                using (var q = CreateQuery<TResult>(builder.CommandText, builder.Parameters)) {
                    return q.FirstOrDefault();
                }
            }
        }
        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Max<TEntity, TResult>(string field, object condition = null) where TEntity : class  {
            return Max<TResult>(typeof(TEntity).Name, field, condition);
        }
        #endregion
        #region Average
        /// <summary>
        /// 求平均值
        /// </summary>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Average<TResult>(string collectionName, string field, object condition = null) where TResult : struct {
            CommonException.CheckArgumentNull(collectionName, "collectionName");
            CommonException.CheckArgumentNull(field, "field");

            using (ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Average(field)
                       .Query(condition);
                using (var q = CreateQuery<TResult>(builder.CommandText, builder.Parameters)) {
                    return q.FirstOrDefault();
                }
            }
        }
        /// <summary>
        /// 求平均值
        /// </summary>
        /// <typeparam name="TEntity">任意类型</typeparam>
        /// <typeparam name="TResult">数字类型</typeparam>
        /// <param name="field">字段名称</param>
        /// <param name="condition">过滤条件</param>
        /// <returns></returns>
        public virtual TResult Average<TEntity, TResult>(string field, object condition = null) where TEntity : class where TResult : struct {
            return Average<TResult>(typeof(TEntity).Name, field, condition);
        }
        #endregion


        #region Insert
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert<TEntity>(TEntity model) where TEntity : class {
            return Insert(typeof(TEntity).Name, model, (InsertCommandBuilderFilter)null);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert<TEntity>(TEntity model, string[] removeFields) where TEntity : class {
            InsertCommandBuilderFilter builderFilter;
            if (removeFields == null || removeFields.Length == 0)
                builderFilter = null;
            else
                builderFilter = (builder) => {
                    builder.Fields.RemoveKeys(removeFields);
                };
            return Insert(typeof(TEntity).Name, model, builderFilter);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert<TEntity>(TEntity model, InsertCommandBuilderFilter builderFilter) where TEntity : class {
            return Insert(typeof(TEntity).Name, model, builderFilter);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert<TEntity>(object values) where TEntity : class {
            return Insert(typeof(TEntity).Name, values, (InsertCommandBuilderFilter)null);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert<TEntity>(object values, string[] removeFields) where TEntity : class {
            InsertCommandBuilderFilter builderFilter;
            if (removeFields == null || removeFields.Length == 0)
                builderFilter = null;
            else
                builderFilter = (builder) => {
                    builder.Fields.RemoveKeys(removeFields);
                };
            return Insert(typeof(TEntity).Name, values, builderFilter);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert<TEntity>(object values, InsertCommandBuilderFilter builderFilter) where TEntity : class {
            return Insert(typeof(TEntity).Name, values, builderFilter);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual TResult Insert<TEntity, TResult>(object values, string[] removeFields) where TEntity : class {
            return Insert<TResult>(typeof(TEntity).Name, values, removeFields);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual TResult Insert<TEntity, TResult>(object values, InsertCommandBuilderFilter builderFilter) where TEntity : class {
            return Insert<TResult>(typeof(TEntity).Name, values, builderFilter);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert(string collectionName, object values) {
            return Insert(collectionName, values, (InsertCommandBuilderFilter)null);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert(string collectionName, object values, string[] removeFields) {
            InsertCommandBuilderFilter builderFilter;
            if (removeFields == null || removeFields.Length == 0)
                builderFilter = null;
            else
                builderFilter = (builder) => {
                    builder.Fields.RemoveKeys(removeFields);
                };
            return Insert(collectionName, values, builderFilter);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual long Insert(string collectionName, object values, InsertCommandBuilderFilter builderFilter) {
            return Insert<long>(collectionName, values, builderFilter);
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual TResult Insert<TResult>(string collectionName, object values, string[] removeFields) {
            InsertCommandBuilderFilter builderFilter;
            if (removeFields == null || removeFields.Length == 0)
                builderFilter = null;
            else
                builderFilter = (builder) => {
                    builder.Fields.RemoveKeys(removeFields);
                };
            return Insert<TResult>(collectionName, values, builderFilter);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <returns>返回插入数据的id(取决于是否有自增主键）。</returns>
        public virtual TResult Insert<TResult>(string collectionName, object values, InsertCommandBuilderFilter builderFilter) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");
            CommonException.CheckArgumentNull(values, "values");

            using (IInsertCommandBuilder builder = CreateInsert(collectionName)) {
                builder.Fields.SetValues(values);
                builder.Fields.Remove("id");
                builderFilter?.Invoke(builder);

                if (builder.Fields.Count == 0)
                    return default(TResult);
                object result = ExecuteScalar(builder.CommandText, builder.Values);
                if (result == null)
                    return default(TResult);
                return TypeExtensions.Convert<TResult>(result);
            }
        }
        #endregion
        #region Update
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(TEntity model, object condition, CommandQueryFilterDelegate queryFilter = null) where TEntity : class {
            return Update(typeof(TEntity).Name, model, condition, queryFilter, (UpdateCommandBuilderFilter)null);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(TEntity model, object condition, string[] removeFields) where TEntity : class {
            UpdateCommandBuilderFilter builderFilter;
            if (removeFields == null || removeFields.Length == 0)
                builderFilter = null;
            else
                builderFilter = (builder) => {
                    builder.Fields.RemoveKeys(removeFields);
                };
            return Update(typeof(TEntity).Name, model, condition, null, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(TEntity model, object condition, UpdateCommandBuilderFilter builderFilter) where TEntity : class {
            return Update(typeof(TEntity).Name, model, condition, null, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="model">更新数据，实体</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(TEntity model, object condition, CommandQueryFilterDelegate queryFilter, UpdateCommandBuilderFilter builderFilter = null) where TEntity : class {
            return Update(typeof(TEntity).Name, model, condition, queryFilter, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(object values, object condition, CommandQueryFilterDelegate queryFilter = null) where TEntity : class {
            return Update(typeof(TEntity).Name, values, condition, queryFilter, (UpdateCommandBuilderFilter)null);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(object values, object condition, string[] removeFields) where TEntity : class {
            UpdateCommandBuilderFilter builderFilter;
            if (removeFields == null || removeFields.Length == 0)
                builderFilter = null;
            else
                builderFilter = (builder) => {
                    builder.Fields.RemoveKeys(removeFields);
                };
            return Update(typeof(TEntity).Name, values, condition, null, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(object values, object condition, UpdateCommandBuilderFilter builderFilter) where TEntity : class {
            return Update(typeof(TEntity).Name, values, condition, null, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update<TEntity>(object values, object condition, CommandQueryFilterDelegate queryFilter, UpdateCommandBuilderFilter builderFilter = null) where TEntity : class {
            return Update(typeof(TEntity).Name, values, condition, queryFilter, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update(string collectionName, object values, object condition, CommandQueryFilterDelegate queryFilter = null) {
            return Update(collectionName, values, condition, queryFilter, (UpdateCommandBuilderFilter)null);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update(string collectionName, object values, object condition, string[] removeFields) {
            UpdateCommandBuilderFilter builderFilter;
            if (removeFields == null || removeFields.Length == 0)
                builderFilter = null;
            else
                builderFilter = (builder) => {
                    builder.Fields.RemoveKeys(removeFields);
                };
            return Update(collectionName, values, condition, null, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update(string collectionName, object values, object condition, UpdateCommandBuilderFilter builderFilter) {
            return Update(collectionName, values, condition, null, builderFilter);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <returns>返回更新条数。</returns>
        public virtual int Update(string collectionName, object values, object condition, CommandQueryFilterDelegate queryFilter, UpdateCommandBuilderFilter builderFilter = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            if (values == null)
                return 0;
            using (Symbol.Data.IUpdateCommandBuilder builder = CreateUpdate(collectionName)) {
                builder.Fields.SetValues(values);
                builder.Fields.RemoveKeys("Id", "id");
                builderFilter?.Invoke(builder);
                if (builder.Fields.Count == 0)
                    return 0;

                return builder.QueryBlock((whereBuilder) => {
                    whereBuilder.Query(condition, queryFilter);
                    return whereBuilder.WhereCommandText.Length > 0;
                }, (p1, p2) => {
                    return ExecuteNonQuery(p1, p2);
                });
            }

        }
        #endregion
        #region Delete
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回删除条数。</returns>
        public virtual int Delete<TEntity>(object condition, CommandQueryFilterDelegate queryFilter = null) where TEntity : class {
            return Delete(typeof(TEntity).Name, condition, queryFilter);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <returns>返回删除条数。</returns>
        public virtual int Delete(string collectionName, object condition, CommandQueryFilterDelegate queryFilter = null) {
            CommonException.CheckArgumentNull(collectionName, "collectionName");

            using (Symbol.Data.ISelectCommandBuilder builder = CreateSelect(collectionName)) {
                builder.Select("1").Query(condition, queryFilter);
                if (builder.WhereCommandText.Length == 0)
                    return 0;
                return ExecuteNonQuery(builder.DeleteCommmandText, builder.Parameters);
            }
        }
        #endregion

        #region TryInsert
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert<TEntity>(object values, string[] removeFields, DataContextExecuteCallback<long> callback = null) where TEntity : class {
            try {
                long result = Insert<TEntity>(values, removeFields);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0L, error);
                return false;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert<TEntity>(object values, InsertCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<long> callback = null) where TEntity : class {
            try {
                long result = Insert<TEntity>(values, builderFilter);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0L, error);
                return false;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert<TEntity, TResult>(object values, string[] removeFields, DataContextExecuteCallback<TResult> callback = null) where TEntity : class {
            try {
                TResult result = Insert<TEntity, TResult>(values, removeFields);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, default(TResult), error);
                return false;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert<TEntity, TResult>(object values, InsertCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<TResult> callback = null) where TEntity : class {
            try {
                TResult result = Insert<TEntity, TResult>(values, builderFilter);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, default(TResult), error);
                return false;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert(string collectionName, object values, string[] removeFields, DataContextExecuteCallback<long> callback = null) {
            try {
                long result = Insert(collectionName, values, removeFields);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0L, error);
                return false;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert(string collectionName, object values, InsertCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<long> callback = null) {
            try {
                long result = Insert(collectionName, values, builderFilter);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0L, error);
                return false;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert<TResult>(string collectionName, object values, string[] removeFields, DataContextExecuteCallback<TResult> callback = null) {
            try {
                TResult result = Insert<TResult>(collectionName, values, removeFields);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, default(TResult), error);
                return false;
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TResult">自增主键类型</typeparam>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="builderFilter">构造器过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryInsert<TResult>(string collectionName, object values, InsertCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<TResult> callback = null) {
            try {
                TResult result = Insert<TResult>(collectionName, values, builderFilter);
                callback?.Invoke(this, result, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, default(TResult), error);
                return false;
            }
        }
        #endregion
        #region TryUpdate
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryUpdate<TEntity>(object values, object condition, string[] removeFields, DataContextExecuteCallback<int> callback = null) where TEntity : class {
            try {
                int count = Update<TEntity>(values, condition, removeFields);
                callback?.Invoke(this, count, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0, error);
                return false;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryUpdate<TEntity>(object values, object condition, CommandQueryFilterDelegate queryFilter = null, UpdateCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<int> callback = null) where TEntity : class {
            try {
                int count = Update<TEntity>(values, condition, queryFilter, builderFilter);
                callback?.Invoke(this, count, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0, error);
                return false;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="removeFields">需要排除的字段列表。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryUpdate(string collectionName, object values, object condition, string[] removeFields, DataContextExecuteCallback<int> callback = null) {
            try {
                int count = Update(collectionName, values, condition, removeFields);
                callback?.Invoke(this, count, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0, error);
                return false;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="values">更新数据，实体/匿名对象/键值对</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="builderFilter">构造器过滤器。</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryUpdate(string collectionName, object values, object condition, CommandQueryFilterDelegate queryFilter = null, UpdateCommandBuilderFilter builderFilter = null, DataContextExecuteCallback<int> callback = null) {
            try {
                int count = Update(collectionName, values, condition, queryFilter, builderFilter);
                callback?.Invoke(this, count, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0, error);
                return false;
            }
        }
        #endregion
        #region TryDelete
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryDelete<TEntity>(object condition, CommandQueryFilterDelegate queryFilter = null, DataContextExecuteCallback<int> callback = null) where TEntity : class {
            try {
                int count = Delete<TEntity>(condition, queryFilter);
                callback?.Invoke(this, count, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0, error);
                return false;
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="collectionName">集合名称（表名称）</param>
        /// <param name="condition">过滤规则（NoSQL）</param>
        /// <param name="queryFilter">查询过滤器</param>
        /// <param name="callback">回调</param>
        /// <returns>返是否有成功。</returns>
        public virtual bool TryDelete(string collectionName, object condition, CommandQueryFilterDelegate queryFilter = null, DataContextExecuteCallback<int> callback = null) {
            try {
                int count = Delete(collectionName, condition, queryFilter);
                callback?.Invoke(this, count, null);
                return true;
            } catch (System.Exception error) {
                callback?.Invoke(this, 0, error);
                return false;
            }

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
        public abstract ISelectCommandBuilder CreateSelect(string tableName);
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="commandText">查询命令。</param>
        /// <returns>返回构造器对象。</returns>
        public abstract ISelectCommandBuilder CreateSelect(string tableName, string commandText);
        /// <summary>
        /// 创建查询命令构造器。
        /// </summary>
        /// <typeparam name="T">任意可引用类型。</typeparam>
        /// <returns>返回构造器对象。</returns>
        public virtual ISelectCommandBuilder CreateSelect<T>() where T : class {
            return CreateSelect(typeof(T).Name);
        }
        #endregion
        #region CreateInsert
        /// <summary>
        /// 创建插入命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public abstract IInsertCommandBuilder CreateInsert(string tableName);
        /// <summary>
        /// 创建插入命令构造器(自动填充数据)。
        /// </summary>
        /// <param name="model">实体对象,不能为null。</param>
        /// <exception cref="System.NullReferenceException">model不能为null。</exception>
        /// <returns>返回构造器对象。</returns>
        public virtual IInsertCommandBuilder CreateInsert(object model) {
            if (model == null) {
                throw new System.NullReferenceException("model不能为null。");
            }
            IInsertCommandBuilder builder = CreateInsert(model.GetType().Name);
            builder.Fields.SetValues(model);
            builder.Fields.Remove("Id");
            builder.Fields.Remove("id");
            return builder;
        }

        #endregion
        #region CreateUpdate
        /// <summary>
        /// 创建更新命令构造器。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <returns>返回构造器对象。</returns>
        public abstract IUpdateCommandBuilder CreateUpdate(string tableName);
        /// <summary>
        /// 创建更新命令构造器(自动填充数据)。
        /// </summary>
        /// <param name="model">实体对象,不能为null。</param>
        /// <exception cref="System.NullReferenceException">model不能为null。</exception>
        /// <returns>返回构造器对象。</returns>
        public virtual IUpdateCommandBuilder CreateUpdate(object model) {
            if (model == null) {
                throw new System.NullReferenceException("model不能为null。");
            }
            IUpdateCommandBuilder builder = CreateUpdate(model.GetType().Name);
            builder.Fields.SetValues(model);
            builder.Fields.Remove("Id");
            builder.Fields.Remove("id");
            return builder;
        }
        #endregion

        #endregion

        #region Schema
        /// <summary>
        /// 获取表空间的位置
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <returns></returns>
        public virtual string GetTableSpacePath(string name) {
            CommonException.ThrowNotSupported();
            return null;
        }
        /// <summary>
        /// 获取默认表空间目录。
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTableSpaceDirectory() {
            try {
                string dir = GetTableSpacePath(null);
                if (string.IsNullOrEmpty(dir))
                    return null;
                return System.IO.Path.GetDirectoryName(dir);
            } catch {
                return "";
            }
        }
        /// <summary>
        /// 判断表空间是否存在。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool TableSpaceExists(string name) {
            CommonException.ThrowNotSupported();
            return false;
        }
        /// <summary>
        /// 创建表空间。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        /// <param name="path">路径，为空将自动处理（默认与当前数据库同目录）。</param>
        public virtual bool TableSpaceCreate(string name, string path = null) {
            CommonException.ThrowNotSupported();
            return false;
        }
        /// <summary>
        /// 删除表空间。
        /// </summary>
        /// <param name="name">名称，不带[]等符号。</param>
        public virtual void TableSpaceDelete(string name) {
            CommonException.ThrowNotSupported();
        }

        /// <summary>
        /// 创建表（仅用于简单的逻辑，复杂的创建语句请直接调用ExecuteNonQuery）。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columns">列，每一个列请自行拼接好属性。</param>
        public abstract void TableCreate(string tableName, params string[] columns);
        /// <summary>
        /// 判断表是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool TableExists(string tableName) {
            return TableExists(tableName, null);
        }
        /// <summary>
        /// 判断表是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        public abstract bool TableExists(string tableName, string schemaName = "@default");

        /// <summary>
        /// 删除表。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        public virtual void TableDelete(string tableName) {
            if (string.IsNullOrEmpty(tableName))
                return;
            ExecuteNonQuery(string.Format("drop table {0}", Provider?.Dialect?.PreName(tableName)));
        }

        /// <summary>
        /// 判断列（字段）是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <returns></returns>
        public virtual bool ColumnExists(string tableName, string columnName) {
            return ColumnExists(tableName, columnName, null);
        }
        /// <summary>
        /// 判断列（字段）是否存在。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns></returns>
        public abstract bool ColumnExists(string tableName, string columnName, string schemaName = "@default");
        /// <summary>
        /// 获取数据库中列（字段）的信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <returns>永远不会返回null。</returns>
        public virtual DatabaseTableField GetColumnInfo(string tableName, string columnName) {
            return GetColumnInfo(tableName, columnName, null);
        }
        /// <summary>
        /// 获取数据库中列（字段）的信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="columnName">列（字段）名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>永远不会返回null。</returns>
        public abstract DatabaseTableField GetColumnInfo(string tableName, string columnName, string schemaName = "@default");
        /// <summary>
        /// 获取数据库中表的所有列（字段）信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <returns></returns>
        public virtual System.Collections.Generic.List<DatabaseTableField> GetColumns(string tableName) {
            return GetColumns(tableName, null);
        }
        /// <summary>
        /// 获取数据库中表的所有列（字段）信息。
        /// </summary>
        /// <param name="tableName">表名，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns></returns>
        public abstract System.Collections.Generic.List<DatabaseTableField> GetColumns(string tableName, string schemaName = "@default");

        /// <summary>
        /// 判断函数是否存在。
        /// </summary>
        /// <param name="functionName">函数名称，不带[]等符号。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool FunctionExists(string functionName) {
            return FunctionExists(functionName, null);
        }
        /// <summary>
        /// 判断函数是否存在。
        /// </summary>
        /// <param name="functionName">函数名称，不带[]等符号。</param>
        /// <param name="schemaName">架构名称，@default由实现者解析为默认值（dbo或数据库名称）</param>
        /// <returns>返回判断结果。</returns>
        public abstract bool FunctionExists(string functionName, string schemaName = "@default");


        /// <summary>
        /// 创建外键关系。
        /// </summary>
        /// <param name="primaryKeyTableName">主键表名。</param>
        /// <param name="primaryKey">主键列名。</param>
        /// <param name="foreignKeyTableName">外键表名。</param>
        /// <param name="foreignKey">外键列名。</param>
        /// <param name="cascadeDelete">级联删除。</param>
        /// <param name="cascadeUpdate">级联更新。</param>
        public abstract void ForeignKeyCreate(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey, bool cascadeDelete = false, bool cascadeUpdate = false);
        /// <summary>
        /// 删除外键关系。
        /// </summary>
        /// <param name="primaryKeyTableName">主键表名。</param>
        /// <param name="primaryKey">主键列名。</param>
        /// <param name="foreignKeyTableName">外键表名。</param>
        /// <param name="foreignKey">外键列名。</param>
        public abstract void ForeignKeyDelete(string primaryKeyTableName, string primaryKey, string foreignKeyTableName, string foreignKey);

        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 释放DataContext所有的资源，包括数据库连接对象。
        /// </summary>
        public virtual void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放DataContext所有的资源，包括数据库连接对象。
        /// </summary>
        /// <param name="disposing">为true时才是真正的时机。</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                ThreadHelper.InterlockedSet(ref _connections, null);
                ThreadHelper.InterlockedSet(ref _dataBinderObjectCache, null);
                ThreadHelper.InterlockedSet(ref _disposableObjectList, null)?.Dispose();
            }
        }
        #endregion

        #endregion

    }

}
