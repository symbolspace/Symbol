/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
using System;
namespace Symbol.IO.Packing {
        /// <summary>
    /// �Զ������ӿ�
    /// </summary>
    public interface ICustomPackage {
        /// <summary>
        /// ��������Ϊbyte[]
        /// </summary>
        /// <param name="instance">��Ҫ�����ʵ����û��null�������</param>
        /// <returns>���ش��������ݡ�</returns>
        byte[] Save(object instance);
        /// <summary>
        /// ��byte[]�м��ض���
        /// </summary>
        /// <param name="buffer">��������ݡ�</param>
        /// <returns>���ؽ�����Ķ���</returns>
        object Load(byte[] buffer);
    }

}