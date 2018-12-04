/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
#if !net20
using System;

namespace Symbol.Encryption {
    /// <summary>
    /// 标准AES加密类，适应Java、PHP。
    /// </summary>
    public class GeneralAESEncryptionHelper {

        #region methods

        #region Encrypt
        /// <summary>
        /// 加密（文本，返回为base64字符。）
        /// </summary>
        /// <param name="text">要加密的文本，为空或空文本将采用new byte[0]。</param>
        /// <param name="password">密钥</param>
        /// <param name="encoding">文本编码,为null将采用utf8编码。</param>
        /// <returns>返回加密后的数据。</returns>
        public static string EncryptBase64(string text, string password, System.Text.Encoding encoding = null) {
            return Convert.ToBase64String(Encrypt(text, password, encoding));
        }
        /// <summary>
        /// 加密（文本）
        /// </summary>
        /// <param name="text">要加密的文本，为空或空文本将采用new byte[0]。</param>
        /// <param name="password">密钥</param>
        /// <param name="encoding">文本编码,为null将采用utf8编码。</param>
        /// <returns>返回加密后的数据。</returns>
        public static byte[] Encrypt(string text, string password, System.Text.Encoding encoding = null) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;

            byte[] buffer = string.IsNullOrEmpty(text) ? new byte[0] : encoding.GetBytes(text);
            return Encrypt(buffer, password, encoding);
        }
        /// <summary>
        /// 加密（二进制，返回为base64字符。）
        /// </summary>
        /// <param name="data">需要加密的数据。</param>
        /// <param name="password">密钥</param>
        /// <param name="encoding">文本编码,为null将采用utf8编码。</param>
        /// <returns>返回加密后的数据。</returns>
        public static string EncryptBase64(byte[] data, string password, System.Text.Encoding encoding = null) {
            return Convert.ToBase64String(Encrypt(data, password, encoding));
        }
        /// <summary>
        /// 加密（二进制）
        /// </summary>
        /// <param name="data">需要加密的数据。</param>
        /// <param name="password">密钥</param>
        /// <param name="encoding">文本编码,为null将采用utf8编码。</param>
        /// <returns>返回加密后的数据。</returns>
        public static byte[] Encrypt(byte[] data, string password, System.Text.Encoding encoding = null) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            //password = Md5_16(password);
            return Encrypt(data, encoding.GetBytes(password));
        }
        /// <summary>
        /// 加密（二进制）
        /// </summary>
        /// <param name="data">需要加密的数据。</param>
        /// <param name="password">密钥</param>
        /// <returns>返回加密后的数据。</returns>
        public static byte[] Encrypt(byte[] data, byte[] password) {
#if netcore
            System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
#else
            System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create("AES");
#endif
            aes.Mode = System.Security.Cryptography.CipherMode.ECB;
            aes.Key = password;
            System.Security.Cryptography.ICryptoTransform encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        #endregion
        #region Decrypt
        /// <summary>
        /// 解密（base64，结果是文本）
        /// </summary>
        /// <param name="base64">需要解密的数据。</param>
        /// <param name="password">密钥</param>
        /// <param name="encoding">文本编码,为null将采用utf8编码。</param>
        /// <returns></returns>
        public static string DecryptBase64Text(string base64, string password, System.Text.Encoding encoding = null) {
            byte[] buffer = DecryptBase64(base64, password, encoding);
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(buffer);
        }
        /// <summary>
        /// 解密（base64）
        /// </summary>
        /// <param name="base64">需要解密的数据。</param>
        /// <param name="password">密钥</param>
        /// <param name="encoding">文本编码,为null将采用utf8编码。</param>
        /// <returns></returns>
        public static byte[] DecryptBase64(string base64, string password, System.Text.Encoding encoding = null) {
            return Decrypt(Convert.FromBase64String(base64), password, encoding);
        }
        /// <summary>
        /// 解密（二进制）
        /// </summary>
        /// <param name="data">需要解密的数据。</param>
        /// <param name="password">密钥</param>
        /// <param name="encoding">文本编码,为null将采用utf8编码。</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, string password, System.Text.Encoding encoding = null) {
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            //password = Md5_16(password);
            return Decrypt(data, encoding.GetBytes(password));
        }
        /// <summary>
        /// 解密（二进制）
        /// </summary>
        /// <param name="data">需要解密的数据。</param>
        /// <param name="password">密钥</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, byte[] password) {
#if netcore
            System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
#else
            System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create("AES");
#endif
            aes.Mode = System.Security.Cryptography.CipherMode.ECB;
            aes.Key = password;
            System.Security.Cryptography.ICryptoTransform encryptor = aes.CreateDecryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        #endregion

        #endregion
    }

}
#endif