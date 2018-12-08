/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Text;

namespace Symbol.IO.Packing {
    partial class TreePackage {

        /// <summary>
        /// 树型包键值项
        /// </summary>
        internal struct KeyEntry {
            /// <summary>
            /// 键值长度，由于是字符串键值，所以长度不固定的，并且注意键值不会太长。长度1-65535
            /// </summary>
            public ushort KeyLength;
            /// <summary>
            /// 值的类型
            /// </summary>
            public PackageValueTypes ValueType;
            ///// <summary>
            ///// 值变换类型
            ///// </summary>
            //public PackageValueAsTypes ValueAsType;
            /// <summary>
            /// 是否为空值，null。它与<see cref="ArrayType"/>共占一个字节，0xx-1xx，百位的0与1
            /// </summary>
            public bool Nullable;
            /// <summary>
            /// 值的数组类型，它与<see cref="Nullable"/>共占一个字节，x00-x99,十位与个数的数据
            /// </summary>
            public PackageArrayTypes ArrayType;
            /// <summary>
            /// 是否有扩展属性，它与<see cref="CompressType"/>共占一个字节，0xx-1xx，百位的0与1
            /// </summary>
            public bool HasAttributes;
            /// <summary>
            /// 扩展属性长度，它是一个嵌套包，所以长度不确定会是多少，另外uint的正值比int大。
            /// </summary>
            /// <remarks>只有<see cref="HasAttributes"/>时才会有必要读取此值。</remarks>
            public int AttributesLength;

            //Attributes

            /// <summary>
            /// 值的二进制数据压缩类型。它与<see cref="HasAttributes"/>共占一个字节，x00-x99，十位与个位的数据。
            /// </summary>
            /// <remarks>有的数据非常大，比如流或二进制数组或超大文本，可以适当的考虑用一些压缩。</remarks>
            public PackageCompressTypes CompressType;
            /// <summary>
            /// 值的数据长度
            /// </summary>
            public int ValueDataLength;
            /// <summary>
            /// 值的数据CRC32校验码
            /// </summary>
            public ulong CRC32;
            /// <summary>
            /// 紧跟在Entry后面的就是键值的实际数据。
            /// </summary>
            public byte[] KeyData;
            /// <summary>
            /// 在<see cref="KeyData"/>后面是扩展属性的数据，如果有<see cref="HasAttributes"/>时，数据长度就是 <see cref="AttributesLength"/>
            /// </summary>
            public byte[] AttributesData;
        }
    }
}
/*
Node                      Type           Length     Value-Range
---------------------------------------------------------------
KeyLength                 ushort         2           1-65535
ValueType                 byte           1           0-255
[Nullable                 bool           1           0xx-1xx,true,false
]ArrayType                byte                       x00-x99
[HasAttributes            bool           1           0xx-1xx,true,false
 :true)AttributesLength   uint           4           
]CompressType             byte                       x00-x99,
ValueDataLength           uint           4
CRC32                     ulong          8
 KeyData:bytes            bytes[]        byte[]      [n]
 AttribtesData:bytes      byte[]         byte[]      [n]  NestedPackage
 ValueData:bytes          byte[]         byte[]      [n]
*/