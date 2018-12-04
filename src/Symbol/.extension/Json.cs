/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

#define Mapper

namespace Symbol.Serialization {

    /// <summary>
    /// Json相关操作集。
    /// </summary>
    public class Json {


        #region methods

        #region Parse
        /// <summary>
        /// 解析json
        /// </summary>
        /// <param name="json">json文本</param>
        /// <param name="throwError">是否需要抛出异常</param>
        /// <returns>返回json对象。</returns>
        /// <remarks>如果json文本是数组就是List&lt;object&gt;,如果是自定义对象就是JsonObject,如果解析异常或空json文本就是null。</remarks>
        public static object Parse(string json,bool throwError=false) {
            if (string.IsNullOrEmpty(json))
                return null;
            if (throwError) {
                return JSON.Parse(json);
                //return (new JavaScriptSerializer() ).DeserializeObject(json);
            } else {
                try {
                    return JSON.Parse(json);
                    //return (new JavaScriptSerializer() ).DeserializeObject(json);
                } catch {
                    return null;
                }
            }
        }
        /// <summary>
        /// 解析json
        /// </summary>
        /// <param name="json">json文本</param>
        /// <param name="type">类型</param>
        /// <param name="throwError">是否需要抛出异常</param>
        /// <returns>返回json对象。</returns>
        /// <remarks>如果json文本是数组就是List&lt;object&gt;,如果是自定义对象就是JsonObject,如果解析异常或空json文本就是null。</remarks>
        public static object Parse(string json,System.Type type, bool throwError = false) {
            if (string.IsNullOrEmpty(json))
                return null;
            if (throwError) {
                if (type == null) {
                    return JSON.Parse(json);
                }
                return JSON.ToObject(json, type);
                //return (new JavaScriptSerializer() ).DeserializeObject(json);
            } else {
                try {
                    if (type == null) {
                        return JSON.Parse(json);
                    }
                    return JSON.ToObject(json, type);
                    //return (new JavaScriptSerializer() ).DeserializeObject(json);
                } catch {
                    return null;
                }
            }
        }
        #endregion
        #region Parse`1
        /// <summary>
        /// 解析json(泛型)
        /// </summary>
        /// <typeparam name="T">任意类型</typeparam>
        /// <param name="json">json文本</param>
        /// <param name="throwError">是否需要抛出异常</param>
        /// <returns>返回json对象。</returns>
        public static T Parse<T>(string json, bool throwError = false) {
            if (string.IsNullOrEmpty(json))
                return default(T);
            if (throwError) {
                return JSON.ToObject<T>(json);
                //return (new JavaScriptSerializer()).Deserialize<T>(json);
            } else {
                try {
                    return JSON.ToObject<T>(json);
                    //return (new JavaScriptSerializer()).Deserialize<T>(json);
                } catch {
                    return default(T);
                }
            }
        }
        #endregion

        #region ToString
        /// <summary>
        /// 将json对象转换为json文本
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="throwError">是否需要抛出异常</param>
        /// <returns>返回json文本。</returns>
        /// <remarks>如果传入的是空对象，将返回 "null" 。</remarks>
        public static string ToString(object json, bool throwError = false) {
            return ToString(json, throwError, false);
        }
        /// <summary>
        /// 将json对象转换为json文本
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="throwError">是否需要抛出异常</param>
        /// <param name="formated">是否格式化。</param>
        /// <returns>返回json文本。</returns>
        /// <remarks>如果传入的是空对象，将返回 "null" 。</remarks>
        public static string ToString(object json, bool throwError,bool formated=false) {
            if (json == null)
                return "null";
            if (throwError) {
                return !formated ? JSON.ToJSON(json): JSON.ToNiceJSON(json);
                //return (new JavaScriptSerializer() { Formated=formated }).Serialize(json);
            } else {
                try {
                    return !formated ? JSON.ToJSON(json) : JSON.ToNiceJSON(json);
                    //return (new JavaScriptSerializer() { Formated=formated }).Serialize(json);
                } catch {
                    return "null";
                }
            }
        }
        #endregion

        #region Format
        /// <summary>
        /// 美化Json(忽略异常)
        /// </summary>
        /// <param name="json">Json文本</param>
        /// <returns>返回格式化后的json。</returns>
        public static string Format(string json) {
            return Format(json, false);
        }
        /// <summary>
        /// 美化Json
        /// </summary>
        /// <param name="json">Json文本</param>
        /// <param name="throwError">是否需要抛出异常</param>
        /// <returns>返回格式化后的json。</returns>
        public static string Format(string json, bool throwError ) {
            if (string.IsNullOrEmpty(json))
                return json;

            if (throwError) {
                return JSON.Beautify(json);
            } else {
                try {
                    return JSON.Beautify(json);
                } catch {
                    return json;
                }
            }
        }
        #endregion

        #region Path
        /// <summary>
        /// path，快速获取值。
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="path">操作路径。字典key:"aa";数组索引:[0];组合使用:"data.items[0].name"。</param>
        /// <returns>返回最终的值。</returns>
        [System.Obsolete("请更改为FastObject.Path")]
        public static object Path(object instance, string path) {
            return FastObject.Path(instance, path);
        }
        /// <summary>
        /// path，快速设置值。
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="path">操作路径。字典key:"aa";数组索引:[0];组合使用:"data.items[0].name"。</param>
        /// <param name="value">要设置的值</param>
        /// <returns>返回是否操作成功。</returns>
        [System.Obsolete("请更改为FastObject.Path")]
        public static bool Path(object instance, string path, object value) {
            return FastObject.Path(instance, path, value);
        }
        #endregion


        #endregion


    }
}


