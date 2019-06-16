/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite.Functions {
#pragma warning disable CS1591

    /// <summary>
    /// 产生一个随机数，第一个参数：最小值，第二个参数：最大值（始终会小于此值。），第三个参数：是否输出整数。
    /// </summary>
    [SQLiteFunction(Name = "randomNext", Arguments = 3, Type = FunctionTypes.Scalar)]
    public class randomNext : SQLiteFunction {
        private static readonly Random _random = new Random();

        public override object Invoke(object[] args) {
            double d1 = TypeExtensions.Convert<double>(args[0], 0D);
            double d2 = TypeExtensions.Convert<double>(args[1], 0D);
            if (d2 <= d1)
                d2 = d1 + 1D;
            if (TypeExtensions.Convert<bool>(args[2], false))
                return (long)(Math.Floor(_random.NextDouble() * (d2 - d1) + d1));
            else
                return _random.NextDouble() * (d2 - d1) + d1;
        }
    }

#pragma warning restore CS1591
}