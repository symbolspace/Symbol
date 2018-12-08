/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    partial class TreePackage {
        /// <summary>
        /// 项列表接口
        /// </summary>
        public interface IEntryList {
            /// <summary>
            /// 获取键值对应的项
            /// </summary>
            /// <param name="key">键值，如果不存在，将返回 null 。</param>
            /// <returns>返回此键值对应的项。</returns>
            Entry this[string key] { get; }
        }
    }
}