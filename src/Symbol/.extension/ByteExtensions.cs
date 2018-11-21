/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Byte扩展类。
/// </summary>
public static class ByteExtensions {

    #region fields
    private static readonly byte[] _emptyArray = new byte[0];
    #endregion

    #region cctor
    #endregion

    #region methods

    #region Find
    /// <summary>
    /// 查询指定的数据在源数据中的位置（从0开始，不输出endKey，任何不合法调用都会返回new byte[0]）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <returns>返回读取到的数据，不匹配时返回new byte[0]。</returns>
    public static byte[] Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey) {
        byte[] data;
        if (Find(source, endKey, 0, false, out data) == -1) {
            return _emptyArray;
        }
        return data;
    }
    /// <summary>
    /// 查询指定的数据在源数据中的位置（从0开始，任何不合法调用都会返回new byte[0]）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="outEndKey">是否将endKey的数据包含在data内。</param>
    /// <returns>返回读取到的数据，不匹配时返回new byte[0]。</returns>
    public static byte[] Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey, bool outEndKey) {
        byte[] data;
        if (Find(source, endKey, 0, outEndKey, out data) == -1) {
            return _emptyArray;
        }
        return data;
    }
    /// <summary>
    /// 查询指定的数据在源数据中的位置（不输出endKey，任何不合法调用都会返回new byte[0]）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="startIndex">起始位置，从0开始，小于0或超出source的有效范围，直接返回 -1。</param>
    /// <returns>返回读取到的数据，不匹配时返回new byte[0]。</returns>
    public static byte[] Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey, int startIndex) {
        byte[] data;
        if (Find(source, endKey, startIndex, false, out data) == -1) {
            return _emptyArray;
        }
        return data;
    }
    /// <summary>
    /// 查询指定的数据在源数据中的位置（任何不合法调用都会返回new byte[0]）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="startIndex">起始位置，从0开始，小于0或超出source的有效范围，直接返回 -1。</param>
    /// <param name="outEndKey">是否将endKey的数据包含在data内。</param>
    /// <returns>返回读取到的数据，不匹配时返回new byte[0]。</returns>
    public static byte[] Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey, int startIndex, bool outEndKey) {
        byte[] data;
        if (Find(source, endKey, startIndex, outEndKey, out data) == -1) {
            return _emptyArray;
        }
        return data;
    }

    /// <summary>
    /// 查询指定的数据在源数据中的位置（从0开始，不输出endKey，任何不合法调用都会返回-1）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="data">输出读取到的数据，如果读取失败始终返回new byte[0]。</param>
    /// <returns>返回读取之后的起始位置，会向后推移endKey的长度。</returns>
    public static int Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey, out byte[] data) {
        return Find(source, endKey, 0, false, out data);
    }
    /// <summary>
    /// 查询指定的数据在源数据中的位置（从0开始，任何不合法调用都会返回-1）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="outEndKey">是否将endKey的数据包含在data内。</param>
    /// <param name="data">输出读取到的数据，如果读取失败始终返回new byte[0]。</param>
    /// <returns>返回读取之后的起始位置，会向后推移endKey的长度。</returns>
    public static int Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey, bool outEndKey, out byte[] data) {
        return Find(source, endKey, 0, outEndKey, out data);
    }
    /// <summary>
    /// 查询指定的数据在源数据中的位置（不输出endKey，任何不合法调用都会返回-1）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="startIndex">起始位置，从0开始，小于0或超出source的有效范围，直接返回 -1。</param>
    /// <param name="data">输出读取到的数据，如果读取失败始终返回new byte[0]。</param>
    /// <returns>返回读取之后的起始位置，会向后推移endKey的长度。</returns>
    public static int Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey, int startIndex, out byte[] data) {
        return Find(source, endKey, startIndex, false, out data);
    }
    /// <summary>
    /// 查询指定的数据在源数据中的位置（任何不合法调用都会返回-1）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="endKey">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="startIndex">起始位置，从0开始，小于0或超出source的有效范围，直接返回 -1。</param>
    /// <param name="outEndKey">是否将endKey的数据包含在data内。</param>
    /// <param name="data">输出读取到的数据，如果读取失败始终返回new byte[0]。</param>
    /// <returns>返回读取之后的起始位置，会向后推移endKey的长度。</returns>
    public static int Find(
#if !net20
        this
#endif
        byte[] source, byte[] endKey, int startIndex, bool outEndKey, out byte[] data) {
        int index = FindIndex(source, endKey, startIndex);
        if (index == -1) {
            data = _emptyArray;
            return -1;
        }

        int keyLength = endKey.Length;

        int length = index - startIndex;
        if (outEndKey)
            length += keyLength;

        data = new byte[length];
        Buffer.BlockCopy(source, startIndex, data, 0, length);
        if (outEndKey)
            index += keyLength;
        return index;
    }
    #endregion

    #region FindIndex
    /// <summary>
    /// 查询指定的数据在源数据中的位置（任何不合法调用都会返回-1）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="key">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <returns>如果找不到返回-1，则返回从0开始的索引位置。</returns>
    public static int FindIndex(
#if !net20
        this
#endif
        byte[] source, byte[] key) {
        return FindIndex(source, key, 0);
    }
    /// <summary>
    /// 查询指定的数据在源数据中的位置（任何不合法调用都会返回-1）。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="key">要查找的数据，为null或空白数组，直接返回 -1 。</param>
    /// <param name="startIndex">起始位置，从0开始，小于0或超出source的有效范围，直接返回 -1。</param>
    /// <returns>如果找不到返回-1，则返回从0开始的索引位置。</returns>
    public static int FindIndex(
#if !net20
        this
#endif
        byte[] source, byte[] key, int startIndex) {
        if (
               source == null || source.Length == 0
            || key == null || key.Length == 0
            || startIndex < 0 || startIndex > source.Length - 1
            || (source.Length - 1 - startIndex - key.Length) < 0
        )
            return -1;
        int sourceLength = source.Length;
        int keyLength = key.Length;
        int length = sourceLength - keyLength;
        for (int i = startIndex; i < length; i++) {
            if (source[i] == key[0]) {
                if (keyLength == 1)
                    return i;
                bool find = true;
                for (int j = 1; j < keyLength; j++) {
                    if (source[i + j] != key[j]) {
                        find = false;
                        break;
                    }
                }
                if (find)
                    return i;
            }
        }
        return -1;
    }
    #endregion

    #region Sub
    /// <summary>
    /// 子数组（超出范围不会报错，均为new byte[0]）
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 new byte[0]</param>
    /// <param name="index">起始位置，从0开始，小于0或超出source的有效范围，直接返回new byte[0]</param>
    /// <returns>返回新的数据。</returns>
    public static byte[] Sub(
#if !net20
        this
#endif
        byte[] source, int index) {
        return Sub(source, index, -1);
    }
    /// <summary>
    /// 子数组（超出范围不会报错，均为new byte[0]）
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 new byte[0]</param>
    /// <param name="index">起始位置，从0开始，小于0或超出source的有效范围，直接返回new byte[0]</param>
    /// <param name="length">长度，从起始位置开始计算，传-1表示剩余的所有数据；超出有效范围时 返回 new byte[0]。</param>
    /// <returns>返回新的数据。</returns>
    public static byte[] Sub(
#if !net20
        this
#endif
        byte[] source, int index, int length) {
        if (source == null || source.Length == 0 || index < 0 || length == 0 || index > (source.Length - 1)) {
            return _emptyArray;
        }

        int length2 = source.Length - 1 - index;
        if (length > 0 && length < length2) {
            length2 = length;
        }
        byte[] array = new byte[length2];
        Buffer.BlockCopy(source, index, array, 0, length2);
        return array;
    }
    #endregion

    #region ToHex
    /// <summary>
    /// 转换为16进制文本(大写字母)。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 ""</param>
    /// <returns></returns>
    public static string ToHex(
#if !net20
        this
#endif
        byte[] source) {
        return ToHex(source, true);
    }
    /// <summary>
    /// 转换为16进制文本。
    /// </summary>
    /// <param name="source">源数据，为null或空白数组，直接返回 ""</param>
    /// <param name="upper">是否为大写的字母</param>
    /// <returns></returns>
    public static string ToHex(
#if !net20
        this
#endif
        byte[] source, bool upper) {
        if (source == null || source.Length == 0)
            return "";
        int length = source.Length;
        int length2 = 0;
        char[] buffer = new char[length * 2];
        string hex = upper ? _hex_upper : _hex_lower;
        for (int i = 0; i < length; i++) {
            byte value = source[i];
            buffer[length2++] = hex[value / 16];
            buffer[length2++] = hex[value % 16];
        }
        return new string(buffer);
    }
    private static readonly string _hex_lower = "0123456789abcdef";
    private static readonly string _hex_upper = "0123456789ABCDEF";


    #endregion

    #region HexToBytes
    /// <summary>
    /// 从16进制转换为二进制。
    /// </summary>
    /// <param name="value">16进制文本，如果是0x开头，自动忽略0x</param>
    /// <returns></returns>
    public static byte[] HexToBytes(
#if !net20
        this
#endif
        string value) {
        if (string.IsNullOrEmpty(value))
            return new byte[0];
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            value = value.Substring(2);
        value = value.Replace(" ", "").Replace("-", "");
        if (value.Length % 2 > 0) {
            value = " " + value;
        }
        byte[] data = new byte[value.Length / 2];
        for (int i = 0; i < data.Length; i++) {
            //data[i] = byte.Parse(value.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            data[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
        }
        return data;
    }
    #endregion

    #endregion

}