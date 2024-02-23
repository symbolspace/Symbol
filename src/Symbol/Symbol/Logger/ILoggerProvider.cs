using System;

namespace Symbol.Logger
{
    /// <summary>
    /// 接口：日志对象提供者。
    /// </summary>
    public interface ILoggerProvider
    {
        /// <summary>
        /// 获取提供者名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <param name="name">日志名称。</param>
        /// <returns>返回日志对象。</returns>
        ILogger GetLogger(string name);
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <param name="type">类型定义。</param>
        /// <returns>返回日志对象。</returns>
        ILogger GetLogger(Type type);
        /// <summary>
        /// 获取日志对象。
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <returns>返回日志对象。</returns>
        ILogger GetLogger<T>();

    }

}