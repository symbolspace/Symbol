/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand 
        : System.IDisposable {

        /// <summary>
        /// 获取相关联的数据上下文。
        /// </summary>
        IDataContext DataContext { get; }
        /// <summary>
        /// 获取参数列表。
        /// </summary>
        ICommandParameterList Parameters { get; }

        /// <summary>
        /// 获取或设置当前查询命令语句。
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 获取或设置当前超时时间（秒，不会影响到DataContext）。
        /// </summary>
        int Timeout { get; set; }


        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <returns>返回查询结果。</returns>
        object ExecuteScalar();
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回查询结果。</returns>
        object ExecuteScalar(string commandText);
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <returns>返回查询结果。</returns>
        T ExecuteScalar<T>();
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回查询结果。</returns>
        T ExecuteScalar<T>(string commandText);
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回查询结果。</returns>
        T ExecuteScalar<T>(T defaultValue) where T : struct;
        /// <summary>
        /// 执行查询，并返回查询的第一条记录的第一个列。
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="commandText">命令文本</param>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回查询结果。</returns>
        T ExecuteScalar<T>(string commandText, T defaultValue) where T : struct;

        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        int ExecuteNonQuery();
        /// <summary>
        /// 执行查询，并且忽略返回值；如果期望用返回值当影响行数，建议最好用 ExecuteScalar 直接返回一个值更可靠一些。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>不要期望返回的值当作影响的行数，实验证明这个返回值真的不靠谱，不同数据库实现结果不一样。</returns>
        int ExecuteNonQuery(string commandText);

        /// <summary>
        /// 调用函数
        /// </summary>
        /// <returns>返回此函数的执行结果</returns>
        object ExecuteFunction();
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限系统基础类型）</typeparam>
        /// <returns>返回此函数的执行结果</returns>
        T ExecuteFunction<T>();
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <typeparam name="T">任意类型（仅限struct类型）</typeparam>
        /// <param name="defaultValue">默认值，为null、DBNull、转换不成功时生效。</param>
        /// <returns>返回此函数的执行结果</returns>
        T ExecuteFunction<T>(T defaultValue) where T : struct;


        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <returns>返回存储过程的值。</returns>
        object ExecuteStoredProcedure();


        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns>返回数据查询读取器。</returns>
        IDataQueryReader ExecuteReader();
        /// <summary>
        /// 执行查询。
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns>返回数据查询读取器。</returns>
        IDataQueryReader ExecuteReader(string commandText);

    }


}