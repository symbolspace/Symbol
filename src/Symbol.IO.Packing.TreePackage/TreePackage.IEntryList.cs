/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    partial class TreePackage {
        /// <summary>
        /// ���б�ӿ�
        /// </summary>
        public interface IEntryList {
            /// <summary>
            /// ��ȡ��ֵ��Ӧ����
            /// </summary>
            /// <param name="key">��ֵ����������ڣ������� null ��</param>
            /// <returns>���ش˼�ֵ��Ӧ���</returns>
            Entry this[string key] { get; }
        }
    }
}