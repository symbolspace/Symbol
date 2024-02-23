
using System;
using System.Collections.Generic;

using NLogILogger = NLog.ILogger;
using NLogLogLevel = NLog.LogLevel;
using NLogManager = NLog.LogManager;

namespace Symbol.Logger.NLog
{
    /// <summary>
    /// 基于NLog的日志包装
    /// </summary>
    class NLogger : ILogger
    {
        private readonly NLogILogger _logger;
        private static readonly IDictionary<LogLevel, NLogLogLevel> _logLevels = new Dictionary<LogLevel, NLogLogLevel>()
        {
            { LogLevel.Trace, NLogLogLevel.Trace },
            { LogLevel.Debug, NLogLogLevel.Debug },
            { LogLevel.Information, NLogLogLevel.Info },
            { LogLevel.Warning, NLogLogLevel.Warn },
            { LogLevel.Error, NLogLogLevel.Error },
            { LogLevel.Critical, NLogLogLevel.Fatal },
            { LogLevel.None, NLogLogLevel.Off },
        };

        static NLogger()
        {
            NLogManager.AutoShutdown = true;
        }
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="logger">日志对象。</param>
        public NLogger(NLogILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 是否启用目标日志等级。
        /// </summary>
        /// <param name="logLevel">日志等级。</param>
        /// <returns>返回目标日志等级是否启用。</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(_logLevels[logLevel]);
        }

        /// <summary>
        /// 写入日志。
        /// </summary>
        /// <param name="logLevel">日志等级。</param>
        /// <param name="message">带格式串的日志消息。</param>
        /// <param name="args">格式串对应的参数。</param>
        public void Log(LogLevel logLevel, string message, params object[] args)
        {
            _logger.Log(_logLevels[logLevel], message, args);
        }
        /// <summary>
        /// 写入日志。
        /// </summary>
        /// <param name="logLevel">日志等级。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="message">带格式串的日志消息。</param>
        /// <param name="args">格式串对应的参数。</param>
        public void Log(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            _logger.Log(_logLevels[logLevel], exception, message, args);
        }

    }
}