using System.ComponentModel;

namespace Symbol.Logger
{
    /// <summary>
    /// 接口：日志可观察对象。
    /// </summary>
    public interface ILoggerObservableObject : IObservableObject
    {
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// 获取日志前部分。
        /// </summary>
        string LoggerBefore { get; }
    }

}