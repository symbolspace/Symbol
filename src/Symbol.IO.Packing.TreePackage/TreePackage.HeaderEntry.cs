/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {

    partial class TreePackage {
        /// <summary>
        /// 树型包项
        /// </summary>
        internal struct HeaderEntry {
            /// <summary>
            /// 包开始标记：TREP
            /// </summary>
            public static readonly byte[] StartFlag = new byte[] { 84, 82, 69, 80 };//TREP
            /// <summary>
            /// 表示标记已经开始，接下来是真正的解析，注意标记之后的数据，有特殊处理，并非直接的明文。
            /// </summary>
            public bool Started;
            /// <summary>
            /// 版本号，不同的版本可能有不同的解析方式。它与<see cref="EncryptType"/>共占用一字节,00x-29x。
            /// </summary>
            public byte Version;
            /// <summary>
            /// 是否包含备注。
            /// </summary>
            public bool HasComment;
            /// <summary>
            /// 备注数据长度，只有<see cref="HasComment"/>时才有必要读取它的数据。
            /// </summary>
            public int CommentLength;
            /// <summary>
            /// 加密类型，它与<see cref="Version"/>共占用一节字
            /// </summary>
            /// <remarks>指的是<see cref="CommentData"/>/KeysBlock/DataBlock这些数据的处理方式。</remarks>
            public PackageEncryptTypes EncryptType;
            /// <summary>
            /// 键值是否忽略大小写，它与<see cref="HasAttributes"/>共占一个字节，0xx-1xx，百位的0与1
            /// </summary>
            public bool KeyIgnoreCase;
            /// <summary>
            /// 是否有扩展属性，它与<see cref="KeyIgnoreCase"/>共占一个字节，x0x-x1x，十位的0与1
            /// </summary>
            public bool HasAttributes;
            /// <summary>
            /// 扩展属性长度，它是一个嵌套包，所以长度不确定会是多少，另外uint的正值比int大。
            /// </summary>
            /// <remarks>只有<see cref="HasAttributes"/>时才会有必要读取此值。</remarks>
            public int AttributesLength;

            /// <summary>
            /// 键值的数据长度，它将直接决定会解析出多少键值来。
            /// </summary>
            public int KeysCount;
            /// <summary>
            /// Comment的数据。只有<see cref="HasComment"/>时才需要读取此数据。
            /// </summary>
            public byte[] CommentData;
            /// <summary>
            /// 在CommentData后面是扩展属性的数据，如果有<see cref="HasAttributes"/>时，数据长度就是 <see cref="AttributesLength"/>
            /// </summary>
            public byte[] AttributesData;
            //KeysBlock
        }
    }
}
/*
Node                      Type           Length     Value-Range
---------------------------------------------------------------
[Start]                   byte           4          [T][R][E][P]                                 0-3
[Version                  byte           1          00x-25x                                      4
]EncryptType              byte                      xx0-xx9,0:default-enc,1-BinaryWave
[KeyIgnoreCase            bool           1          0xx-1xx,true,false                           5
 HasAttributes            bool                      x0x-x1x,true,false                           
]HasComment               bool                      xx0-xx1,true,false                           
 :true)AttributesLength   int            4                                                       6,7,8,9 
 :true)CommentLength      int            4                                                       6,7,8,9|7,8,9,10
KeysCount                 int            4                                                       6,7,8,9|7,8,9,10|11,12,13,14
 AttribtesData:bytes      bytes[]        byte[]     [n]
 CommentData              byte[]         byte[]     [n]                                          
KeysBlock                 byte[]         byte[]     [n] 
*/