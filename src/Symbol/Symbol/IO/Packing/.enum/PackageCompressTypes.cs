/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// 数据包单项压缩方式,00-99。
    /// </summary>
    /// <remarks>算法选择方式：压缩和解压的双方实现平台来定，然后挑选速度快，压缩比中等的就行了。</remarks>
    public enum PackageCompressTypes : byte {
        /// <summary>
        /// 不启用压缩
        /// </summary>
        None = 0,
        /// <summary>
        /// QuickLZ，据说全世界最快的，它有一个Quick_all_version.dll，跟随在一起时，会提升它的速度，反之采用的C#代码。
        /// </summary>
        QuickLZ = 1,
        /// <summary>
        /// 接触过压缩算法的人都知道它，速度快，压缩比一般。
        /// </summary>
        Gzip = 2,
        /// <summary>
        /// Lzma，7Z的算法，压缩比虽然高，但是速度较慢。
        /// </summary>
        Lzma7z = 3,
        /// <summary>
        /// Zlib，速度快，但是实现的时候调用的是DLL，在没有文件释放权限的时候不建议用它。
        /// </summary>
        Zlibwapi = 4,
        /// <summary>
        /// LZSS和Zlibwapi差不多，不过Normal压缩比不理想
        /// </summary>
        LZSS_Normal = 10,
        /// <summary>
        /// 此模式能应付很多的压缩类型了
        /// </summary>
        LZSS_Bytes_Strings = 11,
        /// <summary>
        /// 略显得慢些
        /// </summary>
        LZSS_Lazy_Matching = 12,
        /// <summary>
        /// 压缩比和速度一般
        /// </summary>
        LZW_Static = 16,
        /// <summary>
        /// 压缩比和速度一般
        /// </summary>
        LZW_Predefined = 17,

        /// <summary>
        /// 哈夫曼算法中比较看好的一种
        /// </summary>
        Huffman_NonGreedy_1 = 20,
        /// <summary>
        /// 哈夫曼算法中比较看好的一种
        /// </summary>
        Huffman_NonGreedy_2 = 21,
        /// <summary>
        /// 不加密不压缩，此功能需要Version2支持
        /// </summary>
        NonEncrypt_NonCompress=22,
    }
}