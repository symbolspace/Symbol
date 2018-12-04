/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据查询接口（非泛型和调用时不方便使用泛型）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    public class DataQuery<T> : IDataQuery<T> {

        #region fields
        /// <summary>
        /// 类型：object
        /// </summary>
        protected static readonly System.Type _objectType = typeof(object);
        /// <summary>
        /// 类型：Symbol.Collections.Generic.NameValueCollection&lt;object&gt;
        /// </summary>
        protected static readonly System.Type _nameValueCollectionType = typeof(Symbol.Collections.Generic.NameValueCollection<object>);
        /// <summary>
        /// 当前数据上下文对象。
        /// </summary>
        protected IDataContext _dataContext;
        /// <summary>
        /// 当前命令对象。
        /// </summary>
        protected IDbCommand _command;
        /// <summary>
        /// 当前实体类型，为null表示未指定。
        /// </summary>
        protected System.Type _type;
        private string _commandText;
        private System.Collections.Generic.List<IDbCommand> _commands;
        private bool _firstExecute=true;
        private Binding.IDataBinderObjectCache _dataBinderObjectCache;
        private bool _dataBinderObjectCacheMyCreated;

        #endregion

        #region properties

        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        public IDataContext DataContext { get { return _dataContext; } }

        /// <summary>
        /// 获取当前实体的类型。
        /// </summary>
        public System.Type Type {
            get { return _type == null ? _nameValueCollectionType : _type; }
        }

        /// <summary>
        /// 命令文本之前的内容，有时CommandText可能会被修改，但有一部分可能会影响修改过程，可以提取出来设置到此属性上。
        /// </summary>
        /// <remarks>在每次执行地，如果此属性有值，将会放在CommandText之前。</remarks>
        public string CommandTextBefore { get; set; }

        /// <summary>
        /// 获取或设置当前查询命令语句。
        /// </summary>
        public string CommandText {
            get { return _commandText; }
            set { _commandText = value; _orignalCommandText = null; }
        }
        /// <summary>
        /// 获取或设置当前查询超时时间（秒，不会影响到DataContext）。
        /// </summary>
        public int CommandTimeout {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }
        /// <summary>
        /// 获取或设置当前查询的类型。
        /// </summary>
        public CommandType CommandType {
            get { return _command.CommandType; }
            set { _command.CommandType = value; }
        }
        /// <summary>
        /// 获取或设置当前查询的连接。
        /// </summary>
        public IDbConnection Connection {
            get { return _command.Connection; }
            set { _command.Connection = value; }
        }
        /// <summary>
        /// 获取或设置当前查询的事务对象。
        /// </summary>
        public IDbTransaction Transaction {
            get { return _command.Transaction; }
            set { _command.Transaction = value; }
        }
        /// <summary>
        /// 获取当前查询命令对象。
        /// </summary>
        public System.Data.IDbCommand Command { get { return _command; } }
        /// <summary>
        /// 获取当前查询的参数列表。
        /// </summary>
        public IDataParameterCollection Parameters {
            get { return _command.Parameters; }
        }
        /// <summary>
        /// 获取或设置当前查询的更新行选项。
        /// </summary>
        public UpdateRowSource UpdatedRowSource {
            get { return _command.UpdatedRowSource; }
            set { _command.UpdatedRowSource = value; }
        }
        /// <summary>
        /// 获取或设置数据查询器回调委托。
        /// </summary>
        public DataQueryCallback<T> Callback { get; set; }
        /// <summary>
        /// 获取或设置数据绑定缓存对象。
        /// </summary>
        public Binding.IDataBinderObjectCache DataBinderObjectCache {
            get { return _dataBinderObjectCache; }
            set {
                if (value != _dataBinderObjectCache) {
                    DisposeDataBinderObjectCache();   
                }
                _dataBinderObjectCache = value;
            }
        }

        #endregion
        #region ctor
        /// <summary>
        /// 创建DataQuery实例。
        /// </summary>
        /// <param name="dataContext">数据上下文接口。</param>
        /// <param name="command">命令对象。</param>
        /// <param name="type">类型。</param>
        public DataQuery(IDataContext dataContext, IDbCommand command, System.Type type = null) {
            _dataContext = dataContext;
            _command = command;
            _commandText = command.CommandText;
            if (type == null)
                _type = typeof(T);
            else
                _type = type;

            if (_type == _objectType || _type == _nameValueCollectionType)
                _type = null;
            AllowPageOutOfMax = true;
            _commands = new System.Collections.Generic.List<IDbCommand>();
        }
        #endregion

        #region methods

        #region ExecuteReader
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns>返回数据读取对象。</returns>
        protected virtual IDataReader ExecuteReader() {
            IDataReader reader;
            string commandText;
            if (!string.IsNullOrEmpty(CommandTextBefore))
                commandText = CommandTextBefore + "\r\n" + _commandText;
            else
                commandText = _commandText;

            if (_firstExecute || _dataContext.Provider.MultipleActiveResultSets) {
                _command.CommandText = commandText;
                reader = _command.ExecuteReader();
                _firstExecute = false;
            } else {
                //var connection = (IDbConnection)FastWrapper.MethodInvoke(_command.Connection.GetType(), "Clone", _command.Connection, new object[0]);
                //var connection = (IDbConnection)FastWrapper.CreateInstance(_command.Connection.GetType(), _dataContext.ConnectionString);
                var connection = _dataContext.PopConnection();

                var command = _dataContext.CreateCommand(connection, commandText, (p) => {
                    foreach (IDbDataParameter param in Parameters) {
                        IDbDataParameter param2 = CreateParameter();
                        param2.DbType = param.DbType;
                        param2.Direction = param.Direction;
                        param2.ParameterName = param.ParameterName;
                        param2.Precision = param.Precision;
                        param2.Scale = param.Scale;
                        param2.Size = param.Size;
                        param2.SourceColumn = param.SourceColumn;
                        param2.SourceVersion = param.SourceVersion;
                        param2.Value = param.Value;
                        p.Parameters.Add(param2);
                    }
                });
                _commands.Add(command);
                reader = command.ExecuteReader();
            }
            return reader;
        }
        #endregion

        /// <summary>
        /// 生成分页语法。
        /// </summary>
        /// <param name="size">每页大小，忽略小于1。</param>
        /// <param name="page">页码，从0开始，忽略小于0。</param>
        /// <returns></returns>
        public virtual void Paging(int size, int page) {
            if (size > 0 && page > -1) {
                //TakeCount = size;
                //SkipCount = size * page;
                _currentPageIndex = page;
                _itemPerPage = size;
                ChangePageSetting();
            }
        }

        #region Count
        /// <summary>
        /// 求出当前查询的数据记录数。
        /// </summary>
        /// <returns>返回当前查询的数据记录数。</returns>
        public virtual int Count(){
            if (string.IsNullOrEmpty(_commandText))
                return 0;
            string commandText = string.IsNullOrEmpty(_orignalCommandText) ? _commandText : _orignalCommandText;
            commandText = _dataContext.CreateSelect(_emptyTableName, commandText)
                                   .Count()
                                   .CommandText;
            if (!string.IsNullOrEmpty(CommandTextBefore))
                commandText = CommandTextBefore + "\r\n" + commandText;
            return Count(commandText);
        }
        /// <summary>
        /// 求出当前查询的数据记录数。
        /// </summary>
        /// <param name="commandText">指定查询方式。</param>
        /// <returns>返回当前查询的数据记录数。</returns>
        public virtual int Count(string commandText) {
            return TypeExtensions.Convert<int>(_dataContext.ExecuteScalar(commandText, (command) => {
                foreach (IDbDataParameter param in Parameters) {
                    IDbDataParameter param2 = CreateParameter();
                    param2.DbType = param.DbType;
                    param2.Direction = param.Direction;
                    param2.ParameterName = param.ParameterName;
                    param2.Precision = param.Precision;
                    param2.Scale = param.Scale;
                    param2.Size = param.Size;
                    param2.SourceColumn = param.SourceColumn;
                    param2.SourceVersion = param.SourceVersion;
                    param2.Value = param.Value;
                    command.Parameters.Add(param2);
                }

            }), 0);
        }
        #endregion
        #region FirstOrDefault
        //object IDataQuery.FirstOrDefault() {
        //    return FirstOrDefault();
        //}
        /// <summary>
        /// 从查询中拿出第一条记录，如果查询未包含任何记录，将返回此类型的default(T);
        /// </summary>
        /// <returns>返回第一条记录。</returns>
        public virtual T FirstOrDefault() {
            using (var enumerator = GetEnumerator()) {
                if (enumerator.MoveNext()) {
                    return enumerator.Current;
                }
            }
            return default(T);
        }
        #endregion
        #region ToList
        //System.Collections.Generic.List<object> IDataQuery.ToList() {
        //    System.Collections.Generic.List<object> result = new System.Collections.Generic.List<object>();
        //    using (var enumerator = GetEnumerator()) {
        //        while (enumerator.MoveNext()) {
        //            result.Add(enumerator.Current);
        //        }
        //    }
        //    return result;
        //}
        /// <summary>
        /// 将查询快速读取并构造一个List对象。
        /// </summary>
        /// <returns>返回一个List对象。</returns>
        public virtual System.Collections.Generic.List<T> ToList() {
            System.Collections.Generic.List<T> result = new System.Collections.Generic.List<T>();
            using (var enumerator = GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    result.Add(enumerator.Current);
                }
            }
            return result;
        }
        #endregion

        #region NextParamName
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <returns>返回下一个参数的名称。</returns>
        public string NextParamName() {
            return IDbCommandExtensions.NextParamName(_command);
        }
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <param name="offset">偏移多少个参数，可能用于预留。</param>
        /// <returns>返回下一个参数的名称。</returns>
        public string NextParamName(int offset) {
            return IDbCommandExtensions.NextParamName(_command, offset);
        }
        #endregion
        #region CreateParameter
        /// <summary>
        /// 创建查询参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <returns>返回参数实例。</returns>
        public virtual IDbDataParameter CreateParameter() {
            return _command.CreateParameter();
        }
        /// <summary>
        /// 创建查询参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="parameterName">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public virtual IDbDataParameter CreateParameter(string parameterName, object value) {
            return IDbCommandExtensions.CreateParameter(_command, parameterName, _dataContext.PreParameter(parameterName, value));
        }
        #endregion
        #region AddParameter
        /// <summary>
        /// 添加一个查询参数（无值）。
        /// </summary>
        /// <returns>返回参数实例。</returns>
        public virtual IDbDataParameter AddParameter() {
            return IDbCommandExtensions.AddParameter(_command);
        }
        /// <summary>
        /// 添加一个查询参数（自动命名@p1 @p2）。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public virtual IDbDataParameter AddParameter(object value) {
            string pName = IDbCommandExtensions.NextParamName(_command);
            return IDbCommandExtensions.AddParameter(_command, pName, _dataContext.PreParameter(pName, value));
        }
        /// <summary>
        /// 添加一个查询参数。
        /// </summary>
        /// <param name="parameterName">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public virtual IDbDataParameter AddParameter(string parameterName, object value) {
            return IDbCommandExtensions.AddParameter(_command, parameterName, _dataContext.PreParameter(parameterName, value));
        }
        #endregion

        #region CreateBuilder
        /// <summary>
        /// 创建查询命令构造器（自动关联参数）。
        /// </summary>
        /// <returns>返回构造器对象。</returns>
        public virtual ISelectCommandBuilder CreateBuilder() {
            ISelectCommandBuilder builder = _dataContext.CreateSelect("table", CommandText);
            builder.AddCommandParameter = AddCommandParameter_ISelectCommandBuilder;
            builder.AutoEnd = true;
            System.EventHandler endedHandler = null; endedHandler = (p1, p2) => {
                builder.Ended -= endedHandler;
                CommandText = builder.CommandText;
            };
            builder.Ended += endedHandler;
            return builder;
        }
        string AddCommandParameter_ISelectCommandBuilder(object value) {
            return AddParameter(value).ParameterName;
        }
        #endregion

        #region GetEnumerator
        /// <summary>
        /// 获取迭代器。
        /// </summary>
        /// <returns></returns>
        public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator() {
            if (_dataBinderObjectCache == null) {
                _dataBinderObjectCache = new Binding.DataBinderObjectCache();
                _dataBinderObjectCacheMyCreated = true;
            }
            return new DataQueryEnumerator<T>(_dataContext, ExecuteReader(), Type) { Callback = Callback, DataBinderObjectCache = _dataBinderObjectCache };
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region IPagingCollection<T> 成员
        /// <summary>
        /// 是否允许页码超出最大值（默认允许）
        /// </summary>
        public bool AllowPageOutOfMax { get; set; }
        private int _currentPageIndex;
        /// <summary>
        /// 当前页码(0开始）
        /// </summary>
        public int CurrentPageIndex {
            get {
                return _currentPageIndex;
            }
            set {
                if (_currentPageIndex == value)
                    return;
                int pageCount = PageCount;
                if (value >= pageCount && AllowPageOutOfMax)
                    value = pageCount - 1;
                if (value < 0)
                    value = 0;
                _currentPageIndex = value; ChangePageSetting();
            }
        }

        private int _itemPerPage;
        /// <summary>
        /// 每页行数
        /// </summary>
        public int ItemPerPage {
            get {
                return _itemPerPage;
            }
            set {
                if (_itemPerPage == value)
                    return;
                if (value <= 0)
                    throw new System.ArgumentOutOfRangeException("ItemPerPage", "必须大于0");
                _itemPerPage = value;
                ChangePageSetting();
            }
        }

        private int _totalCount = -1;
        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalCount {
            get {
                if (_totalCount == -1) {
                    _totalCount = Count();
                }
                return _totalCount;
            }
        }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount {
            get {
                if (_itemPerPage == 0)
                    return 0;
                int count = TotalCount;
                if (count % ItemPerPage == 0) {
                    return count / ItemPerPage;
                } else {
                    return count / ItemPerPage + 1;
                }
            }
        }
        private static readonly string _emptyTableName = "table";
        /// <summary>
        /// 原始命令文本。
        /// </summary>
        protected string _orignalCommandText;
        /// <summary>
        /// 变更翻页设置
        /// </summary>
        protected virtual void ChangePageSetting() {
            if (_currentPageIndex == -1 || _itemPerPage == -1) {
                if (!string.IsNullOrEmpty(_orignalCommandText))
                    _commandText = _orignalCommandText;
                return;
            }
            if (string.IsNullOrEmpty(_orignalCommandText))
                _orignalCommandText = _commandText;
            _commandText = _dataContext.CreateSelect(_emptyTableName, _orignalCommandText)
                                               .Skip(CurrentPageIndex * ItemPerPage)
                                               .Take(ItemPerPage)
                                               .CommandText;
            _totalCount = -1;
        }
        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_command != null) {
                    _command.Dispose();
                    _command = null;
                }
                if (_commands != null) {
                    for (int i = 0; i < _commands.Count; i++) {
                        var connection = _commands[i].Connection;
                        _commands[i].Connection = null;
                        _dataContext.PushConnection(connection);
                        _commands[i].Dispose();
                    }
                    _commands.Clear();
                    _commands = null;
                }
                DisposeDataBinderObjectCache();
                _commandText = null;
                _orignalCommandText = null;
                _dataContext = null;
                _type = null;
            }
        }
        void DisposeDataBinderObjectCache() {
            if (_dataBinderObjectCacheMyCreated) {
                _dataBinderObjectCache?.Dispose();
                _dataBinderObjectCacheMyCreated = false;
            }
            _dataBinderObjectCache = null;
        }
        #endregion

        #endregion
    }


}