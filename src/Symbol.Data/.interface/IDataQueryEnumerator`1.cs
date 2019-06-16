/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 数据查询迭代器接口。
    /// </summary>
    public interface IDataQueryEnumerator<T>
        : System.IDisposable 
        , System.Collections.Generic.IEnumerator<T> {


        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        IDataContext DataContext { get; }
        /// <summary>
        /// 获取数据查询。
        /// </summary>
        IDataQuery<T> Query { get; }
        /// <summary>
        /// 获取数据查询器回调委托。
        /// </summary>
        DataQueryCallback<T> Callback { get; }

        /// <summary>
        /// 获取查询读取器。
        /// </summary>
        IDataQueryReader Reader { get; }

        /// <summary>
        /// 获取数据绑定缓存对象。
        /// </summary>
        Binding.IDataBinderObjectCache DataBinderObjectCache { get; }

    }
}