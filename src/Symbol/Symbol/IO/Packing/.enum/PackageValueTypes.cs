/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// ���ݰ���ֵ������,0-255��
    /// </summary>
    public enum PackageValueTypes : byte {
        /// <summary>
        /// �����������ݿ���������bit����0��1���ڳ������棬����true��false���е�ƽ̨true��ʾ-1������ͳһΪ1��0�����ȣ�1�ֽ�
        /// </summary>
        Boolean = 0,
        /// <summary>
        /// �ֽڣ�0~255������1�ֽ�
        /// </summary>
        Byte = 1,
        /// <summary>
        /// �ַ�����Ҫ����Ϊ���ַ�������ֻ�ǵ������ַ�������2�ֽ�
        /// </summary>
        Char = 2,
        /// <summary>
        /// �����ͣ��е�ƽ̨��short���еĽ�integer������2�ֽ�
        /// </summary>
        Int16 = 3,
        /// <summary>
        /// ���ͣ�int�����ϵ�ƽ̨���棬���൱����long���ͣ��Ǹ����û����ô������֣�����4�ֽ�
        /// </summary>
        Int32 = 4,
        /// <summary>
        /// �����ͣ���ƽ̨��֧�ֵ�long������8�ֽ�
        /// </summary>
        Int64 = 5,
        /// <summary>
        /// �����ȣ��еĽ�float�������ݿ������float����sql server�����൱��double�ģ�����4�ֽ�
        /// </summary>
        Single = 6,
        /// <summary>
        /// ˫���ȣ�����8�ֽ�
        /// </summary>
        Double = 7,
        /// <summary>
        /// 8λ�з���������-128~127����ͬ�ǰ�������byte��������룬����1�ֽ�
        /// </summary>
        SByte = 8,
        /// <summary>
        /// ʮ���ƣ���ʵ�����е����֣���С����ͨ�����ڽ�Ǯ���㣬�����е�ƽ̨�ǿ��Ա䳤�ģ������ﵽ32λ�����Ȳ��̶����ַ������洢��
        /// </summary>
        Decimal = 9,
        /// <summary>
        /// �޷��Ŷ����ͣ��еĽ�ushort������ֻ��0������������2�ֽ�
        /// </summary>
        UInt16 = 10,
        /// <summary>
        /// �޷������ͣ�uint��0������������4�ֽ�
        /// </summary>
        UInt32 = 11,
        /// <summary>
        /// �޷��ų�����,ulong��0��������8�ֽ�
        /// </summary>
        UInt64 = 12,
        /// <summary>
        /// ���ڱ�ʾָ�������ƽ̨�ض����͡�ע���������32λ/64λƽ̨�����Ȳ�һ����4/8�ֽ�
        /// </summary>
        IntPtr = 13,
        /// <summary>
        /// ���ڱ�ʾָ�������ƽ̨�ض����͡��޷��ţ�0��������ע���������32λ/64λƽ̨�����Ȳ�һ����4/8�ֽ�
        /// </summary>
        UIntPtr = 14,
        /// <summary>
        /// ��ʾʱ���ϵ�һ�̣�ͨ�������ں͵����ʱ���ʾ�������ֱ�ʾ��Ȼ������ԭ������4�ֽ�
        /// </summary>
        DateTime = 15,
        /// <summary>
        /// ��ʾһ��ʱ��������ʱ������ʾʱ��ģ���˫���ȱ�ʾ��Ȼ������ԭ������8�ֽڡ�
        /// </summary>
        TimeSpan = 16,
        /// <summary>
        /// �ַ��������Ȳ��̶���null��""����0���ȣ�������ᱣ��null��""��
        /// </summary>
        String = 17,
        /// <summary>
        /// �������Ȳ��̶�
        /// </summary>
        Stream = 18,
        /// <summary>
        /// Guid��
        /// </summary>
        Guid = 19,
        /// <summary>
        /// ͼ�꣬ռ�ó���ȡ������������byte[n]��
        /// </summary>
        Icon = 20,
        /// <summary>
        /// ͼ��ռ�ó���ȡ������������byte[n]��
        /// </summary>
        Image = 21,
        /// <summary>
        /// ��ɫ��
        /// </summary>
        Color=22,
        /// <summary>
        /// Ƕ�װ����Ͱ���ͬһ�����ͣ���������ʵ�����޼����Ľṹ��
        /// </summary>
        NestedPackage = 254,
        /// <summary>
        /// �Զ�����󣬲���Ƕ�װ���ʽ���д����
        /// </summary>
        Object = 255
    }
}
/*
   0:Boolean               //bool,       1
   1:Byte                  //byte,       1
   2:Char                  //char,       2
   3:Int16                 //short,      2
   4:Int32                 //int,        4
   5:Int64                 //long,       8
   6:Single                //float,      4
   7:Double                //double,     8
   8:SByte                 //sbyte       1
   9:Decimal               //decimal     n
  10:UInt16                //ushort      2
  11:UInt32                //uint        4
  12:UInt64                //ulong       8
  13:IntPtr                //IntPtr      4
  14:UIntPtr               //UIntPtr     4
  15:DateTime              //DateTime    n
  16:TimeSpan              //TimeSpan    n
  17:String                //string      0-n
  18:Stream                //Stream      0-n
 254:NestedPackage         
 255:Object                //any
*/