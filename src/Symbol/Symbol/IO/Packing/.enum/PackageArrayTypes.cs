/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// ���ݰ��������ͣ����㻹ԭ����ӽ���ԭʼ״̬,00-99��
    /// </summary>
    public enum PackageArrayTypes : byte {
        /// <summary>
        /// ������
        /// </summary>
        None = 0,
        /// <summary>
        /// T[]�����͵�����
        /// </summary>
        T_Array = 1,
        /// <summary>
        /// List&lt;T&gt;������List
        /// </summary>
        T_List = 2,
        ///// <summary>
        ///// IEnumerable&lt;T&gt;������ö��������ö����֧����ָ�����͵ļ����Ͻ��м򵥵�����foreach
        ///// </summary>
        //T_IEnumerable = 3,
        ///// <summary>
        ///// IEnumerator&lt;T&gt;��֧���ڷ��ͼ����Ͻ��м򵥵��� while(MoveNext())
        ///// </summary>
        //T_IEnumerator = 4,
        /// <summary>
        /// object[]���Ƿ��͵�����
        /// </summary>
        Object_Array = 10,
        /// <summary>
        /// List&gt;T&gt;������List
        /// </summary>
        Object_List = 11,
        ///// <summary>
        ///// <seealso cref="System.Collections.IEnumerable"/>����ö�����ͣ��Ƿ�������Ĳ��
        ///// </summary>
        //IEnumerable = 20,
        ///// <summary>
        ///// <seealso cref="System.Collections.IEnumerator"/>����ö�����У���<seealso cref="System.Collections.IEnumerator.MoveNext"/>��������������foreach
        ///// </summary>
        //IEnumerator = 21,
        /// <summary>
        /// <seealso cref="System.Collections.Specialized.NameValueCollection"/>����ֵ��Ӧ���ֵ
        /// </summary>
        NameValueCollection = 20,
        /// <summary>
        /// Dictionary&lt;T1,string,T2&gt;���ֵ�����
        /// </summary>
        Dictionary = 21,
        ///// <summary>
        ///// <seealso cref="System.Collections.Hashtable"/>����ϣ���ڷǷ��͵�������õúܶࡣ
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