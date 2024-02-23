using System.IO;

namespace Symbol.Localization
{
    /// <summary>
    /// 接口：资源管理器。
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// 获取资源文本。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的文本内容。</returns>
        string GetString(string key);
        /// <summary>
        /// 获取资源对象。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        object GetObject(string key);
        /// <summary>
        /// 获取资源流。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <returns>返回该资源标识对应的对象内容。</returns>
        Stream GetStream(string key);
    }

}