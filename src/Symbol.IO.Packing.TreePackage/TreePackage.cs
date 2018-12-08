/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Symbol.IO.Packing {

    /// <summary>
    /// 树型包
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Count={Count},Comment={Comment}")]
    public partial class TreePackage : IEnumerable<TreePackage.Entry>, IDisposable {

        #region fields
        /// <summary>
        /// 当前最大版本号。
        /// </summary>
        public const byte MaxVersion = 2;
        private Dictionary<string, Entry> _entries;
        private HeaderEntry _packageEntry;
        private EntryList _entryList;
        private string _comment;
        private TreePackage _attributes;
        private static readonly Dictionary<TypeCode, PackageValueTypes> _typeCodes;
        private static readonly Dictionary<Type, PackageValueTypes> _typeValueTypes;
        private static readonly Dictionary<PackageValueTypes, Type> _valueTypes;
        private static readonly Dictionary<PackageCompressTypes, CompressorHandler> _compressors;
        //private static readonly Dictionary<Type, ICustomPackage> _customPackages;
        private static readonly byte[] _defaultBinaryWavePassword = new byte[] { 
           //00   01   02   03   04   05   06   07   08   09   10   11   12   13   14   15
            033, 178, 238, 022, 017, 199, 168, 009, 022, 094, 064, 212, 222, 208, 004, 143
        };
        #endregion

        #region properties
        /// <summary>
        /// 获取当前树型包的核心版本号。
        /// </summary>
        public byte Version {
            get { return _packageEntry.Version; }
        }
        /// <summary>
        /// 获取树型包的键值是否忽略大小写。
        /// </summary>
        public bool KeyIgnoreCase { 
            get { return _packageEntry.KeyIgnoreCase; } 
        }
        /// <summary>
        /// 获取项列表。
        /// </summary>
        public IEntryList Entries {
            get {
                if (_entryList == null) {
                    _entryList = new EntryList(this);
                }
                return _entryList;
            }
        }
        /// <summary>
        /// 当前键值数量。
        /// </summary>
        public int Count { 
            get { return _entries.Count; }
        }
        /// <summary>
        /// 获取所有的键值
        /// </summary>
        public IEnumerable<string> Keys {
            get { return _entries.Keys; }
        }

        #region Comment
        /// <summary>
        /// 获取或设置备注信息。
        /// </summary>
        public string Comment {
            get {
                return _comment;
            }
            set {
                _comment = value;
                _packageEntry.CommentData = null;
                _packageEntry.CommentLength = 0;
                //if (string.IsNullOrEmpty(value)) {
                //    _packageEntry.HasComment = false;
                //} else {
                //    _packageEntry.HasComment = true;
                //    _packageEntry.CommentData = Encoding.UTF8.GetBytes(value);
                //    _packageEntry.CommentLength = (uint)_packageEntry.CommentData.Length;
                //}
            }
        }
        #endregion

        #region Attributes
        /// <summary>
        /// 获取扩展属性列表。
        /// </summary>
        public TreePackage Attributes {
            get {
                if (_attributes == null) {
                    if (_packageEntry.HasAttributes) {
                        if (_packageEntry.AttributesLength == 0 || _packageEntry.AttributesData == null || _packageEntry.AttributesData.Length == 0) {
                            _packageEntry.HasAttributes = false;
                            _packageEntry.AttributesData = null;
                            _packageEntry.AttributesLength = 0;
                            _attributes = new TreePackage();
                        } else {
                            _attributes = Load(_packageEntry.AttributesData);
                            if (_attributes == null)
                                _attributes = new TreePackage();
                            _packageEntry.AttributesData = null;
                            _packageEntry.AttributesLength = 0;
                        }
                    } else {
                        _packageEntry.AttributesData = null;
                        _packageEntry.AttributesLength = 0;
                        _attributes = new TreePackage(false);
                    }
                }
                return _attributes;
            }
        }
        #endregion

        #region EncryptType
        /// <summary>
        /// 获取或设置加密类型
        /// </summary>
        public PackageEncryptTypes EncryptType {
            get {
                return _packageEntry.EncryptType;
            }
            set {
                _packageEntry.EncryptType = value;
            }
        }
        #endregion

        #endregion

        #region ctor
        /// <summary>
        /// 预加载，提前让树型包引擎初始化，后期使用将会很快。
        /// </summary>
        public static void Startup() {
        }
        unsafe static TreePackage() {

            #region typecode map ValueType
            _typeCodes = new Dictionary<TypeCode, PackageValueTypes>();
            _typeCodes.Add(TypeCode.Boolean, PackageValueTypes.Boolean);
            _typeCodes.Add(TypeCode.Byte, PackageValueTypes.Byte);
            _typeCodes.Add(TypeCode.Char, PackageValueTypes.Char);
            _typeCodes.Add(TypeCode.Int16, PackageValueTypes.Int16);
            _typeCodes.Add(TypeCode.Int32, PackageValueTypes.Int32);
            _typeCodes.Add(TypeCode.Int64, PackageValueTypes.Int64);
            _typeCodes.Add(TypeCode.Single, PackageValueTypes.Single);
            _typeCodes.Add(TypeCode.Double, PackageValueTypes.Double);
            _typeCodes.Add(TypeCode.SByte, PackageValueTypes.SByte);
            _typeCodes.Add(TypeCode.Decimal, PackageValueTypes.Decimal);
            _typeCodes.Add(TypeCode.UInt16, PackageValueTypes.UInt16);
            _typeCodes.Add(TypeCode.UInt32, PackageValueTypes.UInt32);
            _typeCodes.Add(TypeCode.UInt64, PackageValueTypes.UInt64);
            _typeCodes.Add(TypeCode.DateTime, PackageValueTypes.DateTime);
            _typeCodes.Add(TypeCode.String, PackageValueTypes.String);
            _typeCodes.Add(TypeCode.Object, PackageValueTypes.Object);
            _typeCodes.Add(TypeCode.DBNull, PackageValueTypes.Object);
            _typeCodes.Add(TypeCode.Empty, PackageValueTypes.Object);
            #endregion

            #region type map ValueType
            _typeValueTypes = new Dictionary<Type, PackageValueTypes>();
            _typeValueTypes.Add(typeof(string), PackageValueTypes.String);
            _typeValueTypes.Add(typeof(IntPtr), PackageValueTypes.IntPtr);
            _typeValueTypes.Add(typeof(UIntPtr), PackageValueTypes.UIntPtr);
            _typeValueTypes.Add(typeof(TimeSpan), PackageValueTypes.TimeSpan);
            _typeValueTypes.Add(typeof(Guid), PackageValueTypes.Guid);
            _typeValueTypes.Add(typeof(System.Drawing.Icon), PackageValueTypes.Icon);
            _typeValueTypes.Add(typeof(System.Drawing.Color), PackageValueTypes.Color);
            _typeValueTypes.Add(typeof(System.Drawing.Image), PackageValueTypes.Image);
            _typeValueTypes.Add(typeof(TreePackage), PackageValueTypes.NestedPackage);
            _typeValueTypes.Add(typeof(Stream), PackageValueTypes.Stream);
            #endregion

            #region valueType map Type
            _valueTypes = new Dictionary<PackageValueTypes, Type>();
            _valueTypes.Add(PackageValueTypes.Boolean, typeof(bool));
            _valueTypes.Add(PackageValueTypes.Byte, typeof(byte));
            _valueTypes.Add(PackageValueTypes.Char, typeof(char));
            _valueTypes.Add(PackageValueTypes.Int16, typeof(short));
            _valueTypes.Add(PackageValueTypes.Int32, typeof(int));
            _valueTypes.Add(PackageValueTypes.Int64, typeof(long));
            _valueTypes.Add(PackageValueTypes.Single, typeof(float));
            _valueTypes.Add(PackageValueTypes.Double, typeof(double));
            _valueTypes.Add(PackageValueTypes.SByte, typeof(sbyte));
            _valueTypes.Add(PackageValueTypes.Decimal, typeof(decimal));
            _valueTypes.Add(PackageValueTypes.UInt16, typeof(ushort));
            _valueTypes.Add(PackageValueTypes.UInt32, typeof(uint));
            _valueTypes.Add(PackageValueTypes.UInt64, typeof(ulong));
            _valueTypes.Add(PackageValueTypes.IntPtr, typeof(IntPtr));
            _valueTypes.Add(PackageValueTypes.UIntPtr, typeof(UIntPtr));
            _valueTypes.Add(PackageValueTypes.DateTime, typeof(DateTime));
            _valueTypes.Add(PackageValueTypes.TimeSpan, typeof(TimeSpan));
            _valueTypes.Add(PackageValueTypes.String, typeof(string));
            _valueTypes.Add(PackageValueTypes.Stream, typeof(Stream));
            _valueTypes.Add(PackageValueTypes.Icon, typeof(System.Drawing.Icon));
            _valueTypes.Add(PackageValueTypes.Image, typeof(System.Drawing.Image));
            _valueTypes.Add(PackageValueTypes.Color, typeof(System.Drawing.Color));
            _valueTypes.Add(PackageValueTypes.NestedPackage, typeof(TreePackage));
            _valueTypes.Add(PackageValueTypes.Object, typeof(object));
            #endregion

            #region compressor map handlers
            _compressors = new Dictionary<PackageCompressTypes, CompressorHandler>();
            _compressors.Add(PackageCompressTypes.QuickLZ, Compressor_QuickLZ);
            _compressors.Add(PackageCompressTypes.Gzip, Compressor_Gzip);
            _compressors.Add(PackageCompressTypes.Lzma7z, Compressor_Lzma);
            //_compressors.Add(PackageCompressTypes.Zlibwapi, (buffer, compress) => compress ? Symbol.IO.Compression.ZlibwapiHelper.Compress(buffer) : Symbol.IO.Compression.ZlibwapiHelper.Decompress(buffer));
            _compressors.Add(PackageCompressTypes.NonEncrypt_NonCompress, Compressor_Empty);
            #endregion

            #region valueType map package SingleData Handler
            _packageValueSingleDataHandlers = new Dictionary<PackageValueTypes, PackageValueSingleDataHandler>();
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Boolean, (keyEntry, entry) => {
                byte[] result = new byte[1];
                if ((bool)entry.Value)
                    result[0] = 1;
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Byte, (keyEntry, entry) => new byte[] { (byte)entry.Value });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Char, (keyEntry, entry) => {
                byte[] result = new byte[2];
                char tChar = (char)entry.Value;
                Marshal.Copy((IntPtr)(&tChar), result, 0, 2);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Int16, (keyEntry, entry) => {
                short tShort = TypeExtensions.Convert<short>(entry.Value, 0);
                byte[] result = new byte[2];
                Marshal.Copy((IntPtr)(&tShort), result, 0, 2);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Int32, (keyEntry, entry) => {
                int tInt32 = TypeExtensions.Convert<int>(entry.Value, 0);
                byte[] result = new byte[4];
                Marshal.Copy((IntPtr)(&tInt32), result, 0, 4);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Int64, (keyEntry, entry) => {
                long tInt64 = TypeExtensions.Convert<long>(entry.Value, 0L);
                byte[] result = new byte[8];
                Marshal.Copy((IntPtr)(&tInt64), result, 0, 8);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Single, (keyEntry, entry) => {
                float tSingle = TypeExtensions.Convert<float>(entry.Value, 0F);
                byte[] result = new byte[4];
                Marshal.Copy((IntPtr)(&tSingle), result, 0, 4);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Double, (keyEntry, entry) => {
                double tDouble = TypeExtensions.Convert<double>(entry.Value, 0D);
                byte[] result = new byte[8];
                Marshal.Copy((IntPtr)(&tDouble), result, 0, 8);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.SByte, (keyEntry, entry) => {
                sbyte tSByte = TypeExtensions.Convert<sbyte>(entry.Value, 0);
                byte[] result = new byte[1];
                Marshal.Copy((IntPtr)(&tSByte), result, 0, 1);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Decimal, (keyEntry, entry) => Encoding.ASCII.GetBytes(entry.Value.ToString()));
            _packageValueSingleDataHandlers.Add(PackageValueTypes.UInt16, (keyEntry, entry) => {
                ushort tUShort = TypeExtensions.Convert<ushort>(entry.Value, 0);
                byte[] result = new byte[2];
                Marshal.Copy((IntPtr)(&tUShort), result, 0, 2);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.UInt32, (keyEntry, entry) => {
                uint tUInt32 = TypeExtensions.Convert<uint>(entry.Value, 0);
                byte[] result = new byte[4];
                Marshal.Copy((IntPtr)(&tUInt32), result, 0, 4);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.UInt64, (keyEntry, entry) => {
                ulong tUInt64 = TypeExtensions.Convert<ulong>(entry.Value, 0L);
                byte[] result = new byte[8];
                Marshal.Copy((IntPtr)(&tUInt64), result, 0, 8);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.IntPtr, (keyEntry, entry) => {
                IntPtr tIntPtr = (IntPtr)entry.Value;
                byte[] result = new byte[IntPtr.Size];
                if (IntPtr.Size == 4) {
                    int tIntPtr2 = tIntPtr.ToInt32();
                    Marshal.Copy((IntPtr)(&tIntPtr2), result, 0, 4);
                } else {
                    long tIntPtr3 = tIntPtr.ToInt64();
                    Marshal.Copy((IntPtr)(&tIntPtr3), result, 0, 8);
                }
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.UIntPtr, (keyEntry, entry) => {
                UIntPtr tUIntPtr = (UIntPtr)entry.Value;
                byte[] result = new byte[UIntPtr.Size];
                if (UIntPtr.Size == 4) {
                    uint tUIntPtr2 = tUIntPtr.ToUInt32();
                    Marshal.Copy((IntPtr)(&tUIntPtr2), result, 0, 4);
                } else {
                    ulong tUIntPtr3 = tUIntPtr.ToUInt64();
                    Marshal.Copy((IntPtr)(&tUIntPtr3), result, 0, 8);
                }
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.DateTime, (keyEntry, entry) => {
                return Encoding.ASCII.GetBytes( ((DateTime)entry.Value).ToString("yyyy-MM-dd HH:mm:ss"));
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.TimeSpan, (keyEntry, entry) => {
                double tTimeSpan = ((TimeSpan)entry.Value).TotalMilliseconds;
                byte[] result = new byte[8];
                Marshal.Copy((IntPtr)(&tTimeSpan), result, 0, 8);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.String, (keyEntry, entry) => Encoding.UTF8.GetBytes((string)entry.Value));
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Stream, (keyEntry, entry) => {
                MemoryStream memoryStream = entry.Value as MemoryStream;
                if (memoryStream != null) {
                    return memoryStream.ToArray();
                }
                FileStream fileStream = entry.Value as FileStream;
                if (fileStream != null) {
                    fileStream.Position = 0;
                    byte[] result = new byte[fileStream.Length];
                    fileStream.Read(result, 0, result.Length);
                    return result;
                }
                using (memoryStream = new MemoryStream()) {
                    StreamExtensions.CopyTo(((Stream)entry.Value),memoryStream, true);
                    return memoryStream.ToArray();
                }
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Icon, (keyEntry, entry) => {
                using (MemoryStream memoryStream = new MemoryStream()) {
                    ((System.Drawing.Icon)entry.Value).Save(memoryStream);
                    return memoryStream.ToArray();
                }
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Image, (keyEntry, entry) => {
                using (MemoryStream memoryStream = new MemoryStream()) {
                    System.Drawing.Image p= (System.Drawing.Image)entry.Value;
                    p.Save(memoryStream, p.RawFormat);
                    return memoryStream.ToArray();
                }
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Color, (keyEntry, entry) => {
                int tColor = ((System.Drawing.Color)entry.Value).ToArgb();
                byte[] result = new byte[4];
                Marshal.Copy((IntPtr)(&tColor), result, 0, 4);
                return result;
            });
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Guid, (keyEntry, entry) =>((Guid)entry.Value).ToByteArray());
            _packageValueSingleDataHandlers.Add(PackageValueTypes.NestedPackage, (keyEntry, entry) => ((TreePackage)entry.Value).Save());
            _packageValueSingleDataHandlers.Add(PackageValueTypes.Object, (keyEntry, entry) => {
                Type type = entry.Value.GetType();
                if (AttributeExtensions.IsDefined<NonPackageAttribute>(type) || AttributeExtensions.IsDefined<Formatting.IgnoreAttribute>(type))
                    return new byte[0];
                ICustomPackage customPackage = null;
                byte flag;
                if (AttributeExtensions.IsDefined<PropertyPackageAttribute>(type)) {
                    customPackage = PropertyPackage.Instance;
                    flag = 0;
                } else if (AttributeExtensions.IsDefined<FieldPackageAttribute>(type)) {
                    customPackage = FieldPackage.Instance;
                    flag = 1;
                } else if (AttributeExtensions.IsDefined<CustomPackageAttribute>(type)) {
                    var attribute=AttributeExtensions.GetCustomAttribute<CustomPackageAttribute>(type);
                    customPackage = FastWrapper.CreateInstance<ICustomPackage>(attribute.CustomPackageType);
                    //customPackage= CustomPackageAttribute.GetCustomPackageType(type);
                    flag = 255;
                } else {
                    customPackage = PropertyPackage.Instance;
                    flag = 0;
                }
                using (MemoryStream stream = new MemoryStream(128)) {
                    stream.WriteByte(flag);
                    byte[] buffer = null;
                    if (flag == 255) {
                        buffer = Encoding.UTF8.GetBytes(customPackage.GetType().AssemblyQualifiedName);
                        short iShort = (short)buffer.Length;
                        byte[] buffer2 = new byte[2];
                        Marshal.Copy((IntPtr)(&iShort), buffer2, 0, 2);
                        stream.Write(buffer2, 0, buffer2.Length);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    buffer= customPackage.Save(entry.Value);
                    stream.Write(buffer,0,buffer.Length);
                    return stream.ToArray();
                } 
            });
            #endregion
            #region valueType map unpackage SingleData Handler
            _unPakcageValueSingleDataHandlers = new Dictionary<PackageValueTypes, UnPackageValueSingleDataHandler>();
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Boolean, (keyEntry, entry, buffer) => entry.Value = buffer[0] == 1);
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Byte, (keyEntry, entry, buffer) => entry.Value = buffer[0]);
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Char, (keyEntry, entry, buffer) => {
                char tChar; ;
                Marshal.Copy(buffer, 0, (IntPtr)(&tChar), 2);
                entry.Value = tChar;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Int16, (keyEntry, entry, buffer) => {
                short tShort;
                Marshal.Copy(buffer, 0, (IntPtr)(&tShort), 2);
                entry.Value = tShort;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Int32, (keyEntry, entry, buffer) => {
                int tInt32;
                Marshal.Copy(buffer, 0, (IntPtr)(&tInt32), 4);
                entry.Value = tInt32;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Int64, (keyEntry, entry, buffer) => {
                long tInt64;
                Marshal.Copy(buffer, 0, (IntPtr)(&tInt64), 8);
                entry.Value = tInt64;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Single, (keyEntry, entry, buffer) => {
                float tSingle;
                Marshal.Copy(buffer, 0, (IntPtr)(&tSingle), 4);
                entry.Value = tSingle;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Double, (keyEntry, entry, buffer) => {
                double tDouble;
                Marshal.Copy(buffer, 0, (IntPtr)(&tDouble), 8);
                entry.Value = tDouble;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.SByte, (keyEntry, entry, buffer) => {
                sbyte tSByte;
                Marshal.Copy(buffer, 0, (IntPtr)(&tSByte), 1);
                entry.Value = tSByte;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Decimal, (keyEntry, entry, buffer) => entry.Value = decimal.Parse(Encoding.ASCII.GetString(buffer)));
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.UInt16, (keyEntry, entry, buffer) => {
                ushort tUShort;
                Marshal.Copy(buffer, 0, (IntPtr)(&tUShort), 2);
                entry.Value = tUShort;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.UInt32, (keyEntry, entry, buffer) => {
                uint tUInt32;
                Marshal.Copy(buffer, 0, (IntPtr)(&tUInt32), 4);
                entry.Value = tUInt32;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.UInt64, (keyEntry, entry, buffer) => {
                ulong tUInt64;
                Marshal.Copy(buffer, 0, (IntPtr)(&tUInt64), 8);
                entry.Value = tUInt64;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.IntPtr, (keyEntry, entry, buffer) => {
                IntPtr tIntPtr;
                if (buffer.Length == 4) {
                    int tIntPtr2;
                    Marshal.Copy(buffer, 0, (IntPtr)(&tIntPtr2), 4);
                    tIntPtr = new IntPtr(tIntPtr2);
                } else {
                    long tIntPtr3;
                    Marshal.Copy(buffer, 0, (IntPtr)(&tIntPtr3), 8);
                    tIntPtr = new IntPtr(tIntPtr3);
                }
                entry.Value = tIntPtr;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.UIntPtr, (keyEntry, entry, buffer) => {
                UIntPtr tUIntPtr;
                if (buffer.Length == 4) {
                    uint tUIntPtr2;
                    Marshal.Copy(buffer, 0, (IntPtr)(&tUIntPtr2), 4);
                    tUIntPtr = new UIntPtr(tUIntPtr2);
                } else {
                    ulong tUIntPtr3;
                    Marshal.Copy(buffer, 0, (IntPtr)(&tUIntPtr3), 8);
                    tUIntPtr = new UIntPtr(tUIntPtr3);
                }
                entry.Value = tUIntPtr;
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.DateTime, (keyEntry, entry, buffer) => entry.Value= DateTime.Parse(Encoding.ASCII.GetString(buffer)));
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.TimeSpan, (keyEntry, entry, buffer) => {
                double tTimeSpan;
                Marshal.Copy(buffer, 0, (IntPtr)(&tTimeSpan), 8);
                entry.Value = TimeSpan.FromMilliseconds(tTimeSpan);
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.String, (keyEntry, entry, buffer) => entry.Value = Encoding.UTF8.GetString(buffer));
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Stream, (keyEntry, entry, buffer) => entry.Value = new MemoryStream(buffer));
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Icon, (keyEntry, entry, buffer) => {
                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    entry.Value = new System.Drawing.Icon(memoryStream);
                }
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Image, (keyEntry, entry, buffer) => {
                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    entry.Value = System.Drawing.Image.FromStream(memoryStream);
                }
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Color, (keyEntry, entry, buffer) => {
                int tColor;
                Marshal.Copy(buffer, 0, (IntPtr)(&tColor), 4);
                entry.Value = System.Drawing.Color.FromArgb(tColor);
            });
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Guid,(keyEntry, entry, buffer) =>entry.Value= new Guid(buffer));
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.NestedPackage, (keyEntry, entry, buffer) => entry.Value = Load(buffer));
            _unPakcageValueSingleDataHandlers.Add(PackageValueTypes.Object, (keyEntry, entry,buffer) => {
                if (buffer.Length == 0)//NonPackage
                    return;
                using (MemoryStream stream = new MemoryStream(buffer)) {
                    byte flag = (byte)stream.ReadByte();
                    ICustomPackage customPackage = null;
                    byte[] buffer2;
                    if (flag == 0) {
                        customPackage = PropertyPackage.Instance;
                    } else if (flag == 1) {
                        customPackage = FieldPackage.Instance;
                    } else if (flag == 255) {
                        short iShort;
                        buffer2 = new byte[2];
                        stream.Read(buffer2, 0, 2);
                        Marshal.Copy(buffer2, 0, (IntPtr)(&iShort), 2);
                        buffer2= new byte [iShort];
                        stream.Read(buffer2, 0, iShort);
                        string typeFullName = Encoding.UTF8.GetString(buffer2);
                        Type customType = FastWrapper.GetWarpperType(typeFullName);
                        if (customType == null)
                            throw new TypeLoadException("未能找到类型“"+typeFullName+"”");
                        customPackage = (ICustomPackage)FastWrapper.CreateInstance(customType);
                    } else {
                        throw new NotSupportedException();
                    }
                    //read to end
                    buffer2 = new byte[stream.Length - stream.Position];
                    stream.Read(buffer2, 0, buffer2.Length);
                    entry.Value = customPackage.Load(buffer2);
                }
            });
            #endregion
        }
        /// <summary>
        /// 创建树型包的实例（键值大小写敏感）
        /// </summary>
        public TreePackage() :this(false){
        }
        /// <summary>
        /// 创建树型包的实例
        /// </summary>
        /// <param name="keyIgnoreCase">键值是否忽略大小写</param>
        public TreePackage(bool keyIgnoreCase) {
            _entries = new Dictionary<string, Entry>(keyIgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            _packageEntry = new HeaderEntry();
            _packageEntry.Version = MaxVersion;
            _packageEntry.KeyIgnoreCase = keyIgnoreCase;
        }
        /// <summary>
        /// 树型包项创建实例
        /// </summary>
        /// <param name="packageEntry">树型包项</param>
        internal TreePackage(HeaderEntry packageEntry) {
            _packageEntry = packageEntry;
            _entries = new Dictionary<string, Entry>(packageEntry.KeyIgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            if (packageEntry.HasComment) {
                if (packageEntry.CommentLength == 0 | packageEntry.CommentData == null || packageEntry.CommentData.Length == 0) {
                    Comment = null;
                } else {
                    _comment = Encoding.UTF8.GetString(_packageEntry.CommentData);
                }
            } else {
                Comment = null;
            }
            _packageEntry.CommentData = null;
            _packageEntry.CommentLength = 0;
        }
        #endregion

        #region methods

        #region this
        /// <summary>
        /// 获取或设置键值对应的值。
        /// </summary>
        /// <param name="key">键值，如果不存在，将返回 null 。</param>
        /// <returns>返回此键值对应的值。</returns>
        /// <remarks>当设置值时，请参考<see cref="Add(string,object,Symbol.IO.Packing.PackageCompressTypes)"/>的说明。</remarks>
        public object this[string key] {
            get {
                if (_entries.ContainsKey(key))
                    return _entries[key].Value;
                return null;
            }
            set {
                Add(key, value);
            }
        }
        
        #endregion

        #region ContainsKey
        /// <summary>
        /// 判断是否包含某个键值。
        /// </summary>
        /// <param name="key">需要检查的键值</param>
        /// <returns>返回是否包含。</returns>
        public bool ContainsKey(string key) {
            return _entries.ContainsKey(key);
        }
        /// <summary>
        /// 判断是否包含某个键值，其中一个不存在就会返回false。
        /// </summary>
        /// <param name="keys">需要检查的键值列表，如果传null或空数组，将直接返回true。</param>
        /// <returns>返回是否包含。</returns>
        public bool ContainsKeys(params string[] keys) {
            if (keys == null || keys.Length == 0)
                return true;
            foreach (string key in keys) {
                if (!_entries.ContainsKey(key))
                    return false;
            }
            return true;
        }
        #endregion

        #region Add
        private void Add(Entry entry) {
            if (_entries.ContainsKey(entry.Key)) {
                _entries[entry.Key] = entry;
            } else {
                _entries.Add(entry.Key, entry);
            }
        }
        /// <summary>
        /// 添加一项
        /// </summary>
        /// <param name="key">键值，注意不要重复；如果忽略大小写，更需要注意；一旦重复将替换原来的项。</param>
        /// <param name="value">值，可以传null。</param>
        /// <param name="compressType">压缩类型，对于大数据可以考虑适当的压缩一下。</param>
        /// <returns>返回的是树型包自己，这样可以做连体的写法。</returns>
        /// <example><code>
        /// package.Add("test","abc")
        ///        .Add("xxx",0)
        ///        .Add("yyy",DateTime.Now);
        /// </code></example>
        /// <remarks>当出现重复时，会替换现有的，并且会继承部分属性：CompressType、Attributes</remarks>
        public TreePackage Add(string key, object value, PackageCompressTypes compressType = PackageCompressTypes.None) {
            Entry entry = null;
            if (_entries.ContainsKey(key)) {
                entry = _entries[key];
                entry.Value = value;
                if (compressType != PackageCompressTypes.None)
                    entry.CompressType = compressType;
                entry.ArrayType = PackageArrayTypes.None;
                SetValueType(entry);
            } else {
                KeyEntry keyEntry = new KeyEntry();
                //keyEntry.KeyData = Encoding.UTF8.GetBytes(key);
                //keyEntry.KeyLength = (ushort)keyEntry.KeyData.Length;
                entry = new Entry(key, keyEntry);
                entry.Value = value;
                SetValueType(entry);
                entry.CompressType = compressType;
                _entries.Add(key, entry);
            }
            return this;
        }
        void SetValueType(Entry entry) {
            if (entry.Nullable) {
                entry.ValueType = PackageValueTypes.Object;
                return;
            }
            entry.ValueType = PackageValueTypes.Object;
            Type type =entry.Value.GetType();
            SetSingleValueType(type, entry);//先确定一下类型

            //不能确定的可能会是数组或集合
            if (entry.ValueType == PackageValueTypes.Object) {
                //确定是否为集合与数组
                type = GetElementType(type, entry);
                //泛型的集合需要特殊的处理一下
                if (entry.ArrayType == PackageArrayTypes.T_Array || entry.ArrayType == PackageArrayTypes.T_List) {
                    SetSingleValueType(type, entry);
                }
            }

            //if(entry.ValueType == PackageValueTypes.Object)
            //    entry.ValueType=_typeCodes[Type.GetTypeCode(type)];
            //if (entry.ValueType == PackageValueTypes.Object) {
            //    if(IsTheTypes(type,typeof(System.IO.Stream))){
            //        entry.ValueType= PackageValueTypes.Stream;
            //    } else if (IsTheTypes(type, typeof(TreePackage))) {
            //        entry.ValueType = PackageValueTypes.NestedPackage;
            //    } else if (IsTheTypes(type, typeof(TimeSpan))) {
            //        entry.ValueType = PackageValueTypes.TimeSpan;
            //    } else if (IsTheTypes(type, typeof(IntPtr))) {
            //        entry.ValueType = PackageValueTypes.IntPtr;
            //    } else if (IsTheTypes(type, typeof(UIntPtr))) {
            //        entry.ValueType = PackageValueTypes.UIntPtr;
            //    }
            //}
        }
        void SetSingleValueType(Type type, Entry entry) {
            //类型已经有映射
            if (_typeValueTypes.ContainsKey(type)) {
                entry.ValueType = _typeValueTypes[type];
                return;
            }
            foreach (KeyValuePair<Type, PackageValueTypes> item in _typeValueTypes) {
                if (IsTheTypes(type, item.Key)) {
                    entry.ValueType = item.Value;
                    return;
                }
            }
            //TypeCode有映射,这个太笼统
            entry.ValueType = _typeCodes[Type.GetTypeCode(type)];
            if (entry.ValueType != PackageValueTypes.Object)
                return;
        }
        Type GetElementType(Type type, Entry entry) {
            
            Type result = null;
            //数组
            if (type.IsArray) {
                result = type.GetElementType();//获取元素类型
                if (result == typeof(object)) {//object[]
                    entry.ArrayType = PackageArrayTypes.Object_Array;
                } else {//T[]
                    entry.ArrayType = PackageArrayTypes.T_Array;
                }
                return result;
            }
            //if (type.FullName == "System.Windows.Forms.Cursor") {
            //    entry.ValueType = PackageValueTypes.Cursor;
            //}
            
            if(IsTheTypes(type,typeof(TreePackage))){
                entry.ValueType = PackageValueTypes.NestedPackage;
                return type;
            }
            if (AttributeExtensions.IsDefined<CustomPackageAttribute>(type)) {
                entry.ValueType = PackageValueTypes.Object;
                return type;
            }
            //IDictionary<T1,T2>,IDictionary(Hashtable)
            if (IsTheTypes(type, typeof(IDictionary<,>), typeof(System.Collections.IDictionary))) {
                Type[] argTypes = type.GetGenericArguments();
                result = typeof(object);
                entry.ArrayType = PackageArrayTypes.Dictionary;
                return result;
            }
            
            if (IsTheTypes(type, typeof(ICollection<>))) {
                result = type.GetGenericArguments()[0];//IList<T>,ICollection<T>
                if (result == typeof(object)) {
                    entry.ArrayType = PackageArrayTypes.Object_List;
                } else {
                    entry.ArrayType = PackageArrayTypes.T_List;
                }
            } else if (IsTheTypes(type, typeof(IEnumerable<>))) {
                result = type.GetGenericArguments()[0];//IEnumerable<T>
                if (result == typeof(object)) {
                    entry.ArrayType = PackageArrayTypes.Object_Array;
                } else {
                    entry.ArrayType = PackageArrayTypes.T_Array;
                }
            } else if (IsTheTypes(type, typeof(IEnumerator<>))) {
                result = type.GetGenericArguments()[0];//IEnumerator<T>
                if (result == typeof(object)) {
                    entry.ArrayType = PackageArrayTypes.Object_Array;
                } else {
                    entry.ArrayType = PackageArrayTypes.T_Array;
                }
            } else if (IsTheTypes(type, typeof(System.Collections.Specialized.NameValueCollection))) {
                result = typeof(object);//NameValueCollection
                entry.ArrayType = PackageArrayTypes.NameValueCollection;
            } else if (IsTheTypes(type, typeof(System.Collections.IEnumerable), typeof(System.Collections.IEnumerator))) {
                result = typeof(object);//IList,ICollection
                entry.ArrayType = PackageArrayTypes.Object_List;
            } else {
                result = type;
            }

            return result;

        }
        static bool IsTheTypes(Type type, params Type[] theTypes) {
            if (type == null || theTypes==null || theTypes.Length==0)
                return false;
            for (int i = 0; i < theTypes.Length; i++) {
                if (TypeExtensions.IsInheritFrom(type,theTypes[i]) || type == theTypes[i])
                    return true;
                if (type.IsGenericType && theTypes[i].IsGenericTypeDefinition) {
                    if (type.GetGenericTypeDefinition() == theTypes[i])
                        return true;
                    foreach (Type t3 in type.GetInterfaces()) {
                        if (t3.IsGenericType && t3.GetGenericTypeDefinition() == theTypes[i])
                            return true;
                    }
                    Type t4 = type.BaseType;
                    while (t4 != null && t4.IsGenericType) {
                        if (t4.GetGenericTypeDefinition() == theTypes[i]) {
                            return true;
                        }
                        t4 = t4.BaseType;
                    }
                }
            }
            return false;
        }
        static Type GetTheTypes(Type type, params Type[] theTypes) {
            if (type == null || theTypes == null || theTypes.Length == 0)
                return null;
            for (int i = 0; i < theTypes.Length; i++) {
                if (TypeExtensions.IsInheritFrom(type,theTypes[i]) || type == theTypes[i])
                    return theTypes[i];
                if (type.IsGenericType && theTypes[i].IsGenericTypeDefinition) {
                    if (type.GetGenericTypeDefinition() == theTypes[i])
                        return type;
                    foreach (Type t3 in type.GetInterfaces()) {
                        if (t3.IsGenericType && t3.GetGenericTypeDefinition() == theTypes[i])
                            return t3;
                    }
                    Type t4 = type.BaseType;
                    while (t4 != null && t4.IsGenericType) {
                        if (t4.GetGenericTypeDefinition() == theTypes[i]) {
                            return t4;
                        }
                        t4 = t4.BaseType;
                    }
                }
            }
            return null;
        }
        #endregion

        #region SetKey
        /// <summary>
        /// 替换键值
        /// </summary>
        /// <param name="originalKey">原始键值</param>
        /// <param name="newKey">新的键值</param>
        /// <returns>返回是否替换成功，不存在时会是false。</returns>
        public bool SetKey(string originalKey, string newKey) {
            //键值为空
            if (originalKey == null || newKey==null)
                return false;
            //两个键值完全相同。
            if (string.Equals(originalKey, newKey, _packageEntry.KeyIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                return true;
            //不存在此键值。
            if (!_entries.ContainsKey(originalKey)) {
                return false;
            }
            Entry entry = _entries[originalKey];
            IEntry entryInterface = (IEntry)entry;
            entryInterface.SetKey(newKey);
            _entries.Remove(originalKey);
            _entries.Add(newKey, entry);
            return true;
        }
        #endregion

        #region Clear
        /// <summary>
        /// 清空树型包，它会将所有值中实现了<seealso cref="System.IDisposable"/>接口的对象释放掉。
        /// </summary>
        public void Clear() {
            Clear(true);
        }
        /// <summary>
        /// 清空树型包
        /// </summary>
        /// <param name="disposable">是否将实现了<seealso cref="System.IDisposable"/>接口的对象释放掉。</param>
        public void Clear(bool disposable) {
            if (_entries==null || _entries.Count == 0)
                return;
            if (disposable) {
                IDisposable disp = null;
                foreach (KeyValuePair<string, Entry> item in _entries) {
                    disp = item.Value.Value as IDisposable;
                    if (disp != null) {
                        try {
                            disp.Dispose();
                        } catch (DisposedException) {
                        } catch (ObjectDisposedException) {
                        }
                    }
                    item.Value.Dispose();
                }
            }
            _entries.Clear();
        }
        #endregion

        #region Remove
        /// <summary>
        /// 移除键值（自动释放实现了<seealso cref="System.IDisposable"/>接口的对象）
        /// </summary>
        /// <param name="key">需要移除的键值</param>
        public bool Remove(string key) {
            return Remove(key, true);
        }
        /// <summary>
        /// 移除键值
        /// </summary>
        /// <param name="key">需要移除的键值</param>
        /// <param name="disposable">是否将实现了<seealso cref="System.IDisposable"/>接口的对象释放掉。</param>
        public bool Remove(string key, bool disposable) {
            //不存在键值
            if (!_entries.ContainsKey(key))
                return false;
            if (disposable) {
                IDisposable disp = _entries[key].Value as IDisposable;
                if (disp != null) {
                    try {
                        disp.Dispose();
                        disp = null;
                    } catch (DisposedException) {
                    } catch (ObjectDisposedException) {
                    }
                }
            }
            return _entries.Remove(key);
        }
        #endregion


        #endregion


        #region IDisposable 成员
        /// <summary>
        /// 析构
        /// </summary>
        ~TreePackage() {
            Dispose(false);
        }
        /// <summary>
        /// 释放树型包所占用的资源，它会将所有值中实现了<seealso cref="System.IDisposable"/>接口的对象释放掉。
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        /// <summary>
        /// 释放树型包所占用的资源
        /// </summary>
        /// <param name="disposing">是否为终结。</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_entries != null) {
                    Clear();
                    _entries = null;
                }
                if (_entryList != null) {
                    _entryList.Dispose();
                    _entryList = null;
                }
                if (_comment == null) {
                    _comment = null;
                }
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

        #region IEnumerable<Entry> 成员
        /// <summary>
        /// 获取可循环访问的IEnumerator。
        /// </summary>
        /// <returns>返回可循环访问的IEnumerator。</returns>
        public IEnumerator<TreePackage.Entry> GetEnumerator() {
            return _entries.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员
        /// <summary>
        /// 获取可循环访问的IEnumerator。
        /// </summary>
        /// <returns>返回可循环访问的IEnumerator。</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _entries.Values.GetEnumerator();
        }

        #endregion

        #region types
        delegate byte[] CompressorHandler(byte[] buffer, bool compress);
        #endregion
    }
}
