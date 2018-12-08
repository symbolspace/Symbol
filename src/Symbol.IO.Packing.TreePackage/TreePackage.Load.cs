/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Runtime.InteropServices;

namespace Symbol.IO.Packing {
    partial class TreePackage {
        
        #region fields
        delegate void UnPackageValueSingleDataHandler(KeyEntry keyEntry, Entry entry, byte[] buffer);
        private static readonly System.Collections.Generic.Dictionary<PackageValueTypes, UnPackageValueSingleDataHandler> _unPakcageValueSingleDataHandlers;
        #endregion

        #region metods
        
        #region Load
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="password">密码，为空将会是默认密码。</param>
        /// <returns>返回加载后的树型包实例。</returns>
        public static TreePackage Load(byte[] buffer, byte[] password = null) {
            if (buffer == null || buffer.Length == 0)
                return null;
            using (System.IO.MemoryStream reader = new System.IO.MemoryStream(buffer)) {
                return Load(reader, password);
            }
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="reader">树型包读取器实例</param>
        /// <param name="password">密码，为空将会是默认密码。</param>
        /// <returns>返回加载后的树型包实例。</returns>
        public static unsafe TreePackage Load(System.IO.Stream reader, byte[] password = null) {
            //if (reader == null)
            //    throw new ArgumentNullException("reader", "树型包读取器不能为 null 。");
            if (reader == null)
                return null;
            if (!reader.CanRead)
                throw new ArgumentNullException("reader", "树型包读取器不能进行读取操作。");
            //if (reader.Closed)
            //    throw new ObjectDisposedException("reader", "树型包读取器已经关闭。");

            #region header 1
            byte[] buffer = new byte[8];
            if (reader.Read(buffer, 0, 4) != 4) {//位数不够
                return null;
            }
            //错误的标识
            if (buffer[0] != HeaderEntry.StartFlag[0]
                || buffer[1] != HeaderEntry.StartFlag[1]
                || buffer[2] != HeaderEntry.StartFlag[2]
                || buffer[3] != HeaderEntry.StartFlag[3]) {
                return null;
            }
            HeaderEntry packageEntry = new HeaderEntry();
            packageEntry.Started = true;

            //Version,EncryptType
            if (reader.Read(buffer, 0, 2) != 2)
                return null;
            if (buffer[0] > 9) {
                packageEntry.Version = (byte)(buffer[0] / 10);
                buffer[0] -= (byte)(packageEntry.Version * 10);
            }
            packageEntry.EncryptType = (PackageEncryptTypes)buffer[0];
            //KeyIgnoreCase,HasAttributes,HasComment
            if (buffer[1] >= 100) {
                packageEntry.KeyIgnoreCase = true;
                buffer[1] -= 100;
            }
            if (buffer[1] >= 10) {
                packageEntry.HasAttributes = true;
                buffer[1] -= 10;
            }
            if (buffer[1] == 1) {
                packageEntry.HasComment = true;
            }
            #endregion

            if (password == null || password.Length == 0 || password == _defaultBinaryWavePassword) {
                password = _defaultBinaryWavePassword;
                if (packageEntry.EncryptType != PackageEncryptTypes.BinaryWave_EmptyPassword)
                    packageEntry.EncryptType = PackageEncryptTypes.BinaryWave_DefaultPassword;
            } else {
                if (packageEntry.EncryptType != PackageEncryptTypes.BinaryWave_EmptyPassword)
                    packageEntry.EncryptType = PackageEncryptTypes.BinaryWave_CustomPassword;
            }
            TreePackage result = null;
            using (Symbol.Encryption.BinaryEncryptionStream binaryEncryptionStream = new Symbol.Encryption.BinaryEncryptionStream(reader, password, packageEntry.EncryptType == PackageEncryptTypes.BinaryWave_EmptyPassword)) {
                int int32Var;

                #region header 2
                //AttributesLength
                if (packageEntry.HasAttributes) {
                    if (binaryEncryptionStream.Read(buffer, 0, 4) != 4)
                        return null;
                    Marshal.Copy(buffer, 0, (IntPtr)(&int32Var), 4);
                    packageEntry.AttributesLength = int32Var;
                }
                //CommentLength
                if (packageEntry.HasComment) {
                    if (binaryEncryptionStream.Read(buffer, 0, 4) != 4)
                        return null;
                    Marshal.Copy(buffer, 0, (IntPtr)(&int32Var), 4);
                    packageEntry.CommentLength = int32Var;
                }
                //KeysCount
                if (binaryEncryptionStream.Read(buffer, 0, 4) != 4)
                    return null;
                Marshal.Copy(buffer, 0, (IntPtr)(&int32Var), 4);
                packageEntry.KeysCount = int32Var;

                //AttributesData
                if (packageEntry.HasAttributes) {
                    packageEntry.AttributesData = new byte[packageEntry.AttributesLength];
                    if (binaryEncryptionStream.Read(packageEntry.AttributesData, 0, packageEntry.AttributesLength) != packageEntry.AttributesLength)
                        return null;
                }
                //CommentData
                if (packageEntry.HasComment) {
                    packageEntry.CommentData = new byte[packageEntry.CommentLength];
                    if (binaryEncryptionStream.Read(packageEntry.CommentData, 0, packageEntry.CommentLength) != packageEntry.CommentLength)
                        return null;
                }
                #endregion

                result = new TreePackage(packageEntry);

                #region Keys
                //Keys
                ushort uShortVar = 0;
                ulong uInt64Var = 0;
                KeyEntry keyEntry;
                byte[] bufferValueData = null;
                Entry entry = null;
                for (int keyIndex = 0; keyIndex < packageEntry.KeysCount; keyIndex++) {

                    #region Header
                    if (binaryEncryptionStream.Read(buffer, 0, 5) != 5)
                        return null;
                    //KeyLength
                    Marshal.Copy(buffer, 0, (IntPtr)(&uShortVar), 2);
                    keyEntry = new KeyEntry();
                    keyEntry.KeyLength = uShortVar;
                    if (keyEntry.KeyLength == 0)
                        return null;
                    //ValueType
                    keyEntry.ValueType = (PackageValueTypes)buffer[2];
                    //Nullable|ArrayType
                    if (buffer[3] >= 100) {
                        keyEntry.Nullable = true;
                        buffer[3] -= 100;
                    }
                    keyEntry.ArrayType = (PackageArrayTypes)buffer[3];
                    //HasAttributes,CompressType
                    if (buffer[4] >= 100) {
                        keyEntry.HasAttributes = true;
                        buffer[4] -= 100;
                    }
                    keyEntry.CompressType = (PackageCompressTypes)buffer[4];

                    if (keyEntry.HasAttributes) {//AttributesLength
                        if (binaryEncryptionStream.Read(buffer, 0, 4) != 4)
                            return null;
                        Marshal.Copy(buffer, 0, (IntPtr)(&int32Var), 4);
                        keyEntry.AttributesLength = int32Var;
                    }
                    //ValueDataLength
                    if (!keyEntry.Nullable) {
                        if (binaryEncryptionStream.Read(buffer, 0, 4) != 4)
                            return null;
                        Marshal.Copy(buffer, 0, (IntPtr)(&int32Var), 4);
                        keyEntry.ValueDataLength = int32Var;
                        //CRC32
                        if (binaryEncryptionStream.Read(buffer, 0, 8) != 8)
                            return null;
                        Marshal.Copy(buffer, 0, (IntPtr)(&uInt64Var), 8);
                        keyEntry.CRC32 = uInt64Var;
                    }
                    #endregion

                    #region Data
                    //KeyData
                    keyEntry.KeyData = new byte[keyEntry.KeyLength];
                    if (binaryEncryptionStream.Read(keyEntry.KeyData, 0, keyEntry.KeyLength) != keyEntry.KeyLength)
                        return null;
                    //AttributesData
                    if (keyEntry.HasAttributes) {
                        keyEntry.AttributesData = new byte[keyEntry.AttributesLength];
                        if (binaryEncryptionStream.Read(keyEntry.AttributesData, 0, keyEntry.AttributesLength) != keyEntry.AttributesLength)
                            return null;
                    }
                    entry = new Entry(keyEntry);
                    ((IEntry)entry).SetIsAdd(false);
                    //ValueData
                    if (!keyEntry.Nullable) {
                        bufferValueData = new byte[keyEntry.ValueDataLength];
                        if (keyEntry.CompressType == PackageCompressTypes.NonEncrypt_NonCompress) {
                            if (binaryEncryptionStream.BaseRead(bufferValueData, 0, keyEntry.ValueDataLength) != keyEntry.ValueDataLength)
                                return null;
                        } else {
                            if (binaryEncryptionStream.Read(bufferValueData, 0, keyEntry.ValueDataLength) != keyEntry.ValueDataLength)
                                return null;
                        }
                        uInt64Var = Symbol.Encryption.CRC32EncryptionHelper.Encrypt(bufferValueData);
                        if (uInt64Var != keyEntry.CRC32) {
                            throw new System.IO.InvalidDataException(string.Format("“{0}”的CRC32数据验证失败。应为：{1:X8}，实际：{2:X8}", entry.Key, keyEntry.CRC32, uInt64Var));
                        }
                        UnPackageValueData(keyEntry, entry, bufferValueData, result);
                    } else {
                        entry.Value = null;
                    }
                    #endregion

                    bufferValueData = null;
                    result.Add(entry);
                }
                #endregion
            }

            return result;
        }
        #endregion

        #region UnPackageValueData
        static void UnPackageValueData(KeyEntry keyEntry, Entry entry, byte[] buffer,TreePackage treePackage) {
            if (keyEntry.CompressType != PackageCompressTypes.None) {
                if (!_compressors.ContainsKey(keyEntry.CompressType)) {
                    throw new NotSupportedException("暂不支持“" + keyEntry.CompressType + "”压缩算法。");
                }
                buffer = _compressors[keyEntry.CompressType](buffer, false);
            }
            if (keyEntry.ArrayType == PackageArrayTypes.None) {
                if (!_unPakcageValueSingleDataHandlers.ContainsKey(keyEntry.ValueType))
                    throw new NotSupportedException("暂时不支持“" + keyEntry.ValueType + "”类型。");
                _unPakcageValueSingleDataHandlers[keyEntry.ValueType](keyEntry, entry, buffer);
            } else {
                UnPackageValueArrayData(keyEntry, entry, buffer,treePackage);
            }
        }
        #endregion

        #region UnPackageValueArrayData
        static void UnPackageValueArrayData(KeyEntry keyEntry, Entry entry, byte[] buffer, TreePackage treePackage) {
            switch (keyEntry.ArrayType) {
                case PackageArrayTypes.Dictionary:
                    UnPackageValueDictionary(keyEntry, entry, buffer);
                    break;
                case PackageArrayTypes.NameValueCollection:
                    UnPackageValueNameValueCollection(keyEntry, entry, buffer);
                    break;
                default:
                    UnPackageValueIEnumerable(keyEntry, entry, buffer,treePackage);
                    break;
            }
        }
        #endregion

        #region UnPackageValueNameValueCollection
        static void UnPackageValueNameValueCollection(KeyEntry keyEntry, Entry entry, byte[] buffer) {
            TreePackage package = Load(buffer);
            if (package == null || package.Count == 0) {
                entry.Value = new System.Collections.Specialized.NameValueCollection();//创建实际的数组类型
                return;
            }
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            Entry entry2 = null;
            for (int index = 0; index < package.Count; index++) {
                entry2 = package.Entries[index.ToString()];
                string key = (string)entry2.Attributes["Key"];
                if (entry2.Value != null) {
                    foreach (string item in (string[])entry2.Value) {
                        col.Add(key, item);
                    }
                }
            }
            entry.Value = col;
            package.Clear(false);
            package.Dispose();
            package = null;
        }
        #endregion

        #region CreateTArray
        delegate void UnPackageValueIEnumerableArrayAddHandler(int index, object value);
        delegate System.Collections.IList UnPackageValueIEnumerableCreateArrayHandler(int length);
        static System.Collections.IList CreateTArray(KeyEntry keyEntry, int length, bool list, bool isT,Type type) {
            if (type == null) {
                if (isT) {
                    type = _valueTypes[keyEntry.ValueType];
                } else {
                    type = typeof(object);
                }
            }
            if (list) {
                return (System.Collections.IList)FastWrapper.CreateInstance(typeof(System.Collections.Generic.List<>).MakeGenericType(type), length);
            } else {
                return Array.CreateInstance(type, length);
            }
        }
        #endregion

        #region UnPackageValueIEnumerable
        static void UnPackageValueIEnumerable(KeyEntry keyEntry, Entry entry, byte[] buffer, TreePackage treePackage) {
            if (treePackage.Version == 2) {
                if (entry.ArrayType == PackageArrayTypes.T_Array && entry.ValueType== PackageValueTypes.Byte) {
                    entry.Value = buffer;
                    return;
                }
            }
            TreePackage package = Load(buffer);
            Type t1 = null;
            if (package.Attributes.ContainsKey("T1")) {
                t1 = FastWrapper.GetWarpperType((string)package.Attributes["T1"]);
            }
            UnPackageValueIEnumerableCreateArrayHandler createArrayHandler = null;
            UnPackageValueIEnumerableArrayAddHandler addHandler = null;
            System.Collections.IList array = null;
            Array array2 = null;
            switch (keyEntry.ArrayType) {
                case PackageArrayTypes.T_Array:
                    createArrayHandler = (p1) => array2 = (Array)CreateTArray(keyEntry, p1, false, true,t1);
                    addHandler = (p1, p2) => array2.SetValue(p2, p1);
                    break;
                case PackageArrayTypes.T_List:
                    createArrayHandler = (p1) => CreateTArray(keyEntry, p1, true, true,t1);
                    addHandler = (p1, p2) => array.Add(p2);
                    break;
                case PackageArrayTypes.Object_Array:
                    createArrayHandler = (p1) => array2 = (Array)CreateTArray(keyEntry, p1, false, false,t1);
                    addHandler = (p1, p2) => array2.SetValue(p2, p1);
                    break;
                case PackageArrayTypes.Object_List:
                    createArrayHandler = (p1) => CreateTArray(keyEntry, p1, true, false,t1);
                    addHandler = (p1, p2) => array.Add(p2);
                    break;
                default:
                    throw new NotSupportedException("暂不支持“" + keyEntry.ArrayType + "”类型的数组。");
            }
            if (package == null || package.Count == 0) {
                entry.Value = createArrayHandler(0);//创建实际的数组类型
                return;
            }
            //创建数组
            array = createArrayHandler(package.Count);
            for (int index = 0; index < package.Count; index++) {
                addHandler(index, package[index.ToString()]);
            }
            entry.Value = array;
            package.Clear(false);
            package.Dispose();
            package = null;
        }
        #endregion

        #region UnPackageValueDictionary
        static void UnPackageValueDictionary(KeyEntry keyEntry, Entry entry, byte[] buffer) {
            TreePackage package = Load(buffer);
            if (package == null || package.Count == 0) {
                entry.Value = new System.Collections.Hashtable();
                return;
            }
            bool isGeneric = (bool)package.Attributes["IsGeneric"];
            System.Collections.IDictionary result = null;
            if (isGeneric) {
                Type t1 = FastWrapper.GetWarpperType((string)package.Attributes["T1"]);
                Type t2 = FastWrapper.GetWarpperType((string)package.Attributes["T2"]);
                object comparer = FastWrapper.Get(typeof(System.Collections.Generic.EqualityComparer<>).MakeGenericType(t1), "Default");
                if (package.Attributes.ContainsKey("KeyIsString") && (bool)package.Attributes["KeyIsString"]) {
                    comparer = (bool)package.Attributes["ComparerIgnoreCase"] ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
                }
                result = (System.Collections.IDictionary)FastWrapper.CreateInstance(typeof(System.Collections.Generic.Dictionary<,>).MakeGenericType(t1, t2), package.Count, comparer);
            } else {
                result = new System.Collections.Hashtable(package.Count);
            }

            Entry entry2 = null;
            for (int index = 0; index < package.Count; index++) {
                entry2 = package.Entries[index.ToString()];
                result.Add(entry2.Attributes["Key"], entry2.Value);
            }
            entry.Value = result;
        }
        #endregion

        #endregion
    }
}