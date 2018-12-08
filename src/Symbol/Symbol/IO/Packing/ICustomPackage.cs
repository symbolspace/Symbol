/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
        /// <summary>
    /// 自定义打包接口
    /// </summary>
    public interface ICustomPackage {
        /// <summary>
        /// 将对象打包为byte[]
        /// </summary>
        /// <param name="instance">需要保存的实例，没有null的情况。</param>
        /// <returns>返回打包后的数据。</returns>
        byte[] Save(object instance);
        /// <summary>
        /// 从byte[]中加载对象。
        /// </summary>
        /// <param name="buffer">对象的数据。</param>
        /// <returns>返回解析后的对象。</returns>
        object Load(byte[] buffer);
    }

}