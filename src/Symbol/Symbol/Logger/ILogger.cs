using System;

namespace Symbol.Logger
{
    /// <summary>
    /// 接口：日志对象。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 是否启用目标日志等级。
        /// </summary>
        /// <param name="logLevel">日志等级。</param>
        /// <returns>返回目标日志等级是否启用。</returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// 写入日志。
        /// </summary>
        /// <param name="logLevel">日志等级。</param>
        /// <param name="message">带格式串的日志消息。</param>
        /// <param name="args">格式串对应的参数。</param>
        void Log(LogLevel logLevel, string message, params object[] args);
        /// <summary>
        /// 写入日志。
        /// </summary>
        /// <param name="logLevel">日志等级。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的日志消息。</param>
        /// <param name="args">格式串对应的参数。</param>
        void Log(LogLevel logLevel, Exception exception, string message, params object[] args);

    }

}