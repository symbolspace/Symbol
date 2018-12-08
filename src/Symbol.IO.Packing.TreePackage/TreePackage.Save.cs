/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Symbol.IO.Packing {
    partial class TreePackage {
        
        #region fields
        delegate byte[] PackageValueSingleDataHandler(KeyEntry keyEntry, Entry entry);
        private static readonly System.Collections.Generic.Dictionary<PackageValueTypes, PackageValueSingleDataHandler> _packageValueSingleDataHandlers;
        #endregion

        #region methods

        #region Save
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="password">密码，为空将会是默认密码。</param>
        /// <returns>返回保存后的数据。</returns>
        public byte[] Save(byte[] password = null) {
            using (System.IO.MemoryStream writer = new System.IO.MemoryStream()) {
                Save(writer, password);
                return writer.ToArray();
            }
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="writer">树型包写入器实例</param>
        /// <param name="password">密码，为空将会是默认密码。</param>
        public unsafe void Save(System.IO.Stream writer, byte[] password = null) {
            if (writer == null)
                throw new ArgumentNullException("writer", "树型包写入器不能为 null 。");
            if (!writer.CanWrite)
                throw new ArgumentNullException("writer", "树型包写入器当前不能进行写入操作。");
            if (password == null || password.Length == 0 || password == _defaultBinaryWavePassword) {
                if (_packageEntry.EncryptType != PackageEncryptTypes.BinaryWave_EmptyPassword)
                    _packageEntry.EncryptType = PackageEncryptTypes.BinaryWave_DefaultPassword;
                password = _defaultBinaryWavePassword;
            } else {
                if (_packageEntry.EncryptType != PackageEncryptTypes.BinaryWave_EmptyPassword)
                    _packageEntry.EncryptType = PackageEncryptTypes.BinaryWave_CustomPassword;
            }
            //if (writer.Closed)
            //    throw new ObjectDisposedException("writer", "树型包写入器已经关闭。");

            #region header 1
            //StartFlag
            writer.Write(HeaderEntry.StartFlag, 0, HeaderEntry.StartFlag.Length);
            byte[] buffer = new byte[8];
            //Version,EncryptType
            buffer[0] = (byte)(_packageEntry.Version * 10);
            buffer[0] += (byte)_packageEntry.EncryptType;

            //KeyIgnoreCase,HasAttributes,HasComment
            if (_packageEntry.KeyIgnoreCase)
                buffer[1] = 100;
            else
                buffer[1] = 0;
            _packageEntry.HasAttributes = _attributes != null && _attributes.Count > 0;
            if (_packageEntry.HasAttributes) {
                buffer[1] += 10;
            }
            _packageEntry.HasComment = !string.IsNullOrEmpty(_comment);
            if (_packageEntry.HasComment)
                buffer[1] += 1;
            writer.Write(buffer, 0, 2);

            #endregion

            using (Symbol.Encryption.BinaryEncryptionStream binaryEncryptionStream = new Symbol.Encryption.BinaryEncryptionStream(writer, password, EncryptType == PackageEncryptTypes.BinaryWave_EmptyPassword)) {
                int int32Var;

                #region header 2
                //AttributesLength
                if (_packageEntry.HasAttributes) {
                    if(_packageEntry.EncryptType== PackageEncryptTypes.BinaryWave_EmptyPassword)
                        _attributes.EncryptType = _packageEntry.EncryptType;
                    _packageEntry.AttributesData = _attributes.Save();
                    _packageEntry.AttributesLength = _packageEntry.AttributesData.Length;
                    int32Var = _packageEntry.AttributesLength;
                    Marshal.Copy((IntPtr)(&int32Var), buffer, 0, 4);
                    binaryEncryptionStream.Write(buffer, 0, 4);
                }

                //CommentLength
                if (_packageEntry.HasComment) {
                    _packageEntry.CommentData = System.Text.Encoding.UTF8.GetBytes(_comment);
                    _packageEntry.CommentLength = _packageEntry.CommentData.Length;
                    int32Var = _packageEntry.CommentLength;
                    Marshal.Copy((IntPtr)(&int32Var), buffer, 0, 4);
                    binaryEncryptionStream.Write(buffer, 0, 4);
                }

                //KeysCount
                int32Var = _entries.Count;
                Marshal.Copy((IntPtr)(&int32Var), buffer, 0, 4);
                binaryEncryptionStream.Write(buffer, 0, 4);

                //AttributesData
                if (_packageEntry.HasAttributes) {
                    binaryEncryptionStream.Write(_packageEntry.AttributesData, 0, _packageEntry.AttributesData.Length);
                }
                //CommentData
                if (_packageEntry.HasComment) {
                    binaryEncryptionStream.Write(_packageEntry.CommentData, 0, _packageEntry.CommentData.Length);
                }
                #endregion

                //header clear
                _packageEntry.AttributesData = null;
                _packageEntry.AttributesLength = 0;
                _packageEntry.CommentData = null;
                _packageEntry.CommentLength = 0;

                #region keys
                IEntry entry2;
                KeyEntry keyEntry;
                ushort uShortVar = 0;
                ulong uInt64Var = 0;
                byte[] bufferValueData = null;
                //Keys
                foreach (System.Collections.Generic.KeyValuePair<string, Entry> item in _entries) {
                    entry2 = item.Value;
                    keyEntry = entry2.KeyEntry;
                    keyEntry.KeyData = System.Text.Encoding.UTF8.GetBytes(item.Key);
                    uShortVar = keyEntry.KeyLength = (ushort)keyEntry.KeyData.Length;
                    //0 1 ,KeyLength ushort
                    Marshal.Copy((IntPtr)(&uShortVar), buffer, 0, 2);
                    //ValueType,byte ,1
                    buffer[2] = (byte)keyEntry.ValueType;
                    //Nullable,ArrayType
                    if (keyEntry.Nullable)
                        buffer[3] = 100;
                    else
                        buffer[3] = 0;
                    buffer[3] += (byte)keyEntry.ArrayType;
                    //HasAttributes,CompressType
                    keyEntry.HasAttributes = item.Value.Attributes != null && item.Value.Attributes.Count > 0;
                    if (keyEntry.HasAttributes) {
                        buffer[4] = 100;
                        if (_packageEntry.EncryptType == PackageEncryptTypes.BinaryWave_EmptyPassword)
                            item.Value.Attributes.EncryptType = _packageEntry.EncryptType;
                        keyEntry.AttributesData = item.Value.Attributes.Save();
                        keyEntry.AttributesLength = keyEntry.AttributesData.Length;
                    } else {
                        buffer[4] = 0;
                    }
                    buffer[4] += (byte)keyEntry.CompressType;
                    binaryEncryptionStream.Write(buffer, 0, 5);
                    if (keyEntry.HasAttributes) {//AttributesLength
                        int32Var = keyEntry.AttributesLength;
                        Marshal.Copy((IntPtr)(&int32Var), buffer, 0, 4);
                        binaryEncryptionStream.Write(buffer, 0, 4);
                    }
                    //ValueDataLength
                    if (!keyEntry.Nullable) {
                        bufferValueData = PackageValueData(keyEntry, item.Value);
                        int32Var = keyEntry.ValueDataLength = bufferValueData.Length;
                        Marshal.Copy((IntPtr)(&int32Var), buffer, 0, 4);
                        binaryEncryptionStream.Write(buffer, 0, 4);
                        //CRC32
                        uInt64Var = keyEntry.CRC32 = Symbol.Encryption.CRC32EncryptionHelper.Encrypt(bufferValueData);
                        Marshal.Copy((IntPtr)(&uInt64Var), buffer, 0, 8);
                        binaryEncryptionStream.Write(buffer, 0, 8);
                    }


                    //KeyData
                    binaryEncryptionStream.Write(keyEntry.KeyData, 0, keyEntry.KeyLength);
                    //AttributesData
                    if (keyEntry.HasAttributes) {
                        binaryEncryptionStream.Write(keyEntry.AttributesData, 0, keyEntry.AttributesLength);
                    }
                    //ValueData
                    if (!keyEntry.Nullable) {
                        //
                        if (keyEntry.CompressType == PackageCompressTypes.NonEncrypt_NonCompress) {
                            binaryEncryptionStream.BaseWrite(bufferValueData, 0, keyEntry.ValueDataLength);
                        } else {
                            binaryEncryptionStream.Write(bufferValueData, 0, keyEntry.ValueDataLength);
                        }
                    }
                    bufferValueData = null;
                    keyEntry.AttributesData = null;
                    keyEntry.KeyData = null;
                    entry2 = null;
                }
                #endregion
            }
        }
        #endregion

        #region PackageValueData
        byte[] PackageValueData(KeyEntry keyEntry, Entry entry) {
            byte[] result = null;
            if (keyEntry.ArrayType == PackageArrayTypes.None) {
                if (!_packageValueSingleDataHandlers.ContainsKey(keyEntry.ValueType))
                    throw new NotSupportedException("暂时不支持“" + keyEntry.ValueType + "”类型。");
                result = _packageValueSingleDataHandlers[keyEntry.ValueType](keyEntry, entry);
            } else {
                result = PackageValueArrayData(keyEntry, entry);
            }
            if (keyEntry.CompressType != PackageCompressTypes.None) {
                if (!_compressors.ContainsKey(keyEntry.CompressType)) {
                    throw new NotSupportedException("暂不支持“" + keyEntry.CompressType + "”压缩算法。");
                }
                result = _compressors[keyEntry.CompressType](result, true);
            }
            return result;
        }
        #endregion

        #region PackageValueArrayData
        byte[] PackageValueArrayData(KeyEntry keyEntry, Entry entry) {
            Type type = entry.Value.GetType();
            switch (keyEntry.ArrayType) {
                case PackageArrayTypes.Dictionary:
                    return PackageValueDictionary(keyEntry, entry, type);
                case PackageArrayTypes.NameValueCollection:
                    return PackageValueNameValueCollection(keyEntry, entry, type);
                default:
                    return PackageValueIEnumerable(keyEntry, entry, type);
            }
        }
        #endregion

        #region PackageValueNameValueCollection
        byte[] PackageValueNameValueCollection(KeyEntry keyEntry, Entry entry, Type type) {
            TreePackage package = new TreePackage();
            System.Collections.Specialized.NameValueCollection col = (System.Collections.Specialized.NameValueCollection)entry.Value;
            for (int i = 0; i < col.Count; i++) {
                package.Add(i.ToString(), col.GetValues(i));
                package.Entries[i.ToString()].Attributes.Add("Key", col.GetKey(i));
            }
            return package.Save();
        }
        #endregion

        #region PackageValueDictionary
        byte[] PackageValueDictionary(KeyEntry keyEntry, Entry entry, Type type) {
            bool isGeneric = IsTheTypes(type, typeof(System.Collections.Generic.IDictionary<,>));
            TreePackage package = new TreePackage();
            package.Attributes.Add("IsGeneric", isGeneric);
            if (isGeneric) {
                Type[] types = GetTheTypes(type, typeof(System.Collections.Generic.IDictionary<,>)).GetGenericArguments();
                package.Attributes.Add("T1", types[0].AssemblyQualifiedName);
                package.Attributes.Add("T2", types[1].AssemblyQualifiedName);
                Type typeX=GetTheTypes(type, typeof(System.Collections.Generic.Dictionary<,>));
                if (typeX !=null && typeX.GetGenericArguments()[0] == typeof(string)) {
                    package.Attributes.Add("KeyIsString", true);
                    System.Collections.Generic.IEqualityComparer<string> comparer = (System.Collections.Generic.IEqualityComparer<string>)FastWrapper.Get(entry.Value,"Comparer");
                    package.Attributes.Add("ComparerIgnoreCase", (comparer == StringComparer.CurrentCultureIgnoreCase
                        || comparer == StringComparer.InvariantCultureIgnoreCase
                        || comparer == StringComparer.OrdinalIgnoreCase));
                }
            }
            int index = -1;
            foreach (object item in (System.Collections.IEnumerable)entry.Value) {
                index++;
                package.Add(index.ToString(), FastWrapper.Get(item, "Value"));
                Entry itemEntry = package.Entries[index.ToString()];
                itemEntry.Attributes.Add("Key", FastWrapper.Get(item, "Key"));
            }
            return package.Save();
        }
        #endregion

        #region PackageValueIEnumerable
        byte[] PackageValueIEnumerable(KeyEntry keyEntry, Entry entry, Type type) {
            if (Version == 2) {
                if (entry.ArrayType == PackageArrayTypes.T_Array && entry.ValueType== PackageValueTypes.Byte) {
                    byte[] buffer1 = entry.Value as byte[];
                    if (buffer1 != null)
                        return buffer1;
                    IEnumerable<byte> buffer2 = entry.Value as IEnumerable<byte>;
                    if (buffer2 != null) {
                        buffer1= LinqHelper.ToArray(buffer2);
                        if (buffer1 == null)
                            return new byte[0];
                        return buffer1;
                    }
                    IEnumerator<byte> buffer3 = entry.Value as IEnumerator<byte>;
                    if (buffer3 != null) {
                        using (buffer3) {
                            List<byte> buffer3_x = new List<byte>();
                            while (buffer3.MoveNext()) {
                                buffer3_x.Add(buffer3.Current);
                            }
                            return buffer3_x.ToArray();
                        }
                    }
                    return new byte[0];
                }
            }
            TreePackage package = new TreePackage();
            if (entry.ArrayType == PackageArrayTypes.T_Array || entry.ArrayType == PackageArrayTypes.T_List) {
                if (entry.ValueType == PackageValueTypes.Object) {
                    Type t1 = null;
                    if (entry.ArrayType == PackageArrayTypes.T_Array) {
                        t1 = entry.Value.GetType().GetElementType();
                    } else {
                        t1 = entry.Value.GetType();
                        if (t1.GetGenericArguments().Length == 0) {
                            t1 = null;
                        } else {
                            t1 = t1.GetGenericArguments()[0];
                        }
                    }
                    if (t1 != null)
                        package.Attributes.Add("T1", t1.AssemblyQualifiedName);
                }
            }
            int index = -1;
            if (IsTheTypes(type, typeof(System.Collections.IEnumerable))) {
                foreach (object item in (System.Collections.IEnumerable)entry.Value) {
                    index++;
                    package.Add(index.ToString(), item);
                }
            } else {
                System.Collections.IEnumerator enumerator = (System.Collections.IEnumerator)entry.Value;
                System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>();
                while (enumerator.MoveNext()) {
                    index++;
                    package.Add(index.ToString(), enumerator.Current);
                    list.Add(enumerator.Current);
                }
                IDisposable disp = enumerator as IDisposable;
                if (disp != null)
                    disp.Dispose();
                disp = null;
                enumerator = null;
                entry.Value = list;
            }
            return package.Save();
        }
        #endregion

        #endregion
    }
}