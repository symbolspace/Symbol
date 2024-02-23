using System;

namespace Symbol.Logger
{
    /// <summary>
    /// 静态扩展：日志对象。
    /// </summary>
    public static class LoggerExtensions
    {

        /// <summary>
        /// 格式化输出跟踪日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Trace(
#if !NET20
        this 
#endif
        ILogger logger, string message, params object[] args)
        {
            logger.Log(LogLevel.Trace, message, args);
        }
        /// <summary>
        /// 格式化输出跟踪日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Trace(
#if !NET20
        this 
#endif
        ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(LogLevel.Trace, exception, message, args);
        }


        /// <summary>
        /// 格式化输出调试日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Debug(
#if !NET20
        this 
#endif
        ILogger logger, string message, params object[] args)
        {
            logger.Log(LogLevel.Debug, message, args);
        }
        /// <summary>
        /// 格式化输出调试日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Debug(
#if !NET20
        this 
#endif
        ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(LogLevel.Debug, exception, message, args);
        }

        /// <summary>
        /// 格式化输出信息日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Information(
#if !NET20
        this 
#endif
        ILogger logger, string message, params object[] args)
        {
            logger.Log(LogLevel.Information, message, args);
        }
        /// <summary>
        /// 格式化输出信息日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Information(
#if !NET20
        this 
#endif
        ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(LogLevel.Information, exception, message, args);
        }


        /// <summary>
        /// 格式化输出警告日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Warning(
#if !NET20
        this 
#endif
        ILogger logger, string message, params object[] args)
        {
            logger.Log(LogLevel.Warning, message, args);
        }
        /// <summary>
        /// 格式化输出警告日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Warning(
#if !NET20
        this 
#endif
        ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(LogLevel.Warning, exception, message, args);
        }

        /// <summary>
        /// 格式化输出错误日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Error(
#if !NET20
        this 
#endif
        ILogger logger, string message, params object[] args)
        {
            logger.Log(LogLevel.Error, message, args);
        }
        /// <summary>
        /// 格式化输出错误日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Error(
#if !NET20
        this 
#endif
        ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(LogLevel.Error, exception, message, args);
        }

        /// <summary>
        /// 格式化输出致命异常日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Critical(
#if !NET20
        this 
#endif
        ILogger logger, string message, params object[] args)
        {
            logger.Log(LogLevel.Critical, message, args);
        }
        /// <summary>
        /// 格式化输出致命异常日志。
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的消息。</param>
        /// <param name="args">格式参数。</param>
        public static void Critical(
#if !NET20
        this 
#endif
        ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(LogLevel.Critical, exception, message, args);
        }

    }

}