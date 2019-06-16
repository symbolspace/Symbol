/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据查询基类。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    public abstract class DataQuery<T> : IDataQuery<T> {

        #region fields
        /// <summary>
        /// 当前数据上下文对象。
        /// </summary>
        private IDataContext _dataContext;
        /// <summary>
        /// 当前命令对象。
        /// </summary>
        private ICommand _command;
        /// <summary>
        /// 当前实体类型，为null表示未指定。
        /// </summary>
        private System.Type _type;
        private Binding.IDataBinderObjectCache _dataBinderObjectCache;
        #endregion

        #region properties

        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        public IDataContext DataContext { get { return ThreadHelper.InterlockedGet(ref _dataContext); } }
        /// <summary>
        /// 获取或设置数据绑定缓存对象。
        /// </summary>
        public Binding.IDataBinderObjectCache DataBinderObjectCache {
            get { return ThreadHelper.InterlockedGet(ref _dataBinderObjectCache); }
            set {
                ThreadHelper.InterlockedSet(ref _dataBinderObjectCache, value);
                if (value != null)
                    DataContext?.DisposableObjects.Add(value);
            }
        }

        /// <summary>
        /// 获取当前实体的类型。
        /// </summary>
        public System.Type Type {
            get { return ThreadHelper.InterlockedGet(ref _type); }
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
            get { return Command?.Text; }
            set {
                var command = Command;
                if (command != null)
                    command.Text = value;
                OrignalCommandText = null;
            }
        }
        /// <summary>
        /// 原始命令文本。
        /// </summary>
        protected string OrignalCommandText {
            get { return ThreadHelper.InterlockedGet(ref _orignalCommandText); }
            set { ThreadHelper.InterlockedSet(ref _orignalCommandText, value); }
        }
        /// <summary>
        /// 获取或设置当前查询超时时间（秒，不会影响到DataContext）。
        /// </summary>
        public int CommandTimeout {
            get { return Command?.Timeout ?? 0; }
            set {
                var command = Command;
                if (command != null)
                    command.Timeout = value;
            }
        }

        /// <summary>
        /// 获取当前查询命令对象。
        /// </summary>
        public ICommand Command { get { return ThreadHelper.InterlockedGet(ref _command); } }

        /// <summary>
        /// 获取或设置数据查询器回调委托。
        /// </summary>
        public DataQueryCallback<T> Callback { get; set; }

        #endregion


        #region ctor
        /// <summary>
        /// 创建DataQuery实例。
        /// </summary>
        /// <param name="dataContext">数据上下文接口。</param>
        /// <param name="command">命令对象。</param>
        /// <param name="type">类型。</param>
        public DataQuery(IDataContext dataContext, ICommand command, System.Type type) {
            _dataContext = dataContext;
            _command = command;

            _type = type ?? typeof(T);
            if (_type == typeof(object) || _type == typeof(Symbol.Collections.Generic.NameValueCollection<object>))
                _type = typeof(object);

            AllowPageOutOfMax = true;
        }
        #endregion

        #region methods

        #region CreateBuilder
        /// <summary>
        /// 创建查询命令构造器（自动关联参数）。
        /// </summary>
        /// <returns>返回构造器对象。</returns>
        public virtual ISelectCommandBuilder CreateBuilder() {
            ISelectCommandBuilder builder = DataContext?.CreateSelect("table", CommandText);
            if (builder == null)
                return null;
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
            return Command?.Parameters?.Add(value).Name;
        }
        #endregion

        #region Count
        /// <summary>
        /// 求出当前查询的数据记录数。
        /// </summary>
        /// <returns>返回当前查询的数据记录数。</returns>
        public virtual int Count() {
            string commandText = CommandText;
            if (string.IsNullOrEmpty(commandText))
                return 0;
            string orignalCommandText = OrignalCommandText;
            if (!string.IsNullOrEmpty(orignalCommandText))
                commandText = orignalCommandText;

            var builder = DataContext?.CreateSelect(_emptyTableName, commandText);
            if (builder != null) {
                using (builder) {
                    builder.Count();
                    commandText = builder.CommandText;
                }
            }
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
            return Command?.ExecuteScalar<int?>(commandText) ?? 0;
        }
        #endregion

        #region Paging
        /// <summary>
        /// 生成分页语法。
        /// </summary>
        /// <param name="size">每页大小，忽略小于1。</param>
        /// <param name="page">页码，从0开始，忽略小于0。</param>
        /// <returns></returns>
        public virtual IDataQuery<T> Paging(int size, int page) {
            if (size > 0 && page > -1) {
                //TakeCount = size;
                //SkipCount = size * page;
                _currentPageIndex = page;
                _itemPerPage = size;
                ChangePageSetting();
            }
            return this;
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

        #region GetEnumerator
        /// <summary>
        /// 获取迭代器。
        /// </summary>
        /// <returns></returns>
        public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator() {
            {
                var dataBinderObjectCache = DataBinderObjectCache;
                if (dataBinderObjectCache == null) {
                    dataBinderObjectCache = new Binding.DataBinderObjectCache();
                    DataBinderObjectCache = dataBinderObjectCache;
                }
            }

            string commandText = CommandText;
            if (string.IsNullOrEmpty(commandText))
                return null;
            string orignalCommandText = OrignalCommandText;
            if (!string.IsNullOrEmpty(orignalCommandText))
                commandText = orignalCommandText;

            if (!string.IsNullOrEmpty(CommandTextBefore))
                commandText = CommandTextBefore + "\r\n" + commandText;
            var reader = Command?.ExecuteReader(commandText);
            return CreateEnumerator(reader);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// 创建数据查询迭代器。
        /// </summary>
        /// <param name="reader">数据查询读取器。</param>
        /// <returns>返回数据查询迭代器。</returns>
        protected virtual IDataQueryEnumerator<T> CreateEnumerator(IDataQueryReader reader) {
            return new DataQueryEnumerator<T>(this, reader, Type);
        }

        #endregion

        #region IPagingCollection<T> 成员
        private int _currentPageIndex;
        private int _itemPerPage;
        private int _totalCount = -1;
        /// <summary>
        /// 是否允许页码超出最大值（默认允许）
        /// </summary>
        public bool AllowPageOutOfMax { get; set; }
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
            var command = Command;
            if (command == null)
                return;
            string commandText = command.Text;
            string orignalCommandText = OrignalCommandText;

            if (_currentPageIndex == -1 || _itemPerPage == -1) {
                if (!string.IsNullOrEmpty(orignalCommandText))
                    command.Text = orignalCommandText;
                return;
            }
            if (string.IsNullOrEmpty(orignalCommandText))
                OrignalCommandText = commandText;
            command.Text = DataContext?.CreateSelect(_emptyTableName, orignalCommandText)
                                               ?.Skip(CurrentPageIndex * ItemPerPage)
                                               ?.Take(ItemPerPage)
                                               ?.CommandText;
            _totalCount = -1;
        }
        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public void Dispose() {

            var dataContext = ThreadHelper.InterlockedSet(ref _dataContext, null);
            ThreadHelper.InterlockedSet(ref _type, null);
            var original = ThreadHelper.InterlockedSet(ref _dataBinderObjectCache, null);
            if (dataContext?.DataBinderObjectCache == null || original != dataContext?.DataBinderObjectCache)
                original?.Dispose();

            ThreadHelper.InterlockedSet(ref _command, null)?.Dispose();
            OrignalCommandText = null;
        }

        #endregion

        #endregion
    }


}