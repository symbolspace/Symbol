/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Data;

namespace Symbol.Data {




    /// <summary>
    /// 数据查询迭代器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataQueryEnumerator<T> : System.Collections.Generic.IEnumerator<T> {

        #region fields
        private IDataContext _dataContext;
        private IDataReader _reader;
        private System.Type _type;
        private T _current = default(T);
        #endregion

        #region properties
        object System.Collections.IEnumerator.Current { get { return _current; } }
        /// <summary>
        /// 获取当前对象。
        /// </summary>
        public T Current { get { return _current; } }
        /// <summary>
        /// 获取或设置回调。
        /// </summary>
        public DataQueryCallback<T> Callback { get; set; }
        /// <summary>
        /// 获取或设置数据绑定缓存对象。
        /// </summary>
        public Binding.IDataBinderObjectCache DataBinderObjectCache { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建DataQueryEnumerator实例。
        /// </summary>
        /// <param name="dataContext">数据上下文。</param>
        /// <param name="reader"></param>
        /// <param name="type">类型。</param>
        public DataQueryEnumerator(IDataContext dataContext, IDataReader reader, System.Type type) {
            _dataContext = dataContext;
            dataContext.DisposableObjects?.Add(this);
            _reader = reader;
            _type = type;
        }
        #endregion

        #region methods

        #region MoveNext
        /// <summary>
        /// 移动到下一条。
        /// </summary>
        /// <returns></returns>
        public bool MoveNext() {
            bool result = _reader.Read();
            if (!result) {
                _current = default(T);
            } else {
                _current = (T)DataReaderHelper.Current(_reader, _type);
                Symbol.Data.Binding.DataBinderAttribute.Bind(_dataContext, _reader, _current, _type, DataBinderObjectCache);
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
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_reader != null) {
                    _reader.Close();
                    _reader.Dispose();
                    _reader = null;
                }
                _dataContext = null;
                _type = null;
            }
        }
        #endregion

        #endregion
    }

}