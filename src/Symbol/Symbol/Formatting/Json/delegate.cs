/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Formatting.Json {
    /// <summary>
    /// 将对象转为json文本。
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public delegate string JsonSerializeDelegate(object data);
    /// <summary>
    /// 将对象转为json文本。
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public delegate string JsonSerializeDelegate<T>(T data);
    /// <summary>
    /// 将json文本转为对象。
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public delegate object JsonDeserializeDelegate(string data);
    /// <summary>
    /// 将json文本转为对象。
    /// </summary>
    /// <param name="model"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public delegate void JsonDeserializeDelegate<T>(T model, string data) where T : class, new();


}