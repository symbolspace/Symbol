/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
using System;
using System.Text;

namespace Symbol.IO.Packing {
    partial class TreePackage {

        /// <summary>
        /// ���Ͱ���ֵ��
        /// </summary>
        internal struct KeyEntry {
            /// <summary>
            /// ��ֵ���ȣ��������ַ�����ֵ�����Գ��Ȳ��̶��ģ�����ע���ֵ����̫��������1-65535
            /// </summary>
            public ushort KeyLength;
            /// <summary>
            /// ֵ������
            /// </summary>
            public PackageValueTypes ValueType;
            ///// <summary>
            ///// ֵ�任����
            ///// </summary>
            //public PackageValueAsTypes ValueAsType;
            /// <summary>
            /// �Ƿ�Ϊ��ֵ��null������<see cref="ArrayType"/>��ռһ���ֽڣ�0xx-1xx����λ��0��1
            /// </summary>
            public bool Nullable;
            /// <summary>
            /// ֵ���������ͣ�����<see cref="Nullable"/>��ռһ���ֽڣ�x00-x99,ʮλ�����������
            /// </summary>
            public PackageArrayTypes ArrayType;
            /// <summary>
            /// �Ƿ�����չ���ԣ�����<see cref="CompressType"/>��ռһ���ֽڣ�0xx-1xx����λ��0��1
            /// </summary>
            public bool HasAttributes;
            /// <summary>
            /// ��չ���Գ��ȣ�����һ��Ƕ�װ������Գ��Ȳ�ȷ�����Ƕ��٣�����uint����ֵ��int��
            /// </summary>
            /// <remarks>ֻ��<see cref="HasAttributes"/>ʱ�Ż��б�Ҫ��ȡ��ֵ��</remarks>
            public int AttributesLength;

            //Attributes

            /// <summary>
            /// ֵ�Ķ���������ѹ�����͡�����<see cref="HasAttributes"/>��ռһ���ֽڣ�x00-x99��ʮλ���λ�����ݡ�
            /// </summary>
            /// <remarks>�е����ݷǳ��󣬱����������������򳬴��ı��������ʵ��Ŀ�����һЩѹ����</remarks>
            public PackageCompressTypes CompressType;
            /// <summary>
            /// ֵ�����ݳ���
            /// </summary>
            public int ValueDataLength;
            /// <summary>
            /// ֵ������CRC32У����
            /// </summary>
            public ulong CRC32;
            /// <summary>
            /// ������Entry����ľ��Ǽ�ֵ��ʵ�����ݡ�
            /// </summary>
            public byte[] KeyData;
            /// <summary>
            /// ��<see cref="KeyData"/>��������չ���Ե����ݣ������<see cref="HasAttributes"/>ʱ�����ݳ��Ⱦ��� <see cref="AttributesLength"/>
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