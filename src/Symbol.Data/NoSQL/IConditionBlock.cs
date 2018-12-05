/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.NoSQL {

    /// <summary>
    /// 规则对象块接口
    /// </summary>
    public interface IConditionBlock {
        /// <summary>
        /// 匹配：相等  $eq == =
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock Eq(string name, object value);
        /// <summary>
        /// 匹配：相等 $!eq $noteq $neq &lt;&gt; !=
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock NotEq(string name, object value);
        /// <summary>
        /// 匹配：大于 $gt &gt;
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock Gt(string name, object value);
        /// <summary>
        /// 匹配：大于、相等 $gte &gt;=
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock Gte(string name, object value);
        /// <summary>
        /// 匹配：小于 $lt &lt;
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock Lt(string name, object value);
        /// <summary>
        /// 匹配：小于、相等 $lte &lt;=
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock Lte(string name, object value);
        /// <summary>
        /// 匹配：出现在数组中 $in
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">任意数组对象，支持json数组字符串</param>
        /// <returns></returns>
        IConditionBlock In(string name, object value);
        /// <summary>
        /// 匹配：不出现在数组中 $!in $notin 
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">任意数组对象，支持json数组字符串</param>
        /// <returns></returns>
        IConditionBlock NotIn(string name, object value);
        /// <summary>
        /// 匹配：为空 $nul $null
        /// </summary>
        /// <param name="name">键</param>
        /// <returns></returns>
        IConditionBlock Null(string name);
        /// <summary>
        /// 匹配：模糊匹配 $like
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock Like(string name, string value);
        /// <summary>
        /// 匹配：模糊匹配 $like
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        IConditionBlock Like(string name, string value, bool reverse);
        /// <summary>
        /// 匹配：以指定文本起始 $start
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock Start(string name, string value);
        /// <summary>
        /// 匹配：以指定文本起始 $start
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <returns></returns>
        IConditionBlock Start(string name, string value, bool reverse);
        /// <summary>
        /// 匹配：以指定文本结尾 $end
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        IConditionBlock End(string name, string value);
        /// <summary>
        /// 匹配：以指定文本结尾 $end
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <param name="reverse">倒转，为true时表示value like field。</param>
        /// <returns></returns>
        IConditionBlock End(string name, string value, bool reverse);


        /// <summary>
        /// 逻辑与 $and &amp;&amp;
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns></returns>
        IConditionBlock And(ConditionBlockAction action);
        /// <summary>
        /// 逻辑或 $or ||
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns></returns>
        IConditionBlock Or(ConditionBlockAction action);
        /// <summary>
        /// 逻辑非 ! $not 
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns></returns>
        IConditionBlock Not(ConditionBlockAction action);

        /// <summary>
        /// 引用 #  $ref
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="targetName">目标键</param>
        /// <returns></returns>
        IConditionBlock Ref(string name,string targetName);

    }
    /// <summary>
    /// 规则对象块委托
    /// </summary>
    /// <param name="block"></param>
    public delegate void ConditionBlockAction(IConditionBlock block);

}