/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// ���ݰ����ܷ�ʽ,0-9
    /// </summary>
    public enum PackageEncryptTypes : byte {
        /// <summary>
        /// �����Ʋ��μ��ܣ�Ĭ�����룬�ɴ���ͽ��˫������Ĭ���Ƕ��٣�
        /// </summary>
        BinaryWave_DefaultPassword = 0 ,
        /// <summary>
        /// �����Ʋ��μ��ܣ��Զ������룩
        /// </summary>
        BinaryWave_CustomPassword = 1,
        /// <summary>
        /// �����Ʋ��μ��ܣ������룩
        /// </summary>
        BinaryWave_EmptyPassword=2,
    }
}