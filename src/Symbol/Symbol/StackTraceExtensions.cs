using System.Diagnostics;

namespace System
{
    /// <summary>
    /// 静态扩展：栈跟踪
    /// </summary>
    public static class StackTraceExtensions
    {

        /// <summary>
        /// 获取调用成员名称。
        /// </summary>
        /// <param name="stackTrace">栈跟踪，不能为空。</param>
        /// <param name="index">索引序号，从0开始。</param>
        /// <returns>返回调用方法名称。</returns>
        public static string GetCallMemberName(
#if !NET20
        this 
#endif
        StackTrace stackTrace, int index)
        {
            var frame= stackTrace.GetFrame(index);
            return frame.GetMethod()?.Name;
        }
        
    }
}