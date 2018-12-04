/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Security.Cryptography;

namespace Symbol.Encryption {
    /// <summary>
    /// AES加密辅助类。
    /// </summary>
    public class AESEncryptionHelper {

        #region methods

        #region Encrypt
        /// <summary>
        /// 将一个流加密为内存流。
        /// </summary>
        /// <param name="stream">一个可读取的流。</param>
        /// <param name="key">key。</param>
        /// <param name="vector">vector。</param>
        /// <returns>返回加密后的数据。</returns>
        public static System.IO.MemoryStream Encrypt(System.IO.Stream stream, string key, string vector) {
            byte[] bKey = new byte[32];
            Array.Copy(System.Text.Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

            byte[] bVector = new byte[16];
            Array.Copy(System.Text.Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);


            using (var rijndaelAES = Rijndael.Create()) {
                var outStream = new System.IO.MemoryStream();
                using (var cryptoStream = new CryptoStream(
                                                    outStream,
                                                    rijndaelAES.CreateEncryptor(bKey, bVector),
                                                    CryptoStreamMode.Write
                                               )) {
                    int i = 0;
                    byte[] b = new byte[1024];

                    while ((i = stream.Read(b, 0, b.Length)) > 0) {
                        cryptoStream.Write(b, 0, i);
                    }
                    cryptoStream.FlushFinalBlock();
                    outStream.Position = 0;
                    return outStream;
                }
            }
          
        }
        /// <summary>
        /// 加密指定的数据。
        /// </summary>
        /// <param name="array">需要加密的数据。</param>
        /// <param name="key">key。</param>
        /// <param name="vector">vector。</param>
        /// <returns>返回加密后的数据。</returns>
        public static byte[] Encrypt(byte[] array, string key, string vector) {
            byte[] bKey = new byte[32];
            Array.Copy(System.Text.Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

            byte[] bVector = new byte[16];
            Array.Copy(System.Text.Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);

            using (var rijndaelAES = Rijndael.Create()) {
                var outStream = new System.IO.MemoryStream();
                using (var cryptoStream = new CryptoStream(
                                                    outStream,
                                                    rijndaelAES.CreateEncryptor(bKey, bVector),
                                                    CryptoStreamMode.Write
                                               )) {
                    byte[] b = new byte[1024];
                    cryptoStream.Write(array, 0, array.Length);
                    cryptoStream.FlushFinalBlock();
                    return outStream.ToArray();
                }
            }
        }
        /// <summary>
        /// 加密指定的数据（文本，UTF8编码）。
        /// </summary>
        /// <param name="text">需要加密的文本。</param>
        /// <param name="key">key。</param>
        /// <param name="vector">vector。</param>
        /// <returns>返回加密后的数据。</returns>
        public static string Encrypt(string text, string key, string vector) {
            if (string.IsNullOrEmpty(text))
                return text;
            byte[] array = Encrypt(System.Text.Encoding.UTF8.GetBytes(text), key, vector);
            return ByteExtensions.ToHex(array);// System.BitConverter.ToString(array).Replace("-", "");
        }
        #endregion

        #region Decrypt
        /// <summary>
        /// 将一个流解密为内存流。
        /// </summary>
        /// <param name="stream">一个可读取的流。</param>
        /// <param name="key">key。</param>
        /// <param name="vector">vector。</param>
        /// <returns>返回解密后的数据。</returns>
        public static System.IO.MemoryStream Decrypt(System.IO.Stream stream, string key, string vector) {
            byte[] bKey = new byte[32];
            Array.Copy(System.Text.Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

            byte[] bVector = new byte[16];
            Array.Copy(System.Text.Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);

            CryptoStream cryptoStream = null;
            System.IO.MemoryStream memoryResult = null;
            Rijndael rijndaelAES = Rijndael.Create();
            try {
                cryptoStream = new CryptoStream(stream,
                    rijndaelAES.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read);
                memoryResult = new System.IO.MemoryStream();
                byte[] buffer = new byte[1024];
                int readBytes = 0;
                while ((readBytes = cryptoStream.Read(buffer, 0, buffer.Length)) > 0) {
                    memoryResult.Write(buffer, 0, readBytes);
                }
                memoryResult.Position = 0;
            } catch {
                throw;
            } finally {
                if (cryptoStream != null) {
                    cryptoStream.Close();
                    cryptoStream.Dispose();
                    cryptoStream = null;
                }
            }

            return memoryResult;
        }
        /// <summary>
        /// 解密指定的数据。
        /// </summary>
        /// <param name="array">需要解密的数据。</param>
        /// <param name="key">key。</param>
        /// <param name="vector">vector。</param>
        /// <returns>返回解密后的数据。</returns>
        public static byte[] Decrypt(byte[] array, string key, string vector) {
            System.IO.MemoryStream memoryStream = null;
            System.IO.MemoryStream memoryResult = null;
            byte[] result = null;
            try {
                memoryStream = new System.IO.MemoryStream(array);
                memoryResult = Decrypt(memoryStream, key, vector);
                result = memoryResult.ToArray();
            } finally {
                if (memoryResult != null) {
                    memoryResult.Close();
                    memoryResult.Dispose();
                    memoryResult = null;
                }
                if (memoryStream != null) {
                    memoryStream.Close();
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }

            return result;
        }
        /// <summary>
        /// 解密指定的数据（文本，UTF8编码）。
        /// </summary>
        /// <param name="text">需要解密的文本。</param>
        /// <param name="key">key。</param>
        /// <param name="vector">vector。</param>
        /// <returns>返回解密后的数据。</returns>
        public static string Decrypt(string text, string key, string vector) {
            if (string.IsNullOrEmpty(text))
                return text;
            if (text.Length % 2 != 0)
                CommonException.ThrowArgument("text");
                //throw new EncryptTextNotSymmetryException();

            //byte[] array = new byte[text.Length / 2];
            //for (int i = 0, j = 0; i < text.Length; i += 2, j++) {
            //    array[j] = byte.Parse(text.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            //}
            byte[] bResult = Decrypt(ByteExtensions.HexToBytes(text), key, vector);
            return System.Text.Encoding.UTF8.GetString(bResult);

        }
        #endregion

        #endregion
    }

}