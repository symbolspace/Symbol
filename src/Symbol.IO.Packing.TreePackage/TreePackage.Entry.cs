/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */

using System;
using System.Text;

namespace Symbol.IO.Packing {
    partial class TreePackage {
        /// <summary>
        /// ����ڶ�Ӧÿһ��ֵ
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
            /// ��ȡ��Ķ�Ӧ�ļ�ֵ��
            /// </summary>
            public string Key {
                get { return _key; }
            }
            /// <summary>
            /// ��ȡ�Ƿ�Ϊ���ģʽ��ֻ�и�Add��������Ż���true��һ����������ͱ��false�ˡ�
            /// </summary>
            public bool IsAdd {
                get { return _isAdd; }
            }
            /// <summary>
            /// ��ȡ������ֵ�����ͣ��������޸�����
            /// </summary>
            public PackageValueTypes ValueType {
                get { return _keyEntry.ValueType; }
                set { _keyEntry.ValueType = value; }
            }
            ///// <summary>
            ///// ��ȡ������ֵ�ı任���ͣ��������޸�����
            ///// </summary>
            //public PackageValueAsTypes ValueAsType {
            //    get { return _keyEntry.ValueAsType; }
            //    set { _keyEntry.ValueAsType = value; }
            //}
            /// <summary>
            /// ��ȡֵ�Ƿ�Ϊ�գ�null����
            /// </summary>
            public bool Nullable {
                get { return _keyEntry.Nullable; }
            }
            /// <summary>
            /// ��ȡ������ֵ���������ͣ������޸�Ϊ��Ҫ�����͡�
            /// </summary>
            public PackageArrayTypes ArrayType {
                get { return _keyEntry.ArrayType; }
                set { _keyEntry.ArrayType = value; }
            }
            /// <summary>
            /// ��ȡ��չ�����б�
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
            /// ��ȡCRC32У���롣
            /// </summary>
            public ulong CRC32 {
                get { return _keyEntry.CRC32; }
            }
            /// <summary>
            /// ��ȡ������ѹ����ʽ��
            /// </summary>
            public PackageCompressTypes CompressType {
                get { return _keyEntry.CompressType; }
                set { _keyEntry.CompressType = value; }
            }

            #region Value
            /// <summary>
            /// ��ȡ���������ֵ��
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

            #region IEntry ��Ա
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

            #region IDisposable ��Ա
            /// <summary>
            /// ���� Entry
            /// </summary>
            ~Entry() {
                Dispose(false);
            }
            /// <summary>
            /// �ͷ�ռ�õ���Դ��
            /// </summary>
            public void Dispose() {
                Dispose(true);
            }
            /// <summary>
            /// �ͷ�ռ�õ���Դ��
            /// </summary>
            /// <param name="disposing">�Ƿ�Ϊ�սᡣ</param>
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