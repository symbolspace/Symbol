/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace System.Collections.Generic {
    /// <summary>
    /// ICollection&lt;T&gt;的扩展类。
    /// </summary>
    public static class ICollectionExtensions {

        #region methods

        #region AddRange
        /// <summary>
        /// 将指定的枚举追加到集合中。
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <param name="collection">当前集合。</param>
        /// <param name="source">可枚举遍历的枚举。</param>
        /// <exception cref="System.ArgumentNullException">当collection或source为null时。</exception>
        public static void AddRange<T>(
#if !net20
            this 
#endif
            ICollection<T> source, IEnumerable<T> collection) {
            foreach (T item in collection) {
                source.Add(item);
            }
        }
        #endregion

        #endregion

    }
}