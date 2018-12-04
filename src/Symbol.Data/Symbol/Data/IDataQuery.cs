/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {
    /// <summary>
    /// 数据查询接口（非泛型和调用时不方便使用泛型）。
    /// </summary>
    public interface IDataQuery :
        System.Collections.IEnumerable,
        System.IDisposable {

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
        /// 获取或设置当前查询的类型。
        /// </summary>
        System.Data.CommandType CommandType { get; set; }
        /// <summary>
        /// 获取或设置当前查询的连接。
        /// </summary>
        System.Data.IDbConnection Connection { get; set; }
        /// <summary>
        /// 获取或设置当前查询的事务对象。
        /// </summary>
        System.Data.IDbTransaction Transaction { get; set; }
        /// <summary>
        /// 获取当前查询命令对象。
        /// </summary>
        System.Data.IDbCommand Command { get; }

        /// <summary>
        /// 获取当前查询的参数列表。
        /// </summary>
        System.Data.IDataParameterCollection Parameters { get; }


        /// <summary>
        /// 获取或设置当前查询的更新行选项。
        /// </summary>
        /// 
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
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <returns>返回下一个参数的名称。</returns>
        string NextParamName();
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <param name="offset">偏移多少个参数，可能用于预留。</param>
        /// <returns>返回下一个参数的名称。</returns>
        string NextParamName(int offset);

        /// <summary>
        /// 创建查询参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <returns>返回参数实例。</returns>
        System.Data.IDbDataParameter CreateParameter();
        /// <summary>
        /// 创建查询参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="parameterName">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        System.Data.IDbDataParameter CreateParameter(string parameterName, object value);
        /// <summary>
        /// 添加一个查询参数（无值）。
        /// </summary>
        /// <returns>返回参数实例。</returns>
        System.Data.IDbDataParameter AddParameter();
        /// <summary>
        /// 添加一个查询参数（自动命名@p1 @p2）。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        System.Data.IDbDataParameter AddParameter(object value);
        /// <summary>
        /// 添加一个查询参数。
        /// </summary>
        /// <param name="parameterName">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        System.Data.IDbDataParameter AddParameter(string parameterName, object value);

        /// <summary>
        /// 创建查询命令构造器（自动关联参数）。
        /// </summary>
        /// <returns>返回构造器对象。</returns>
        ISelectCommandBuilder CreateBuilder();

        /// <summary>
        /// 生成分页语法。
        /// </summary>
        /// <param name="size">每页大小，忽略小于1。</param>
        /// <param name="page">页码，从0开始，忽略小于0。</param>
        void Paging(int size, int page);

        ///// <summary>
        ///// 从查询中拿出第一条记录，如果查询未包含任何记录，将返回此类型的default(T);
        ///// </summary>
        ///// <returns>返回第一条记录。</returns>
        //object FirstOrDefault();
        ///// <summary>
        ///// 将查询快速读取并构造一个List对象。
        ///// </summary>
        ///// <returns>返回一个List对象。</returns>
        //System.Collections.Generic.List<object> ToList();

    }

}