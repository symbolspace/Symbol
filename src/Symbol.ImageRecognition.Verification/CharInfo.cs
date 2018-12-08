/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 字符信息
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Value}={Points.Count}")]
    public class CharInfo {

        #region properties
        /// <summary>
        /// 字符
        /// </summary>
        public char? Value { get; set; }
        /// <summary>
        /// 点阵
        /// </summary>
        public CharPointCollection Points { get; private set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建 CharInfo 实例。
        /// </summary>
        public CharInfo() {
            Points = new CharPointCollection();
        }
        #endregion

        #region methods

        #region TiltCorrect
        /// <summary>
        /// 矩阵旋转
        /// </summary>
        public void TiltCorrect() {
            if (Points.Count == 0)
                return;
            int minX = LinqHelper.Min(Points, p1 => p1.X);
            int minY = LinqHelper.Min(Points, p1 => p1.Y);
            CharPoint leftPoint = LinqHelper.FirstOrDefault(LinqHelper.OrderBy(LinqHelper.Where(Points,p1=>p1.X==minX), p1 => p1.Y));
            CharPoint topPoint = LinqHelper.FirstOrDefault(LinqHelper.ThenByDescending(LinqHelper.OrderBy(Points, p1 => p1.Y), p1 => p1.X));
            if (leftPoint.Y == topPoint.Y || leftPoint.X == topPoint.X)
                return;
            System.Drawing.Point centerPoint=new System.Drawing.Point(leftPoint.X, leftPoint.Y);
            double l = System.Drawing.PointHelper.Distance(leftPoint.X, leftPoint.Y, topPoint.X, topPoint.Y);
            double angle;
            if (leftPoint.Y < topPoint.Y) {
                //顺时针旋转
                int x = leftPoint.X + (int)l;
                int y = leftPoint.Y;
                angle =-System.Drawing.PointHelper.ClipAngle(
                    centerPoint,
                    new System.Drawing.Point(topPoint.X, topPoint.Y),
                    new System.Drawing.Point(x, y));
                
            } else {
                //逆时针旋转
                //int x = leftPoint.X + (int)l;
                //int y = leftPoint.Y;
                //angle = Symbol.Drawing.BitmapHelper.Angle_3Point(
                //    centerPoint,
                //    new System.Drawing.Point(x, y),
                //    new System.Drawing.Point(topPoint.X, topPoint.Y));
                return;
            }
            foreach (CharPoint item in Points) {
                if (item == leftPoint)
                    continue;
                System.Drawing.Point pointX = System.Drawing.PointHelper.Rotate2(centerPoint, new System.Drawing.Point(item.X, item.Y), angle);
                item.X = pointX.X;
                item.Y = pointX.Y;
            }
            CharPoint leftPoint2 = LinqHelper.FirstOrDefault(LinqHelper.ThenBy(LinqHelper.OrderBy(Points, p1 => p1.X), p1 => p1.Y));
            CharPoint topPoint2 = LinqHelper.FirstOrDefault(LinqHelper.ThenByDescending(LinqHelper.OrderBy(Points, p1 => p1.Y), p1 => p1.X));
            int offsetX = 0;
            if (leftPoint2.X < 0) {
                offsetX = System.Math.Abs(leftPoint2.X);
            }
            int offsetY = 0;
            if (topPoint2.Y < 0) {
                offsetY = System.Math.Abs(topPoint2.Y);
            }
            if (offsetX == 0 && offsetY == 0)
                return;
            foreach (CharPoint item in Points) {
                item.X += offsetX;
                item.Y += offsetY;
            }
        }
        #endregion

        #region CenterMiddle
        /// <summary>
        /// 点阵居中
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void CenterMiddle(int width, int height) {
            if (Points.Count == 0)
                return;
            int maxWidth = LinqHelper.Max(Points, p => p.X) + 1;
            int maxHeight = LinqHelper.Max(Points, p => p.Y) + 1;
            int offsetX = 0;
            if (width > maxWidth) {
                offsetX = (width - maxWidth);
                if (offsetX % 2 != 0)
                    offsetX--;
                offsetX /= 2;
            }
            int offsetY = 0;
            if (height > maxHeight) {
                offsetY = (height - maxHeight);
                if (offsetY % 2 != 0)
                    offsetY--;
                offsetY /= 2;
            }

            if (offsetX == 0 && offsetY == 0)
                return;

            foreach (CharPoint item in Points) {
                item.X += offsetX;
                item.Y += offsetY;
            }
        }
        #endregion

        #endregion
    }

}
