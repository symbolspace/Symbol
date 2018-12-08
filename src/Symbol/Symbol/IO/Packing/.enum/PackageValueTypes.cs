/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// 数据包单值的类型,0-255。
    /// </summary>
    public enum PackageValueTypes : byte {
        /// <summary>
        /// 布尔，在数据库里面它叫bit，即0与1，在程序里面，它是true与false，有的平台true表示-1，这里统一为1与0，长度：1字节
        /// </summary>
        Boolean = 0,
        /// <summary>
        /// 字节，0~255，长度1字节
        /// </summary>
        Byte = 1,
        /// <summary>
        /// 字符，不要误认为是字符串，它只是单个的字符，长度2字节
        /// </summary>
        Char = 2,
        /// <summary>
        /// 短整型，有的平台叫short，有的叫integer，长度2字节
        /// </summary>
        Int16 = 3,
        /// <summary>
        /// 整型，int，在老的平台里面，它相当于是long类型，那个年代没有这么大的数字，长度4字节
        /// </summary>
        Int32 = 4,
        /// <summary>
        /// 长整型，新平台才支持的long，长度8字节
        /// </summary>
        Int64 = 5,
        /// <summary>
        /// 单精度，有的叫float，在数据库里面的float至少sql server它是相当于double的，长度4字节
        /// </summary>
        Single = 6,
        /// <summary>
        /// 双精度，长度8字节
        /// </summary>
        Double = 7,
        /// <summary>
        /// 8位有符号整数，-128~127，等同是把正常的byte拆成了两半，长度1字节
        /// </summary>
        SByte = 8,
        /// <summary>
        /// 十进制，现实生活中的数字，带小数，通常用于金钱计算，它在有的平台是可以变长的，甚至达到32位，长度不固定用字符串来存储。
        /// </summary>
        Decimal = 9,
        /// <summary>
        /// 无符号短整型，有的叫ushort，就是只有0和正数，长度2字节
        /// </summary>
        UInt16 = 10,
        /// <summary>
        /// 无符号整型，uint，0和正数，长度4字节
        /// </summary>
        UInt32 = 11,
        /// <summary>
        /// 无符号长整型,ulong，0和正数，8字节
        /// </summary>
        UInt64 = 12,
        /// <summary>
        /// 用于表示指针或句柄的平台特定类型。注意它会根据32位/64位平台，长度不一样，4/8字节
        /// </summary>
        IntPtr = 13,
        /// <summary>
        /// 用于表示指针或句柄的平台特定类型。无符号，0与正数，注意它会根据32位/64位平台，长度不一样，4/8字节
        /// </summary>
        UIntPtr = 14,
        /// <summary>
        /// 表示时间上的一刻，通常以日期和当天的时间表示。用数字表示，然后做还原，长度4字节
        /// </summary>
        DateTime = 15,
        /// <summary>
        /// 表示一个时间间隔，有时用来表示时间的，用双精度表示，然后做还原，长度8字节。
        /// </summary>
        TimeSpan = 16,
        /// <summary>
        /// 字符串，长度不固定，null或""都是0长度，解析后会保持null或""。
        /// </summary>
        String = 17,
        /// <summary>
        /// 流，长度不固定
        /// </summary>
        Stream = 18,
        /// <summary>
        /// Guid，
        /// </summary>
        Guid = 19,
        /// <summary>
        /// 图标，占用长度取决于它保存后的byte[n]。
        /// </summary>
        Icon = 20,
        /// <summary>
        /// 图像，占用长度取决于它保存后的byte[n]。
        /// </summary>
        Image = 21,
        /// <summary>
        /// 颜色，
        /// </summary>
        Color=22,
        /// <summary>
        /// 嵌套包，和包是同一个类型，这样可以实现无限级树的结构。
        /// </summary>
        NestedPackage = 254,
        /// <summary>
        /// 自定义对象，采用嵌套包方式进行打包。
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