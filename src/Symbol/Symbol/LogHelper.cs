/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Reflection;
namespace Symbol;

/// <summary>
/// 日志辅助类。
/// </summary>
public class LogHelper {

    #region methods

    #region ExceptionToString
    /// <summary>
    /// 将异常转换为文本（Message、StackTrace、Data、InnerException）。
    /// </summary>
    /// <param name="error">当前异常。</param>
    /// <returns>返回详细的信息。</returns>
    [Obsolete("请更改为LogBase.ExceptionToString")]
    public static string ExceptionToString(Exception error) {
        return LogBase.ExceptionToString(error);
    }
    /// <summary>
    /// 将异常转换为文本。
    /// </summary>
    /// <param name="error">当前异常。</param>
    /// <param name="builder">输出的文本缓冲区。</param>
    /// <param name="layer">层数，会因为层数会自动在左边追加缩近。</param>
    [Obsolete("请更改为LogBase.ExceptionToString")]
    public static void ExceptionToString(Exception error, System.Text.StringBuilder builder, int layer) {
        LogBase.ExceptionToString(error, builder, layer);
    }
    #endregion


    #region WriteEventLog
#if !netcore
    /// <summary>
    /// 写入系统事件（在控制面板－事件查看，可以看到这些事件记录，日志类型：消息）。
    /// </summary>
    /// <param name="message">消息，也可以是带格式的文本。</param>
    /// <param name="args">如果message是一个格式串，args将参与格式。</param>
    public static void WriteEventLog(string message, params object[] args) {
        WriteEventLog(message, args, System.Diagnostics.EventLogEntryType.Information);
    }
    /// <summary>
    /// 写入系统事件（在控制面板－事件查看，可以看到这些事件记录，日志类型：消息）。
    /// </summary>
    /// <param name="message">消息，也可以是带格式的文本。</param>
    /// <param name="source">事件源，比如：测试程序。</param>
    /// <param name="args">如果message是一个格式串，args将参与格式。</param>
    public static void WriteEventLog(string message, string source, params object[] args) {
        WriteEventLog(message, args, System.Diagnostics.EventLogEntryType.Information, -1, -1, source);
    }
    /// <summary>
    /// 写入系统事件（在控制面板－事件查看，可以看到这些事件记录）。
    /// </summary>
    /// <param name="message">消息，也可以是带格式的文本。</param>
    /// <param name="logType">日志类型。</param>
    /// <param name="args">如果message是一个格式串，args将参与格式。</param>
    public static void WriteEventLog(string message, System.Diagnostics.EventLogEntryType logType, params object[] args) {
        WriteEventLog(message, args, logType);
    }
    /// <summary>
    /// 写入系统事件（在控制面板－事件查看，可以看到这些事件记录）。
    /// </summary>
    /// <param name="message">消息，也可以是带格式的文本。</param>
    /// <param name="logType">日志类型。</param>
    /// <param name="source">事件源，比如：测试程序。</param>
    /// <param name="args">如果message是一个格式串，args将参与格式。</param>
    public static void WriteEventLog(string message, System.Diagnostics.EventLogEntryType logType, string source, params object[] args) {
        WriteEventLog(message, args, logType, -1, -1, source);
    }
    /// <summary>
    /// 写入系统事件（在控制面板－事件查看，可以看到这些事件记录）。
    /// </summary>
    /// <param name="message">消息，也可以是带格式的文本。</param>
    /// <param name="args">如果message是一个格式串，args将参与格式。</param>
    /// <param name="logType">日志类型。</param>
    /// <param name="eventId">事件Id，有效值：1-65535。</param>
    /// <param name="category">子类Id，有效值：1-32767。</param>
    /// <param name="source">事件源，比如：测试程序。</param>
    /// <param name="logName">日志名称，一般为Application（应用程序）。</param>
    public static void WriteEventLog(string message, object[] args, System.Diagnostics.EventLogEntryType logType= System.Diagnostics.EventLogEntryType.Information, int eventId = -1, short category = -1, string source = null, string logName = "Application") {
        using (System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog()) {
            eventLog.Log = string.IsNullOrEmpty(logName) ? "Application" : logName;
            if (!string.IsNullOrEmpty(source))
                eventLog.Source = source;
            else
                eventLog.Source = AppHelper.Assembly.FullName;

            if (message != null && args != null && args.Length > 0)
                message = string.Format(message, args);
            if (eventId != -1) {
                if (category != -1) {
                    eventLog.WriteEntry(message, logType, eventId, category);
                } else {
                    eventLog.WriteEntry(message, logType, eventId);
                }
            } else {
                eventLog.WriteEntry(message, logType);
            }
        }
    }
#endif
    #endregion

    #endregion

}