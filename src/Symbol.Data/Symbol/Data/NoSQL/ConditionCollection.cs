/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
namespace Symbol.Data.NoSQL {

    /// <summary>
    /// Condition集合
    /// </summary>
    public class ConditionCollection : System.Collections.Generic.IList<Condition> {

        #region fields
        private System.Collections.Generic.List<Condition> _list = null;
        private Condition _owner;
        private bool _isArray;
        #endregion

        #region properties

        #region this[int index]
        /// <summary>
        /// 获取或设置成员
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回成员</returns>
        public Condition this[int index] {
            get {
                return (index < 0 || index > _list.Count - 1) ? null : _list[index];
            }

            set {
                if (index < 0) {
                    if (value == null)
                        return;
                    SetItemParent(value);
                    _list.Insert(0, value);
                } else if (index > _list.Count - 1) {
                    if (value == null)
                        return;
                    SetItemParent(value);
                    _list.Add(value);
                } else {
                    if (value == null) {
                        _list.RemoveAt(index);
                    } else {
                        SetItemParent(value);
                        _list[index] = value;
                    }
                }
            }
        }
        #endregion
        #region Count
        /// <summary>
        /// 获取数量
        /// </summary>
        public int Count {
            get {
                return _list.Count;
            }
        }
        #endregion
        #region IsReadOnly
        bool System.Collections.Generic.ICollection<Condition>.IsReadOnly {
            get {
                return false;
            }
        }
        #endregion
        #region IsArray
        /// <summary>
        /// 获取或设置是否为数组
        /// </summary>
        public bool IsArray {
            get {
                return _isArray;
            }
            set {
                _isArray = value;
            }
        }
        #endregion
        #region Owner
        /// <summary>
        /// 获取拥有者
        /// </summary>
        public Condition Owner { get { return _owner; } }
        #endregion
        #endregion

        #region ctor
        /// <summary>
        /// 创建ConditionCollection实例
        /// </summary>
        public ConditionCollection(Condition owner) {
            _owner = owner;
            _list = new System.Collections.Generic.List<Condition>();
        }
        #endregion



        #region methods

        #region SetItemParent
        void SetItemParent(Condition item) {
            ((IConditionSetter)item).SetParent(_owner);
        }
        #endregion

        #region Insert
        void System.Collections.Generic.IList<Condition>.Insert(int index, Condition item) {
            Insert(index, item);
        }
        /// <summary>
        /// 插入成员（自动检查重复）
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="item">自动忽略null</param>
        /// <returns>返回是否成功</returns>
        public bool Insert(int index, Condition item) {
            if (item == null)
                return false;
            SetItemParent(item);
            int index2 = IndexOf(item);
            if (index2 > -1)
                return false;
            if (index < 0 || index > _list.Count - 1) {
                this[index] = item;
            } else {
                _list.Insert(index, item);
            }
            return true;
        }
        #endregion
        #region Add
        void System.Collections.Generic.ICollection<Condition>.Add(Condition item) {
            Add(item);
        }
        /// <summary>
        /// 添加成员（自动检查重复）
        /// </summary>
        /// <param name="item">自动忽略null</param>
        /// <returns>返回是否成功</returns>
        public bool Add(Condition item) {
            if (item == null)
                return false;
            SetItemParent(item);
            int index = IndexOf(item);
            if (index == -1) {
                _list.Add(item);
                return true;
            }
            return false;
        }
        #endregion
        #region Remove
        /// <summary>
        /// 移除成员（按名称）
        /// </summary>
        /// <param name="name">名称自动忽略null或empty</param>
        /// <returns></returns>
        public bool Remove(string name) {
            if (string.IsNullOrEmpty(name))
                return false;
            return _list.RemoveAll(p => p.Name == name) > 0;
        }
        #endregion
        #region Remove
        /// <summary>
        /// 移除成员
        /// </summary>
        /// <param name="item">自动忽略null</param>
        /// <returns></returns>
        public bool Remove(Condition item) {
            if (item == null)
                return false;
            return _list.Remove(item);
        }
        #endregion
        #region RemoveAt
        void System.Collections.Generic.IList<Condition>.RemoveAt(int index) {
            RemoveAt(index);
        }
        /// <summary>
        /// 移除成员(按索引值)
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns></returns>
        public bool RemoveAt(int index) {
            if (index < 0 || index > _list.Count - 1) {
                return false;
            }
            _list.RemoveAt(index);
            return true;
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
        #region Contains
        /// <summary>
        /// 是否包含成员
        /// </summary>
        /// <param name="item">自动忽略null</param>
        /// <returns></returns>
        public bool Contains(Condition item) {
            if (item == null)
                return false;
            return _list.Contains(item);
        }
        #endregion
        #region IndexOf
        /// <summary>
        /// 检测成员的索引值
        /// </summary>
        /// <param name="item">自动忽略null</param>
        /// <returns></returns>
        public int IndexOf(Condition item) {
            if (item == null)
                return -1;
            return _list.IndexOf(item);
        }
        #endregion
        #region IndexOf
        /// <summary>
        /// 检测成员的索引值
        /// </summary>
        /// <param name="match">判断规则</param>
        /// <returns></returns>
        public int IndexOf(Predicate<Condition> match) {
            if (match == null)
                return -1;
            return _list.FindIndex(match);
        }
        #endregion
        /// <summary>
        /// 查找匹配的元素
        /// </summary>
        /// <param name="match">判断规则</param>
        /// <returns>找不到返回null</returns>
        public Condition Find(Predicate<Condition> match) {
            if (match == null)
                return null;
            return _list.Find(match);
        }
        #region CopyTo
        /// <summary>
        /// 复制成员到数组中
        /// </summary>
        /// <param name="array">目标数组</param>
        /// <param name="arrayIndex">从0开始的索引</param>
        public void CopyTo(Condition[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }
        #endregion
        #region ToArray
        /// <summary>
        /// 成员数组
        /// </summary>
        /// <returns></returns>
        public Condition[] ToArray() {
            return _list.ToArray();
        }
        #endregion
        #region GetEnumerator
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerator<Condition> GetEnumerator() {
            return _list.GetEnumerator();
        }
        #endregion

        //indexof index count
        //lastindexof
        //insertrange
        //addrange
        //exists
        //find findall findindex
        //removeall sort toarray trim trueforall
        //reverse


        #endregion

    }

}