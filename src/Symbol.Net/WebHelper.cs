/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

 using System.Collections.Generic;
namespace Symbol.Net {
    /// <summary>
    /// Web辅助类。
    /// </summary>
    public static class WebHelper {

        #region fields
        //multipart/form-data
        /// <summary>
        /// ContentType: multipart/form-data; boundary=---------------------------7dc184012027a
        /// </summary>
        public static readonly string MulitPartFormContentType = "multipart/form-data; boundary=---------------------------7dc184012027a";
        //private static readonly byte[] _mulitPartFormFlag = System.Text.Encoding.ASCII.GetBytes("-----------------------------7dc184012027a\r\n");
        private static readonly byte[] _mulitPartFormEndFalg = System.Text.Encoding.ASCII.GetBytes("-----------------------------7dc184012027a--\r\n");
        private static readonly byte[] _mulitPartFormKeyBefore = System.Text.Encoding.ASCII.GetBytes("-----------------------------7dc184012027a\r\nContent-Disposition: form-data; name=\"");
        private static readonly byte[] _mulitPartFormKeyAfter = System.Text.Encoding.ASCII.GetBytes("\"\r\n\r\n");
        private static readonly byte[] _mulitPartFormSpliter = System.Text.Encoding.ASCII.GetBytes("\r\n");
        #endregion

        #region methods

        #region MulitPartFormData
        /// <summary>
        /// 将传入的数据，进行multipart/form-data编码
        /// </summary>
        /// <param name="values">需要编码的值</param>
        /// <param name="encoding">字符编码类型，如果传null将自动采用UTF-8</param>
        /// <returns>返回编码后的数据。</returns>
        /// <remarks>value的类型可以是基础类型、array、IEnumerable。</remarks>
        public static byte[] MulitPartFormData(IDictionary<string, object> values, System.Text.Encoding encoding = null) {
            byte[] result = null;
            if (encoding == null)
                encoding = System.Text.Encoding.UTF8;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream()) {
                //stream.Write(_mulitPartFormFlag, 0, _mulitPartFormFlag.Length);
                if (values != null) {
                    foreach (KeyValuePair<string, object> item in values) {
                        WriteMulitPart_Any(item.Key, item.Value, stream, encoding);
                    }
                }
                stream.Write(_mulitPartFormEndFalg, 0, _mulitPartFormEndFalg.Length);
                result = stream.ToArray();
            }
            return result;
        }
        static void WriteMulitPart_Key(string key, System.IO.Stream stream) {
            stream.Write(_mulitPartFormKeyBefore, 0, _mulitPartFormKeyBefore.Length);
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(key);
            stream.Write(buffer, 0, buffer.Length);
            stream.Write(_mulitPartFormKeyAfter, 0, _mulitPartFormKeyAfter.Length);
        }
        static void WriteMulitPart_Spliter(System.IO.Stream stream) {
            stream.Write(_mulitPartFormSpliter, 0, _mulitPartFormSpliter.Length);
        }
        static void WriteMulitPart_String(string key, string value, System.IO.Stream stream, System.Text.Encoding encoding) {
            WriteMulitPart_Key(key, stream);
            byte[] buffer = null;
            if (value == null)
                value = string.Empty;
            buffer = encoding.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
            WriteMulitPart_Spliter(stream);
        }
        static void WriteMulitPart_ByteArray(string key, byte[] value, System.IO.Stream stream) {
            WriteMulitPart_Key(key, stream);
            if (value != null)
                stream.Write(value, 0, value.Length);
            WriteMulitPart_Spliter(stream);
        }
        static void WriteMulitPart_Bytes(string key, IEnumerable<byte> value, System.IO.Stream stream) {
            WriteMulitPart_Key(key, stream);
            if (value != null) {
                foreach (byte b in value) {
                    stream.WriteByte(b);
                }
            }
            WriteMulitPart_Spliter(stream);
        }
        static void WriteMulitPart_Mulit(string key, System.Collections.IEnumerable value, System.IO.Stream stream, System.Text.Encoding encoding) {
            foreach (object item in (System.Collections.IEnumerable)value) {
                WriteMulitPart_Any(key, item, stream, encoding);
            }
        }
        static void WriteMulitPart_Item(string key, MulitPartItem value, System.IO.Stream stream) {
            /*
-----------------------------7dc19db140126
Content-Disposition: form-data; name="litpic"; filename=""
Content-Type: application/octet-stream


-----------------------------7dc19db140126*/

            stream.Write(_mulitPartFormKeyBefore, 0, _mulitPartFormKeyBefore.Length);
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(key);
            stream.Write(buffer, 0, buffer.Length);
            if (value.Attributes != null && value.Attributes.Count > 0) {
                foreach (KeyValuePair<string, object> pair in value.Attributes) {
                    buffer = System.Text.Encoding.ASCII.GetBytes("\"; " + pair.Key + "=\"" + (pair.Value == null ? string.Empty : pair.Value.ToString()));
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            if (value.Headers != null && value.Headers.Count > 0) {
                buffer = System.Text.Encoding.ASCII.GetBytes("\"\r\n");
                stream.Write(buffer, 0, buffer.Length);
                string line = null;
                foreach (KeyValuePair<string, object> pair in value.Headers) {
                    line = pair.Key + ": " + pair.Value + "\r\n";
                    buffer = System.Text.Encoding.ASCII.GetBytes(line);
                    stream.Write(buffer, 0, buffer.Length);
                }
                line = "\r\n";
                buffer = System.Text.Encoding.ASCII.GetBytes(line);
                stream.Write(buffer, 0, buffer.Length);
            } else {
                stream.Write(_mulitPartFormKeyAfter, 0, _mulitPartFormKeyAfter.Length);
            }
            if (value.Value != null)
                stream.Write(value.Value, 0, value.Value.Length);
            WriteMulitPart_Spliter(stream);
        }
        static void WriteMulitPart_Any(string key, object value, System.IO.Stream stream, System.Text.Encoding encoding) {
            if (value is string || value == null) {
                WriteMulitPart_String(key, (string)value, stream, encoding);
                return;
            }
            MulitPartItem item = value as MulitPartItem;
            if (item != null) {
                WriteMulitPart_Item(key, item, stream);
                return;
            }

            byte[] valueByteArray = value as byte[];
            if (valueByteArray != null) {
                WriteMulitPart_ByteArray(key, valueByteArray, stream);
                return;
            }
            IEnumerable<byte> valueByteE = value as IEnumerable<byte>;
            if (valueByteE != null) {
                WriteMulitPart_Bytes(key, valueByteE, stream);
                return;
            }
            System.Collections.IEnumerable e = value as System.Collections.IEnumerable;
            if (e != null) {
                WriteMulitPart_Mulit(key, e, stream, encoding);
                return;
            }
            WriteMulitPart_String(key, value == null ? string.Empty : value.ToString(), stream, encoding);
        }
        #endregion

        #endregion

        #region classes
        /// <summary>
        /// 多节点数据项
        /// </summary>
        public class MulitPartItem {
            /// <summary>
            /// 字段名称
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// 附加属性列表，通常是用于上传文件时，filename=""
            /// </summary>
            public IDictionary<string, object> Attributes { get; private set; }
            /// <summary>
            /// 附加Header，通常是用于上传文件时，Content-Type: application/octet-stream
            /// </summary>
            public IDictionary<string, object> Headers { get; set; }
            /// <summary>
            /// 字段值
            /// </summary>
            public byte[] Value { get; set; }

            /// <summary>
            /// 构造一个多节点数据项
            /// </summary>
            /// <param name="name">字段名称</param>
            /// <param name="value">字段值</param>
            public MulitPartItem(string name, byte[] value) {
                Name = name;
                Value = value;
                Attributes = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
                Headers = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
            }
        }
        #endregion
    }

}