/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 命令参数列表基类
    /// </summary>
    public abstract class CommandParameterList : ICommandParameterList {

        #region fields
        private IProvider _provider;
        private System.Collections.Generic.List<CommandParameter> _list;
        #endregion

        #region properties
        /// <summary>
        /// 获取提供者。
        /// </summary>
        public IProvider Provider { get { return ThreadHelper.InterlockedGet(ref _provider); } }

        /// <summary>
        /// 获取数量。
        /// </summary>
        public int Count {
            get {
                var list = ThreadHelper.InterlockedGet(ref _list);
                if (list == null)
                    return 0;
                return list.Count;
            }
        }
        /// <summary>
        /// 获取Return参数对象。
        /// </summary>
        public virtual CommandParameter ReturnParameter {
            get {
                var list = ThreadHelper.InterlockedGet(ref _list);
                if (list == null)
                    return null;
                return list.Find(p => p.IsReturn);
            }
        }

        /// <summary>
        /// 获取指定索引的参数。
        /// </summary>
        /// <param name="index">索引值，从0开始。</param>
        /// <returns></returns>
        public CommandParameter this[int index] {
            get { return Get(index); }
        }
        /// <summary>
        /// 获取指定名称的参数。
        /// </summary>
        /// <param name="name">参数名称，null或empty直接忽略。</param>
        /// <returns></returns>
        public CommandParameter this[string name] {
            get { return Get(name); }
        }


        #endregion

        #region ctor
        /// <summary>
        /// 创建CommandParameterList实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        public CommandParameterList(IProvider provider) {
            _provider = provider;
            _list = new System.Collections.Generic.List<CommandParameter>();
        }
        #endregion

        #region methods

        #region PreParameter
        ///// <summary>
        ///// 预处理参数
        ///// </summary>
        ///// <param name="parameterName">参数名称。</param>
        ///// <param name="value">值。</param>
        ///// <returns></returns>
        //public abstract object PreParameter(string parameterName, object value);
        #endregion

        #region NextName
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <returns>返回下一个参数的名称。</returns>
        public string NextName() {
            return NextName(1);
        }
        /// <summary>
        /// 获取下一个参数名，如：@p1 @p2。
        /// </summary>
        /// <param name="offset">偏移多少个参数，可能用于预留。</param>
        /// <returns>返回下一个参数的名称。</returns>
        public virtual string NextName(int offset) {
            return Provider?.Dialect?.ParameterNameGrammar("p" + (Count + offset));
        }
        #endregion


        #region Create
        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public CommandParameter Create(object value) {
            return Create(NextName(), value);
        }
        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        public CommandParameter Create(object value, object properties) {
            return Create(NextName(), value, properties);
        }

        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public CommandParameter Create(string name, object value) {
            return Create(name, value, null);
        }
        /// <summary>
        /// 创建参数，仅创建对象，不会追加到参数列表。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        public virtual CommandParameter Create(string name, object value, object properties) {
            var dialect = Provider?.Dialect;
            CommandParameter result = value as CommandParameter;
            if (result != null) {
                if (string.IsNullOrEmpty(result.Name))
                    result.Name = NextName();
                else
                    result.Name = dialect?.ParameterNameGrammar(result.Name);
                if (!result.Created) {
                    OnCreate(result);
                    result.Created = true;
                }
                return result;
            }
            result = new CommandParameter() {
                Value = value,
                RealType = value == null ? typeof(object) : value.GetType(),
            };
            if (string.IsNullOrEmpty(name)) {
                result.Name = NextName();
            } else {
                if (name.StartsWith("out_", System.StringComparison.OrdinalIgnoreCase)) {
                    result.Name = dialect?.ParameterNameGrammar(name.Substring(4));
                    result.IsOut = true;
                } else {
                    result.Name = dialect?.ParameterNameGrammar(name);
                }
            }
            if (properties != null)
                result.Properties = FastWrapper.As(properties);
            OnCreate(result);
            result.Created = true;
            return result;
        }
        /// <summary>
        /// 创建参数回调
        /// </summary>
        /// <param name="item">参数对象</param>
        protected abstract void OnCreate(CommandParameter item);
        #endregion

        #region Add
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="item">参数对象。</param>
        /// <returns></returns>
        public virtual CommandParameter Add(CommandParameter item) {
            if (item == null)
                return null;
            if (string.IsNullOrEmpty(item.Name))
                return null;
            Remove(item.Name);
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null)
                return null;
            list.Add(item);
            return item;
        }
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public CommandParameter Add(object value) {
            return Add(Create(value));
        }
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        public CommandParameter Add(object value, object properties) {
            return Add(Create(value, properties));
        }
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回参数实例。</returns>
        public CommandParameter Add(string name, object value) {
            return Add(Create(name, value, null));
        }
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="name">参数名称，必须以@开头。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="properties">属性列表。</param>
        /// <returns>返回参数实例。</returns>
        public virtual CommandParameter Add(string name, object value, object properties) {
            return Add(Create(name, value, properties));
        }
        #endregion
        #region AddRange
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">参数列表。</param>
        public void AddRange(object[] items) {
            if (items == null || items.Length == 0)
                return;
            foreach (var item in items) {
                Add(item);
            }
        }
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">参数列表。</param>
        public void AddRange(System.Collections.IEnumerable items) {
            if (items == null)
                return;
            foreach (var item in items) {
                Add(item);
            }
        }
        /// <summary>
        /// 批量添加。
        /// </summary>
        /// <param name="items">参数列表。</param>
        public void AddRange(ICommandParameterList items) {
            if (items == null || items.Count == 0)
                return;
            foreach (var item in items) {
                Add(item);
            }
        }
        #endregion

        #region Remove
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="name">参数名称，null或empty直接忽略。</param>
        public bool Remove(string name) {
            var item = Get(name);
            if (item == null)
                return false;
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null)
                return false;
            return list.Remove(item);
        }
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="index">索引值，从0开始。</param>
        public bool Remove(int index) {
            if (index < 0)
                return false;
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null)
                return false;

            if (list == null || index > list.Count - 1)
                return false;
            list.RemoveAt(index);
            return true;
        }
        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="item">参数对象。</param>
        public bool Remove(CommandParameter item) {
            if (item == null)
                return false;
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null)
                return false;

            return list.Remove(item);
        }
        #endregion

        #region Clear
        /// <summary>
        /// 清空参数。
        /// </summary>
        public void Clear() {
            var list = ThreadHelper.InterlockedGet(ref _list);
            list?.Clear();
        }
        #endregion

        #region Get
        /// <summary>
        /// 获取指定索引的参数。
        /// </summary>
        /// <param name="index">索引值，从0开始。</param>
        /// <returns></returns>
        public CommandParameter Get(int index) {
            if (index < 0)
                return null;
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null)
                return null;

            if (list == null || index > list.Count - 1)
                return null;
            return list[index];
        }
        /// <summary>
        /// 获取指定名称的参数。
        /// </summary>
        /// <param name="name">参数名称，null或empty直接忽略。</param>
        /// <returns></returns>
        public virtual CommandParameter Get(string name) {
            if (string.IsNullOrEmpty(name))
                return null;
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null)
                return null;
            return list.Find(p => p.Name == name);
        }
        #endregion

        #region ToArray
        /// <summary>
        /// 输出为数组。
        /// </summary>
        /// <returns></returns>
        public virtual CommandParameter[] ToArray() {
            return ThreadHelper.InterlockedGet(ref _list)?.ToArray() ?? new CommandParameter[0];
        }
        #endregion

        #region GetEnumerator
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerator<CommandParameter> GetEnumerator() {
            var list = ThreadHelper.InterlockedGet(ref _list);
            if (list == null)
                return null;
            return list.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 释放对象占用的资源。
        /// </summary>
        public virtual void Dispose() {
            Clear();
            ThreadHelper.InterlockedSet(ref _list, null);
            ThreadHelper.InterlockedSet(ref _provider, null);
        }

        #endregion

        #endregion

    }
}