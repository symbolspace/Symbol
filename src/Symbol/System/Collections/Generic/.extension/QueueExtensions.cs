/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace System.Collections.Generic {
    /// <summary>
    /// Queue&lt;T&gt;的扩展类
    /// </summary>
    public static class QueueExtensions {

        #region methods

        #region ToQueue
        /// <summary>
        /// 将集合成员转换为一个Queue&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <param name="source">可枚举遍历的集合。</param>
        /// <returns>返回转换后的Queue&lt;T&gt;。有成员都会返回Queue&lt;T&gt;实例。</returns>
        /// <exception cref="System.ArgumentNullException">当source为null时。</exception>
        public static Queue<T> ToQueue<T>(
#if !net20
            this 
#endif
            IEnumerable<T> source) {
            return ToQueue<T>(source, null);
        }
        /// <summary>
        /// 将满足条件的成员转换为一个Queue&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <param name="source">可枚举遍历的集合。</param>
        /// <param name="predicate">判断条件，如果为null将匹配所有的成员。</param>
        /// <returns>返回转换后的Queue&lt;T&gt;。有无匹配都会返回Queue&lt;T&gt;实例。</returns>
        /// <exception cref="System.ArgumentNullException">当source为null时。</exception>
        public static Queue<T> ToQueue<T>(
#if !net20
            this 
#endif
            IEnumerable<T> source, Predicate<T> predicate) {
            if (source == null)
                throw new ArgumentNullException("source");
            Queue<T> result = new Queue<T>();
            foreach (T item in source) {
                if (predicate == null || predicate(item))
                    result.Enqueue(item);
            }
            return result;
        }
        #endregion

        #region AddRange
        /// <summary>
        /// 将指定的集合追加到队列中。
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <param name="collection">当前队列。</param>
        /// <param name="source">可枚举遍历的集合。</param>
        /// <exception cref="System.ArgumentNullException">当collection或source为null时。</exception>
        public static void AddRange<T>(
#if !net20
            this 
#endif
            Queue<T> collection, IEnumerable<T> source) {
            Symbol.CommonException.CheckArgumentNull(collection, "collection");
            Symbol.CommonException.CheckArgumentNull(source, "source");

            foreach (T item in source) {
                collection.Enqueue(item);
            }
        }
        /// <summary>
        /// 将指定的集合追加到队列中。
        /// </summary>
        /// <typeparam name="T">任意类型。</typeparam>
        /// <param name="collection">当前队列。</param>
        /// <param name="source">需要追加的成员数组。</param>
        /// <param name="index">起始位置，从0开始。</param>
        /// <param name="count">追加数量，-1表示从index开始剩下的，反之是从index开始往后多少个成员。</param>
        public static void AddRange<T>(
#if !net20
            this 
#endif
            Queue<T> collection, T[] source, int index = 0, int count = -1) {
            Symbol.CommonException.CheckArgumentNull(collection, "collection");
            Symbol.CommonException.CheckArgumentNull(source, "source");

            int max=source.Length;
            if(count!=-1)
                max=index+count;

            for (int i = index; i < max; i++) {
                collection.Enqueue(source[i]);
            }
        }
        #endregion

        #endregion
    }
}