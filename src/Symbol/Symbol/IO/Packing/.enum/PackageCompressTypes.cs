/*  
 *  author��symbolspace
 *  e-mail��symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// ���ݰ�����ѹ����ʽ,00-99��
    /// </summary>
    /// <remarks>�㷨ѡ��ʽ��ѹ���ͽ�ѹ��˫��ʵ��ƽ̨������Ȼ����ѡ�ٶȿ죬ѹ�����еȵľ����ˡ�</remarks>
    public enum PackageCompressTypes : byte {
        /// <summary>
        /// ������ѹ��
        /// </summary>
        None = 0,
        /// <summary>
        /// QuickLZ����˵ȫ�������ģ�����һ��Quick_all_version.dll��������һ��ʱ�������������ٶȣ���֮���õ�C#���롣
        /// </summary>
        QuickLZ = 1,
        /// <summary>
        /// �Ӵ���ѹ���㷨���˶�֪�������ٶȿ죬ѹ����һ�㡣
        /// </summary>
        Gzip = 2,
        /// <summary>
        /// Lzma��7Z���㷨��ѹ������Ȼ�ߣ������ٶȽ�����
        /// </summary>
        Lzma7z = 3,
        /// <summary>
        /// Zlib���ٶȿ죬����ʵ�ֵ�ʱ����õ���DLL����û���ļ��ͷ�Ȩ�޵�ʱ�򲻽���������
        /// </summary>
        Zlibwapi = 4,
        /// <summary>
        /// LZSS��Zlibwapi��࣬����Normalѹ���Ȳ�����
        /// </summary>
        LZSS_Normal = 10,
        /// <summary>
        /// ��ģʽ��Ӧ���ܶ��ѹ��������
        /// </summary>
        LZSS_Bytes_Strings = 11,
        /// <summary>
        /// ���Ե���Щ
        /// </summary>
        LZSS_Lazy_Matching = 12,
        /// <summary>
        /// ѹ���Ⱥ��ٶ�һ��
        /// </summary>
        LZW_Static = 16,
        /// <summary>
        /// ѹ���Ⱥ��ٶ�һ��
        /// </summary>
        LZW_Predefined = 17,

        /// <summary>
        /// �������㷨�бȽϿ��õ�һ��
        /// </summary>
        Huffman_NonGreedy_1 = 20,
        /// <summary>
        /// �������㷨�бȽϿ��õ�һ��
        /// </summary>
        Huffman_NonGreedy_2 = 21,
        /// <summary>
        /// �����ܲ�ѹ�����˹�����ҪVersion2֧��
        /// </summary>
        NonEncrypt_NonCompress=22,
    }
}