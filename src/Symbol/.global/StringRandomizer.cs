﻿/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
/// <summary>
/// 字符串随机类，辅助生成一些随机的字符串序列。
/// </summary>
public static class StringRandomizer {

    #region methods

    #region Next
    /// <summary>
    /// 生成指定长度的字符串（任何字符）。
    /// </summary>
    /// <param name="length">长度。</param>
    /// <returns>返回生成的字符串序列。</returns>
    public static string Next(int length) {
        return Next(length, true, true, true, true);
    }
    /// <summary>
    /// 生成指定长度的字符串。
    /// </summary>
    /// <param name="length">长度。</param>
    /// <param name="allowNumber">允许数字。</param>
    /// <param name="allowSmallword">允许小写字母。</param>
    /// <param name="allowBigword">允许大小字母。</param>
    /// <returns>返回生成的字符串序列。</returns>
    public static string Next(int length, bool allowNumber, bool allowSmallword, bool allowBigword) {
        return Next(length, allowNumber, false, allowSmallword, allowBigword);
    }
    /// <summary>
    /// 生成指定长度的字符串。
    /// </summary>
    /// <param name="length">长度。</param>
    /// <param name="allowNumber">允许数字。</param>
    /// <param name="allowSign">允许特殊符号。</param>
    /// <param name="allowSmallword">允许小写字母。</param>
    /// <param name="allowBigword">允许大小字母。</param>
    /// <returns>返回生成的字符串序列。</returns>
    public static string Next(int length, bool allowNumber, bool allowSign, bool allowSmallword, bool allowBigword) {

        //定义
        System.Random ranA = new System.Random();
        int intResultRound = 0;
        int intA = 0;
        string strB = "";

        while (intResultRound < length) {

            //生成随机数A，表示生成类型
            //1=数字，2=符号，3=小写字母，4=大写字母
            intA = ranA.Next(1, 5);

            //如果随机数A=1，则运行生成数字
            //生成随机数A，范围在0-10
            //把随机数A，转成字符
            //生成完，位数+1，字符串累加，结束本次循环
            if (intA == 1 && allowNumber) {
                intA = ranA.Next(0, 10);
                strB = intA.ToString() + strB;
                intResultRound = intResultRound + 1;
                continue;
            }

            //如果随机数A=2，则运行生成符号
            //生成随机数A，表示生成值域
            //1：33-47值域，2：58-64值域，3：91-96值域，4：123-126值域
            if (intA == 2 && allowSign == true) {
                intA = ranA.Next(1, 5);



                //如果A=1
                //生成随机数A，33-47的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环
                if (intA == 1) {
                    intA = ranA.Next(33, 48);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }



                //如果A=2
                //生成随机数A，58-64的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环
                if (intA == 2) {
                    intA = ranA.Next(58, 65);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }



                //如果A=3
                //生成随机数A，91-96的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环
                if (intA == 3) {
                    intA = ranA.Next(91, 97);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }



                //如果A=4
                //生成随机数A，123-126的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环
                if (intA == 4) {
                    intA = ranA.Next(123, 127);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }
            }

            //如果随机数A=3，则运行生成小写字母
            //生成随机数A，范围在97-122
            //把随机数A，转成字符
            //生成完，位数+1，字符串累加，结束本次循环
            if (intA == 3 && allowSmallword == true) {
                intA = ranA.Next(97, 123);
                strB = ((char)intA).ToString() + strB;
                intResultRound = intResultRound + 1;
                continue;
            }



            //如果随机数A=4，则运行生成大写字母
            //生成随机数A，范围在65-90
            //把随机数A，转成字符
            //生成完，位数+1，字符串累加，结束本次循环
            if (intA == 4 && allowBigword == true) {
                intA = ranA.Next(65, 89);
                strB = ((char)intA).ToString() + strB;
                intResultRound = intResultRound + 1;
                continue;
            }
        }

        return strB;
    }
    #endregion

    #endregion

}