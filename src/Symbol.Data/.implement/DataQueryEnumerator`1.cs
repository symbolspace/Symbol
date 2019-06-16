/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据查询迭代器基类。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    public class DataQueryEnumerator<T> 
        : IDataQueryEnumerator<T> {

        #region fields

        private IDataQueryReader _reader;
        private IDataQuery<T> _query;
        private Binding.IDataBinderObjectCache _dataBinderObjectCache;
        private T _current = default(T);
        private System.Type _type;

        #endregion

        #region properties

        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        public IDataContext DataContext { get { return Query?.DataContext; } }

        /// <summary>
        /// 获取回调。
        /// </summary>
        public IDataQuery<T> Query { get { return ThreadHelper.InterlockedGet(ref _query); } }
        /// <summary>
        /// 获取数据查询器回调委托。
        /// </summary>
        public DataQueryCallback<T> Callback { get { return Query?.Callback; } }

        /// <summary>
        /// 获取查询读取器。
        /// </summary>
        public IDataQueryReader Reader { get { return ThreadHelper.InterlockedGet(ref _reader); } }

        /// <summary>
        /// 获取数据绑定缓存对象。
        /// </summary>
        public Binding.IDataBinderObjectCache DataBinderObjectCache { get { return ThreadHelper.InterlockedGet(ref _dataBinderObjectCache); } }

        object System.Collections.IEnumerator.Current { get { return _current; } }
        /// <summary>
        /// 获取当前对象。
        /// </summary>
        public T Current { get { return _current; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="query">数据查询。</param>
        /// <param name="reader">数据查询读取器。</param>
        /// <param name="type">类型。</param>
        public DataQueryEnumerator(IDataQuery<T> query, IDataQueryReader reader, System.Type type) {
            _query = query;
            _reader = reader;
            _dataBinderObjectCache = query?.DataBinderObjectCache;
            _type = type;
            query?.DataContext.DisposableObjects.Add(this);
        }
        #endregion

        #region methods

        #region MoveNext
        /// <summary>
        /// 移动到下一条。
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveNext() {
            bool result = _reader.Read();
            if (!result) {
                _current = default(T);
            } else {
                _current = (T)_reader.ToObject(_type);
                Symbol.Data.Binding.DataBinderAttribute.Bind(DataContext, _reader, _current, _type, DataBinderObjectCache);
                Callback?.Invoke(Current, _reader);
            }
            return result;
        }
        #endregion
        #region Reset
        void System.Collections.IEnumerator.Reset() {
            throw new System.NotImplementedException();
        }
        #endregion
        #region Dispose
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public virtual void Dispose() {
            ThreadHelper.InterlockedSet(ref _type, null);
            ThreadHelper.InterlockedSet(ref _reader, null)?.Dispose();
            ThreadHelper.InterlockedSet(ref _query, null);
            ThreadHelper.InterlockedSet(ref _dataBinderObjectCache, null);
            _current = default(T);
        }
        #endregion

        #endregion
    }

}