/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 引用关系实例
    /// </summary>
    public interface IReferInstance {
        /// <summary>
        /// 获取引用关系对象
        /// </summary>
        Refer Refer { get; }

        /// <summary>
        /// 引用（目标为当前查询主对象$self，源字段为id，目标字段为name+Id）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="source">源自</param>
        /// <returns></returns>
        IReferInstance Refence(string name, string source);
        /// <summary>
        /// 引用（目标为当前查询主对象$self，源字段为id）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="source">源自</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        IReferInstance Refence(string name, string source, string targetField);
        /// <summary>
        /// 引用（目标为当前查询主对象$self）
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="source">源自</param>
        /// <param name="sourceField">源字段</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        IReferInstance Refence(string name, string source, string sourceField, string targetField);
        /// <summary>
        /// 引用
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="source">源自</param>
        /// <param name="sourceField">源字段</param>
        /// <param name="target">目标，当前查询主对象为$self</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        IReferInstance Refence(string name, string source, string sourceField, string target, string targetField);

        /// <summary>
        /// 输出为Json文本
        /// </summary>
        /// <param name="formated">是否格式化。</param>
        /// <returns></returns>
        string Json(bool formated = false);

    }
   

}