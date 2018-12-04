/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
namespace Symbol.Encryption {
    
    /// <summary>
    /// SHA算法级别集
    /// </summary>
    [Const("SHA算法级别集")]
    public enum SHALevels {
        /// <summary>
        /// Sha-1
        /// </summary>
        [Const("Sha-1")]
        Sha1 = 1,
        /// <summary>
        /// Sha-256
        /// </summary>
        [Const("Sha-256")]
        Sha256 = 256,
        /// <summary>
        /// Sha-384
        /// </summary>
        [Const("Sha-384")]
        Sha384 = 384,
        /// <summary>
        /// Sha-512
        /// </summary>
        [Const("Sha-512")]
        Sha512 = 512,
    }
}