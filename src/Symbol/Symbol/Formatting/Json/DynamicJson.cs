/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
#if !net20 && !net35

using System;
using System.Collections;
using System.Collections.Generic;

namespace Symbol.Formatting.Json {
    internal class DynamicJson : System.Dynamic.DynamicObject, IEnumerable {

        #region fields
        private IDictionary<string, object> _dictionary { get; set; }
        private List<object> _list { get; set; }
        #endregion

        #region ctor
        public DynamicJson(string json) {
            var parse = JSON.Parse(json);

            if (parse is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)parse;
            else
                _list = (List<object>)parse;
        }

        private DynamicJson(object dictionary) {
            if (dictionary is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)dictionary;
        }
        #endregion

        #region methods

        #region GetDynamicMemberNames
        public override IEnumerable<string> GetDynamicMemberNames() {
            return LinqHelper.ToList(_dictionary.Keys);
        }
        #endregion
        #region TryGetIndex
        public override bool TryGetIndex(System.Dynamic.GetIndexBinder binder, Object[] indexes, out Object result) {
            var index = indexes[0];
            if (index is int) {
                result = _list[(int)index];
            } else {
                result = _dictionary[(string)index];
            }
            if (result is IDictionary<string, object>)
                result = new DynamicJson(result as IDictionary<string, object>);
            return true;
        }
        #endregion
        #region TryGetMember
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result) {
            if (_dictionary.TryGetValue(binder.Name, out result) == false)
                if (_dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                    return false;// throw new Exception("property not found " + binder.Name);

            if (result is IDictionary<string, object>) {
                result = new DynamicJson(result as IDictionary<string, object>);
            } else if (result is List<object>) {
                List<object> list = new List<object>();
                foreach (object item in (List<object>)result) {
                    if (item is IDictionary<string, object>)
                        list.Add(new DynamicJson(item as IDictionary<string, object>));
                    else
                        list.Add(item);
                }
                result = list;
            }

            return _dictionary.ContainsKey(binder.Name);
        }
        #endregion
        #region GetEnumerator
        IEnumerator IEnumerable.GetEnumerator() {
            foreach (var o in _list) {
                yield return new DynamicJson(o as IDictionary<string, object>);
            }
        }
        #endregion

        #endregion
    }

}
#endif
