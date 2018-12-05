/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 规则对象实例
    /// </summary>
    public interface IConditionInstance {
        /// <summary>
        /// 获取规则对象
        /// </summary>
        Condition Condition { get; }

        /// <summary>
        /// 逻辑与
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns></returns>
        IConditionInstance And(ConditionBlockAction action);
        /// <summary>
        /// 逻辑或
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns></returns>
        IConditionInstance Or(ConditionBlockAction action);
        /// <summary>
        /// 逻辑非
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns></returns>
        IConditionInstance Not(ConditionBlockAction action);
        /// <summary>
        /// 输出为Json文本
        /// </summary>
        /// <param name="formated">是否格式化。</param>
        /// <returns></returns>
        string Json(bool formated = false);

    }

}