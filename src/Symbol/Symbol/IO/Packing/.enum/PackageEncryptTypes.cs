/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.IO.Packing {
    /// <summary>
    /// 数据包加密方式,0-9
    /// </summary>
    public enum PackageEncryptTypes : byte {
        /// <summary>
        /// 二进制波形加密（默认密码，由打包和解包双方决定默认是多少）
        /// </summary>
        BinaryWave_DefaultPassword = 0 ,
        /// <summary>
        /// 二进制波形加密（自定义密码）
        /// </summary>
        BinaryWave_CustomPassword = 1,
        /// <summary>
        /// 二进制波形加密（无密码）
        /// </summary>
        BinaryWave_EmptyPassword=2,
    }
}