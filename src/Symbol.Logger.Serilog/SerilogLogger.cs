
using Serilog.Events;
using System;
using System.Collections.Generic;

using SerilogILogger = Serilog.ILogger;

namespace Symbol.Logger.Serilog
{
    /// <summary>
    /// 基于Serilog的日志包装
    /// </summary>
    class SerilogLogger : ILogger
    {
        private readonly SerilogILogger _logger;
        private static readonly IDictionary<LogLevel, LogEventLevel> _logLevels = new Dictionary<LogLevel, LogEventLevel>()
        {
            { LogLevel.Trace, LogEventLevel.Verbose },
            { LogLevel.Debug, LogEventLevel.Debug },
            { LogLevel.Information, LogEventLevel.Information },
            { LogLevel.Warning, LogEventLevel.Warning },
            { LogLevel.Error, LogEventLevel.Error },
            { LogLevel.Critical, LogEventLevel.Fatal },
            { LogLevel.None, LogEventLevel.Fatal+1 },
        };

        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="logger">日志对象。</param>
        public SerilogLogger(SerilogILogger logger)
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
            _logger.Write(_logLevels[logLevel], message, args);
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
            _logger.Write(_logLevels[logLevel], exception, message, args);
        }

    }
}