/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// 数据查询接口（泛型）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    public interface IDataQuery<T> :
        IDataQuery,
        Collections.Generic.IPagingCollection<T>
        {

        /// <summary>
        /// 获取或设置数据查询器回调委托。
        /// </summary>
        DataQueryCallback<T> Callback { get; set; }

        /// <summary>
        /// 从查询中拿出第一条记录，如果查询未包含任何记录，将返回此类型的default(T);
        /// </summary>
        /// <returns>返回第一条记录。</returns>
        T FirstOrDefault();
        /// <summary>
        /// 将查询快速读取并构造一个List对象。
        /// </summary>
        /// <returns>返回一个List对象。</returns>
        System.Collections.Generic.List<T> ToList();

    }
    /// <summary>
    /// 数据查询器回调委托。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    /// <param name="model">当前数据记录对应的实体对象。</param>
    /// <param name="dataReader">数据集读取器。</param>
    public delegate void DataQueryCallback<T>(T model, System.Data.IDataReader dataReader);
}