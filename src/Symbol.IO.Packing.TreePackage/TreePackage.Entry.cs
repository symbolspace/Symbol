/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;
using System.Text;

namespace Symbol.IO.Packing {
    partial class TreePackage {
        /// <summary>
        /// 项，用于对应每一项值
        /// </summary>
        [System.Diagnostics.DebuggerDisplay("Key={Key},Value={Value},{ValueType},{ArrayType},{CompressType},{CRC32}")]
        public class Entry : IEntry, IDisposable {

            #region fields
            private KeyEntry _keyEntry;
            private string _key;
            private bool _isAdd;
            private object _value;
            private TreePackage _attributes;

            #endregion

            #region properties
            /// <summary>
            /// 获取项的对应的键值。
            /// </summary>
            public string Key {
                get { return _key; }
            }
            /// <summary>
            /// 获取是否为添加模式，只有刚Add进来的项才会是true，一旦保存后，它就变成false了。
            /// </summary>
            public bool IsAdd {
                get { return _isAdd; }
            }
            /// <summary>
            /// 获取或设置值的类型，不建议修改它。
            /// </summary>
            public PackageValueTypes ValueType {
                get { return _keyEntry.ValueType; }
                set { _keyEntry.ValueType = value; }
            }
            ///// <summary>
            ///// 获取或设置值的变换类型，不建议修改它。
            ///// </summary>
            //public PackageValueAsTypes ValueAsType {
            //    get { return _keyEntry.ValueAsType; }
            //    set { _keyEntry.ValueAsType = value; }
            //}
            /// <summary>
            /// 获取值是否为空（null）。
            /// </summary>
            public bool Nullable {
                get { return _keyEntry.Nullable; }
            }
            /// <summary>
            /// 获取或设置值的数组类型，可以修改为将要的类型。
            /// </summary>
            public PackageArrayTypes ArrayType {
                get { return _keyEntry.ArrayType; }
                set { _keyEntry.ArrayType = value; }
            }
            /// <summary>
            /// 获取扩展属性列表。
            /// </summary>
            public TreePackage Attributes {
                get {
                    if (_attributes == null) {
                        if (_keyEntry.HasAttributes) {
                            if (_keyEntry.AttributesLength == 0 || _keyEntry.AttributesData == null || _keyEntry.AttributesData.Length == 0) {
                                _keyEntry.HasAttributes = false;
                                _keyEntry.AttributesData = null;
                                _keyEntry.AttributesLength = 0;
                                _attributes = new TreePackage();
                            } else {
                                _attributes = Load(_keyEntry.AttributesData);
                                if (_attributes == null)
                                    _attributes = new TreePackage();
                                _keyEntry.AttributesData = null;
                                _keyEntry.AttributesLength = 0;
                            }
                        } else {
                            _keyEntry.AttributesData = null;
                            _keyEntry.AttributesLength = 0;
                            _attributes = new TreePackage(false);
                        }
                    }
                    return _attributes;
                }
            }
            /// <summary>
            /// 获取CRC32校验码。
            /// </summary>
            public ulong CRC32 {
                get { return _keyEntry.CRC32; }
            }
            /// <summary>
            /// 获取或设置压缩方式。
            /// </summary>
            public PackageCompressTypes CompressType {
                get { return _keyEntry.CompressType; }
                set { _keyEntry.CompressType = value; }
            }

            #region Value
            /// <summary>
            /// 获取或设置项的值。
            /// </summary>
            public object Value {
                get {
                    return _value;
                }
                set {
                    if (value == null) {
                        _keyEntry.Nullable = true;
                        _value = null;
                    } else if (value is DBNull || ((value as DBNull) == DBNull.Value)) {
                        _keyEntry.Nullable = true;
                        _value = null;
                    } else {
                        _keyEntry.Nullable = false;
                        _value = value;
                    }
                }
            }
            #endregion

            #endregion

            #region ctor
            internal Entry(string key, KeyEntry keyEntry) {
                _keyEntry = keyEntry;
                _key = key;
                _isAdd = true;
            }
            internal Entry(KeyEntry keyEntry) {
                _keyEntry = keyEntry;
                _key = Encoding.UTF8.GetString(keyEntry.KeyData);
                _keyEntry.KeyData = null;
                _isAdd = true;
            }
            #endregion

            #region IEntry 成员
            void IEntry.SetKey(string value) {
                _key = value;
                //_keyEntry.KeyData = Encoding.UTF8.GetBytes(value);
                //_keyEntry.KeyLength = (ushort)_keyEntry.KeyData.Length;
            }
            void IEntry.SetIsAdd(bool value) {
                _isAdd = value;
            }

            KeyEntry IEntry.KeyEntry {
                get { return _keyEntry; }
                set { _keyEntry = value; }
            }

            #endregion

            #region IDisposable 成员
            /// <summary>
            /// 析构 Entry
            /// </summary>
            ~Entry() {
                Dispose(false);
            }
            /// <summary>
            /// 释放占用的资源。
            /// </summary>
            public void Dispose() {
                Dispose(true);
            }
            /// <summary>
            /// 释放占用的资源。
            /// </summary>
            /// <param name="disposing">是否为终结。</param>
            protected void Dispose(bool disposing) {
                if (disposing) {
                    if (_key != null) {
                        _key = null;
                    }
                    if (_value != null)
                        _value = null;
                    if (_attributes != null) {
                        _attributes.Dispose();
                        _attributes = null;
                    }
                    GC.SuppressFinalize(this);
                    //GC.Collect(0);
                    //GC.Collect();
                }
            }
            #endregion
        }
    }
}