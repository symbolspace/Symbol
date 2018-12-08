/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.IO.Packing {
    partial class TreePackage {
        class EntryList : IEntryList, IDisposable {
            private TreePackage _package;
            #region IEntryList 成员

            public Entry this[string key] {
                get {
                    if (_package.ContainsKey(key))
                        return _package._entries[key];
                    return null;
                }
            }

            #endregion

            public EntryList(TreePackage package) {
                _package = package;
            }

            #region IDisposable 成员

            public void Dispose() {
                if (_package != null)
                    _package = null;
                GC.SuppressFinalize(this);
                //GC.Collect(0);
                //GC.Collect();
            }

            #endregion
        }
    }
}