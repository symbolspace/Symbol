/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Cloud {

    /// <summary>
    /// 云Action过滤器列表。
    /// </summary>
    public class CloudActionFilterList : System.Collections.Generic.IEnumerable<CloudActionFilterAttribute> {

        #region fields
        private System.Collections.Generic.List<CloudActionFilterAttribute> _list;
        #endregion

        #region properties
        /// <summary>
        /// 获取数量。
        /// </summary>
        public int Count { get { return _list.Count; } }

        /// <summary>
        /// 获取指定索引的对象。
        /// </summary>
        /// <param name="index">小于0或超出Count-1时，直接返回null。</param>
        /// <returns>返回指定索引的对象。</returns>
        public CloudActionFilterAttribute this[int index] {
            get {
                if (index < 0 || index > _list.Count - 1)
                    return null;
                return _list[index];
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建CloudActionFilterList实例。
        /// </summary>
        public CloudActionFilterList() {
            _list = new System.Collections.Generic.List<CloudActionFilterAttribute>();
        }
        #endregion

        #region methods

        #region Contains
        /// <summary>
        /// 是否包含指定对象。
        /// </summary>
        /// <param name="item">为null直接返回false。</param>
        /// <returns>存在时返回true。</returns>
        public bool Contains(CloudActionFilterAttribute item) {
            if (item == null)
                return false;
            return _list.Contains(item);
        }
        #endregion
        #region Add
        /// <summary>
        /// 添加。
        /// </summary>
        /// <param name="item">为null直接返回0。</param>
        /// <returns>返回是否成功，已存在返回false。</returns>
        public bool Add(CloudActionFilterAttribute item) {
            if (item == null)
                return false;
            if (_list.Contains(item))
                return false;
            _list.Add(item);
            return true;
        }
        #endregion
        #region AddRange
        /// <summary>
        /// 添加。
        /// </summary>
        /// <param name="customAttributeProvider">可承载特性的对象：Type或MethodInfo，为null直接返回0。</param>
        /// <returns>返回成功添加的数量。</returns>
        public int AddRange(System.Reflection.ICustomAttributeProvider customAttributeProvider) {
            if (customAttributeProvider == null)
                return 0;
            int count = 0;

            foreach (var item in customAttributeProvider.GetCustomAttributes<CloudActionFilterAttribute>(true)) {
                if (Add(item))
                    count++;
            }
            return count;
        }
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">集合，为null直接返回0.</param>
        /// <returns>返回成功添加的数量。</returns>
        public int AddRange(params CloudActionFilterAttribute[] items) {
            if (items == null || items.Length == 0)
                return 0;
            int count = 0;
            foreach (var item in items) {
                if (Add(item))
                    count++;
            }
            return count;
        }
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">集合，为null直接返回0.</param>
        /// <returns>返回成功添加的数量。</returns>
        public int AddRange(System.Collections.Generic.IEnumerable<CloudActionFilterAttribute> items) {
            if (items == null)
                return 0;
            int count = 0;
            foreach (var item in items) {
                if (Add(item))
                    count++;
            }
            return count;
        }
        #endregion
        #region Remove
        /// <summary>
        /// 移除。
        /// </summary>
        /// <param name="item">为null时直接返回false。</param>
        /// <returns>操作成功返回true。</returns>
        public bool Remove(CloudActionFilterAttribute item) {
            if (item == null)
                return false;
            return _list.Remove(item);
        }
        #endregion
        #region Clear
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() {
            _list.Clear();
        }
        #endregion
        #region GetEnumerator
        /// <summary>
        /// 获取枚举器。
        /// </summary>
        /// <returns>返回可枚举对象。</returns>
        public System.Collections.Generic.IEnumerator<CloudActionFilterAttribute> GetEnumerator() {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
        #endregion
        #region ToArray
        /// <summary>
        /// 将所有元素复制到新数组中。
        /// </summary>
        /// <returns>返回元素数组。</returns>
        public CloudActionFilterAttribute[] ToArray() {
            return _list.ToArray();
        }
        #endregion

        #region Sort
        /// <summary>
        /// 排序
        /// </summary>
        public void Sort() {
            if (_list.Count == 0)
                return;
            double max = LinqHelper.Max(_list, p => p.Sort)+10D;
            var list = LinqHelper.OrderBy(
                                            _list, 
                                            p => p.Sort>0D?p.Sort: (max + _list.IndexOf(p))
                                  ).ToList();
            _list.Clear();
            _list = list;
        }
        #endregion

        #region ActionExecuteBefore
        /// <summary>
        /// Action调用前（如果设置context.Processed为true，将导致ActionExecute不执行）。
        /// </summary>
        /// <param name="cloudContext">云调用上下文。</param>
        public virtual void ActionExecuteBefore(CloudContext cloudContext) {
            if (cloudContext.Processed || _list.Count == 0)
                return;
            for (int i = 0; i < _list.Count; i++) {
                try {
                    _list[i].ActionExecuteBefore(cloudContext);
                    if (cloudContext.Processed)
                        return;
                } catch (Exception error) {
                    cloudContext.SetError(error);
                    return;
                }
            }
        }
        #endregion
        #region ActionExecute
        /// <summary>
        /// Action调用。
        /// </summary>
        /// <param name="cloudContext">云调用上下文。</param>
        public virtual void ActionExecute(CloudContext cloudContext) {
            if (cloudContext.Processed || _list.Count == 0)
                return;
            for (int i = 0; i < _list.Count; i++) {
                try {
                    _list[i].ActionExecute(cloudContext);
                    if (cloudContext.Processed)
                        return;
                } catch (Exception error) {
                    cloudContext.SetError(error);
                    return;
                }
            }
        }
        #endregion
        #region ActionExecuteAfter
        /// <summary>
        /// Action调用后。
        /// </summary>
        /// <param name="cloudContext">云调用上下文。</param>
        public virtual void ActionExecuteAfter(CloudContext cloudContext) {
            if (_list.Count == 0)
                return;
            for (int i = 0; i < _list.Count; i++) {
                try {
                    _list[i].ActionExecuteAfter(cloudContext);
                } catch (Exception error) {
                    cloudContext.SetError(error,-1);
                }
            }
        }


        #endregion

        #endregion

    }

}
