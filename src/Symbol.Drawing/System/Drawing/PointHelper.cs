/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace System.Drawing {
    /// <summary>
    /// Point辅助类。
    /// </summary>
    public class PointHelper {


        #region methods

        #region Distance
        /// <summary>
        /// 求两点之间的距离。
        /// </summary>
        /// <param name="x">第一个点 x</param>
        /// <param name="y">第一个点 y</param>
        /// <param name="x2">第二个点 x</param>
        /// <param name="y2">第二个点 y</param>
        /// <returns>返回两点之间的距离。</returns>
        public static double Distance(int x, int y, int x2, int y2) {
            return Math.Sqrt(
                        Math.Abs(x - x2) * Math.Abs(x - x2)
                      + Math.Abs(y - y2) * Math.Abs(y - y2)
                    );
        }
        /// <summary>
        /// 求两点之间的距离。
        /// </summary>
        /// <param name="p">第一个点</param>
        /// <param name="p2">第二个点</param>
        /// <returns>返回两点之间的距离。</returns>
        public static double Distance(Point p, Point p2) {
            return Math.Sqrt(
                        Math.Abs(p.X - p2.X) * Math.Abs(p.X - p2.X)
                      + Math.Abs(p.Y - p2.Y) * Math.Abs(p.Y - p2.Y)
                    );
        }
        #endregion

        #region ClipAngle
        /// <summary>
        /// 求两点与中心点之间的夹角度数。
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="first">第一个点</param>
        /// <param name="second">第二个点</param>
        /// <returns>返回夹角度数</returns>
        public static double ClipAngle(Point center, Point first, Point second) {
            const double PI = 3.1415926535897;

            double ma_x = first.X - center.X;
            double ma_y = first.Y - center.Y;
            double mb_x = second.X - center.X;
            double mb_y = second.Y - center.Y;
            double v1 = (ma_x * mb_x) + (ma_y * mb_y);
            double ma_val = Math.Sqrt(ma_x * ma_x + ma_y * ma_y);
            double mb_val = Math.Sqrt(mb_x * mb_x + mb_y * mb_y);
            double cosM = v1 / (ma_val * mb_val);
            double angleAMB = Math.Acos(cosM) * 180 / PI;

            return angleAMB;
        }
        #endregion

        #region Rotate
        /// <summary>  
        /// 对一个坐标点按照一个中心进行旋转  
        /// </summary>  
        /// <param name="center">中心点</param>  
        /// <param name="p1">要旋转的点</param>  
        /// <param name="angle">旋转角度，笛卡尔直角坐标</param>  
        /// <returns></returns>  
        public static Point Rotate(Point center, Point p1, double angle) {
            Point tmp = new Point();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (int)x1;
            tmp.Y = (int)y1;
            return tmp;
        }
        /// <summary>  
        /// 以中心点旋转Angle角度  
        /// </summary>  
        /// <param name="center">中心点</param>  
        /// <param name="p1">待旋转的点</param>  
        /// <param name="angle">旋转角度（弧度）</param>  
        public static Point Rotate2(Point center, Point p1, double angle) {
            Point tmp = new Point();
            double x1 = (p1.X - center.X) * Math.Cos(angle) + (p1.Y - center.Y) * Math.Sin(angle) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angle) + (p1.Y - center.Y) * Math.Cos(angle) + center.Y;
            tmp.X = (int)x1;
            tmp.Y = (int)y1;
            return tmp;
        }
        #endregion

        #endregion
    }
}