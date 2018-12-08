/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {

    partial class TreePackage {
        /// <summary>
        /// ���Ͱ���
        /// </summary>
        internal struct HeaderEntry {
            /// <summary>
            /// ����ʼ��ǣ�TREP
            /// </summary>
            public static readonly byte[] StartFlag = new byte[] { 84, 82, 69, 80 };//TREP
            /// <summary>
            /// ��ʾ����Ѿ���ʼ���������������Ľ�����ע����֮������ݣ������⴦������ֱ�ӵ����ġ�
            /// </summary>
            public bool Started;
            /// <summary>
            /// �汾�ţ���ͬ�İ汾�����в�ͬ�Ľ�����ʽ������<see cref="EncryptType"/>��ռ��һ�ֽ�,00x-29x��
            /// </summary>
            public byte Version;
            /// <summary>
            /// �Ƿ������ע��
            /// </summary>
            public bool HasComment;
            /// <summary>
            /// ��ע���ݳ��ȣ�ֻ��<see cref="HasComment"/>ʱ���б�Ҫ��ȡ�������ݡ�
            /// </summary>
            public int CommentLength;
            /// <summary>
            /// �������ͣ�����<see cref="Version"/>��ռ��һ����
            /// </summary>
            /// <remarks>ָ����<see cref="CommentData"/>/KeysBlock/DataBlock��Щ���ݵĴ���ʽ��</remarks>
            public PackageEncryptTypes EncryptType;
            /// <summary>
            /// ��ֵ�Ƿ���Դ�Сд������<see cref="HasAttributes"/>��ռһ���ֽڣ�0xx-1xx����λ��0��1
            /// </summary>
            public bool KeyIgnoreCase;
            /// <summary>
            /// �Ƿ�����չ���ԣ�����<see cref="KeyIgnoreCase"/>��ռһ���ֽڣ�x0x-x1x��ʮλ��0��1
            /// </summary>
            public bool HasAttributes;
            /// <summary>
            /// ��չ���Գ��ȣ�����һ��Ƕ�װ������Գ��Ȳ�ȷ�����Ƕ��٣�����uint����ֵ��int��
            /// </summary>
            /// <remarks>ֻ��<see cref="HasAttributes"/>ʱ�Ż��б�Ҫ��ȡ��ֵ��</remarks>
            public int AttributesLength;

            /// <summary>
            /// ��ֵ�����ݳ��ȣ�����ֱ�Ӿ�������������ټ�ֵ����
            /// </summary>
            public int KeysCount;
            /// <summary>
            /// Comment�����ݡ�ֻ��<see cref="HasComment"/>ʱ����Ҫ��ȡ�����ݡ�
            /// </summary>
            public byte[] CommentData;
            /// <summary>
            /// ��CommentData��������չ���Ե����ݣ������<see cref="HasAttributes"/>ʱ�����ݳ��Ⱦ��� <see cref="AttributesLength"/>
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