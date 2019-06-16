/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// 数据查询接口。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    public interface IDataQuery<T> 
            : System.IDisposable
            , Collections.Generic.IPagingCollection<T>
            , System.Collections.Generic.IEnumerable<T>
            , System.Collections.IEnumerable
        {
        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        IDataContext DataContext { get; }
        /// <summary>
        /// 获取或设置数据绑定缓存对象。
        /// </summary>
        Binding.IDataBinderObjectCache DataBinderObjectCache { get; set; }
        /// <summary>
        /// 获取当前实体的类型。
        /// </summary>
        System.Type Type { get; }
        /// <summary>
        /// 获取命令对象。
        /// </summary>
        ICommand Command { get; }
        /// <summary>
        /// 命令文本之前的内容，有时CommandText可能会被修改，但有一部分可能会影响修改过程，可以提取出来设置到此属性上。
        /// </summary>
        /// <remarks>在每次执行地，如果此属性有值，将会放在CommandText之前。</remarks>
        string CommandTextBefore { get; set; }
        /// <summary>
        /// 获取或设置当前查询命令语句。
        /// </summary>
        string CommandText { get; set; }
        /// <summary>
        /// 获取或设置当前查询超时时间（秒，不会影响到DataContext）。
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// 获取或设置数据查询器回调委托。
        /// </summary>
        DataQueryCallback<T> Callback { get; set; }

        /// <summary>
        /// 创建查询命令构造器（自动关联参数）。
        /// </summary>
        /// <returns>返回构造器对象。</returns>
        ISelectCommandBuilder CreateBuilder();

        /// <summary>
        /// 求出当前查询的数据记录数。
        /// </summary>
        /// <returns>返回当前查询的数据记录数。</returns>
        int Count();
        /// <summary>
        /// 求出当前查询的数据记录数。
        /// </summary>
        /// <param name="commandText">指定查询方式。</param>
        /// <returns>返回当前查询的数据记录数。</returns>
        int Count(string commandText);


        /// <summary>
        /// 生成分页语法。
        /// </summary>
        /// <param name="size">每页大小，忽略小于1。</param>
        /// <param name="page">页码，从0开始，忽略小于0。</param>
        IDataQuery<T> Paging(int size, int page);



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
    /// <param name="reader">数据查询读取器。</param>
    public delegate void DataQueryCallback<T>(T model, IDataQueryReader reader);
}