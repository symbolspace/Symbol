/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// ���ݰ���ֵ�ı任����,0-255��
    /// </summary>
    public enum PackageValueAsTypes : byte {
        /// <summary>
        /// ʵ�����ͣ��������κεı任����ValueType��һ�µ�
        /// </summary>
        Entity = 0,
        /// <summary>
        /// ʵ����һ��ö�����ͣ�ֻ��������ʱʶ��Ϊ�����ˡ�
        /// </summary>
        Enum = 1,
        /// <summary>
        /// ���������ͣ���ö�ٵĶ��Ǵ����ͣ�
        /// </summary>
        Array = 2,
        /// <summary>
        /// �����󣬻�ԭ����ʵ��System.IO.MemoryStream���е�ƽ̨�������Լ�ʵ�ֵ�һ�ַ�ʽ��
        /// </summary>
        Stream = 3,
        /// <summary>
        /// �Զ�����󣬲���Ƕ�װ���ʽ���д����
        /// </summary>
        Object = 4,
        /// <summary>
        /// Ƕ�װ����Ͱ���ͬһ�����ͣ���������ʵ�����޼����Ľṹ��
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