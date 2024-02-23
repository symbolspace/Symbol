using System;

namespace Symbol.Logger
{
    /// <summary>
    /// 枚举：日志等级。
    /// </summary>
    [Const("日志等级")]
    public enum LogLevel : int
    {
        /// <summary>
        /// 跟踪
        /// </summary>
        /// <remarks>包含最详细消息的日志，可能包含敏感信息。在默认情况下建议是禁用，不应该在生产环境中启用。</remarks>
        [Const("跟踪")]
        [Const("Description", "包含最详细消息的日志，可能包含敏感信息。在默认情况下建议是禁用，不应该在生产环境中启用。")]
        Trace = 0,
        /// <summary>
        /// 调试
        /// </summary>
        /// <remarks>在开发期间对调试有用的信息的信息，没有长期价值。</remarks>
        [Const("调试")]
        [Const("Description", "在开发期间对调试有用的信息的信息，没有长期价值。")]
        Debug = 1,
        /// <summary>
        /// 信息
        /// </summary>
        /// <remarks>跟踪应用程序一般流程的日志。这些日志应该具有长期价值。</remarks>
        [Const("信息")]
        [Const("Description", "跟踪应用程序一般流程的日志。这些日志应该具有长期价值。")]
        Information = 2,
        /// <summary>
        /// 警告
        /// </summary>
        /// <remarks>应用程序中出现异常或意外事件，不会引起应用程序崩溃。</remarks>
        [Const("警告")]
        [Const("Description", "应用程序中出现异常或意外事件，不会引起应用程序崩溃。")]
        Warning = 3,
        /// <summary>
        /// 错误
        /// </summary>
        /// <remarks>由于故障而停止当前执行流程的日志，而不是整个应用程序的故障。</remarks>
        [Const("错误")]
        [Const("Description", "由于故障而停止当前执行流程的日志，而不是整个应用程序的故障。")]
        Error = 4,
        /// <summary>
        /// 致命
        /// </summary>
        /// <remarks>描述不可恢复的应用程序或系统崩溃或灾难性的日志，需要立即处理的故障。</remarks>
        [Const("致命")]
        [Const("Description", "描述不可恢复的应用程序或系统崩溃或灾难性的日志，需要立即处理的故障。")]
        Critical = 5,
        /// <summary>
        /// 无
        /// </summary>
        /// <remarks>关闭日志。</remarks>
        [Const("无")]
        [Const("Description", "关闭日志。")]
        None = 6,
    }

}