/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// 数据包数组类型，方便还原到最接近的原始状态,00-99。
    /// </summary>
    public enum PackageArrayTypes : byte {
        /// <summary>
        /// 非数组
        /// </summary>
        None = 0,
        /// <summary>
        /// T[]，泛型的数组
        /// </summary>
        T_Array = 1,
        /// <summary>
        /// List&lt;T&gt;，泛型List
        /// </summary>
        T_List = 2,
        ///// <summary>
        ///// IEnumerable&lt;T&gt;，公开枚举器，该枚举器支持在指定类型的集合上进行简单迭代。foreach
        ///// </summary>
        //T_IEnumerable = 3,
        ///// <summary>
        ///// IEnumerator&lt;T&gt;，支持在泛型集合上进行简单迭代 while(MoveNext())
        ///// </summary>
        //T_IEnumerator = 4,
        /// <summary>
        /// object[]，非泛型的数组
        /// </summary>
        Object_Array = 10,
        /// <summary>
        /// List&gt;T&gt;，泛型List
        /// </summary>
        Object_List = 11,
        ///// <summary>
        ///// <seealso cref="System.Collections.IEnumerable"/>，可枚举类型，非泛型年代的产物。
        ///// </summary>
        //IEnumerable = 20,
        ///// <summary>
        ///// <seealso cref="System.Collections.IEnumerator"/>，可枚举序列，用<seealso cref="System.Collections.IEnumerator.MoveNext"/>来遍历，而不是foreach
        ///// </summary>
        //IEnumerator = 21,
        /// <summary>
        /// <seealso cref="System.Collections.Specialized.NameValueCollection"/>，键值对应多个值
        /// </summary>
        NameValueCollection = 20,
        /// <summary>
        /// Dictionary&lt;T1,string,T2&gt;，字典类型
        /// </summary>
        Dictionary = 21,
        ///// <summary>
        ///// <seealso cref="System.Collections.Hashtable"/>，哈希表，在非泛型的年代，用得很多。
        ///// </summary>
        //Hashtable = 22,
    }

}
/*
   0:None                      //T
   1:T_Array                   //T[]
   2:T_List                    //List<T>
  10:Object_Array              //object[]
  11:Object_List               //List<object>
  20:NameValueCollection       //NameValueCollection
  21:Dictionary                //Dictionary<String,object>
*/