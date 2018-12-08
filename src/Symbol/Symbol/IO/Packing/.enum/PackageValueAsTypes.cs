/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// 数据包单值的变换类型,0-255。
    /// </summary>
    public enum PackageValueAsTypes : byte {
        /// <summary>
        /// 实际类型，不会做任何的变换，与ValueType是一致的
        /// </summary>
        Entity = 0,
        /// <summary>
        /// 实际是一个枚举类型，只不过保存时识别为数字了。
        /// </summary>
        Enum = 1,
        /// <summary>
        /// 是数组类型（可枚举的都是此类型）
        /// </summary>
        Array = 2,
        /// <summary>
        /// 流对象，还原后其实是System.IO.MemoryStream，有的平台可能是自己实现的一种方式。
        /// </summary>
        Stream = 3,
        /// <summary>
        /// 自定义对象，采用嵌套包方式进行打包。
        /// </summary>
        Object = 4,
        /// <summary>
        /// 嵌套包，和包是同一个类型，这样可以实现无限级树的结构。
        /// </summary>
        NestedPackage = 5
    }

}
/*
   0:Entity                //
   1:Enum
   2:Array
   3:Stream
   4:Object
   5:NestedPackage
*/