/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Collections.Generic;

namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 点阵集合
    /// </summary>
    [Symbol.IO.Packing.CustomPackage(typeof(CharPointCollection.CustomPackagePackage))]
    public class CharPointCollection : ICollection<CharPoint> {

        #region fields

        private List<CharPoint> _list = new List<CharPoint>();
        private int _xOffset = 0;
        private int _yOffset = 0;
        //private bool _isYMode = false;

        #endregion

        #region properties
        /// <summary>
        /// 获取数量
        /// </summary>
        public int Count { get { return _list.Count; } }
        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public CharPoint this[int index] { get { return _list[index]; } }
        #endregion

        #region ctor
        /// <summary>
        /// 创建 CharPointCollection 实例
        /// </summary>
        public CharPointCollection() {
            //_xOffset = xOffset;
            //_yOffset = yOffset;
            //if (xOffset != null)
            //    _isYMode = true;
        }
        #endregion

        #region methods

        #region Add
        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="point">CharPoint的实例</param>
        public void Add(CharPoint point) {
            _list.Add(point);
            //point.X = point.OriginalX - (_xOffset == null ? 0 : _xOffset.Value);
            //point.Y = point.OriginalY - (_yOffset == null ? 0 : _yOffset.Value);


            point.Left = FindPointByOriginal(point.OriginalX - 1, point.OriginalY);
            point.Top = FindPointByOriginal(point.OriginalX, point.OriginalY - 1);
            point.LeftTop = FindPointByOriginal(point.OriginalX - 1, point.OriginalY);
            point.RightTop = FindPointByOriginal(point.OriginalX - 1, point.OriginalY);

            if (point.Left != null) {
                point.Left.Right = point;
            }
            if (point.Top != null) {
                point.Top.Bottom = point;
            }
            if (point.LeftTop != null) {
                point.LeftTop.RightBottom = point;
            }
            if (point.RightTop != null) {
                point.RightTop.LeftBottom = point;
            }
        }
        #endregion

        #region FindPointByOriginal
        /// <summary>
        /// 尝试查找点
        /// </summary>
        /// <param name="originalX">原始坐标x</param>
        /// <param name="originalY">原始坐标y</param>
        /// <returns></returns>
        public CharPoint FindPointByOriginal(int originalX, int originalY) {
            return _list.Find(p => p.OriginalX == originalX && p.OriginalY == originalY);
        }
        #endregion
        #region FindPoint
        /// <summary>
        /// 尝试查找点
        /// </summary>
        /// <param name="x">私有坐标x</param>
        /// <param name="y">私有坐标y</param>
        /// <returns></returns>
        public CharPoint FindPoint(int x, int y) {
            return _list.Find(p => p.X == x && p.Y == y);
        }
        #endregion

        #region YZeroOffset
        /// <summary>
        /// Y轴趋0化，将尽可能的把原始坐标Y，往上顶点0靠近
        /// </summary>
        public void YZeroOffset() {
            if (_list.Count == 0)
                return;
            int minY = LinqHelper.Min(_list, p => p.OriginalY);
            _list.ForEach(p => p.Y = p.OriginalY - minY);
            _yOffset = minY;
        }
        #endregion
        #region XZeroOffset
        /// <summary>
        /// X轴趋0化，将尽可能的把原始坐标X，往左顶点0靠近
        /// </summary>
        public void XZeroOffset() {
            if (_list.Count == 0)
                return;
            int minX = LinqHelper.Min(_list, p => p.OriginalX);
            _list.ForEach(p => p.X = p.OriginalX - minX);
            _xOffset = minX;
        }
        #endregion

        #endregion


        #region IEnumerable<CharPoint> 成员

        IEnumerator<CharPoint> IEnumerable<CharPoint>.GetEnumerator() {
            return _list.GetEnumerator();
        }

        #endregion
        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }

        #endregion

        #region ICollection<CharPoint> 成员
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() {
            _list.Clear();
        }
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="item">点</param>
        /// <returns></returns>
        public bool Contains(CharPoint item) {
            return _list.Contains(item);
        }

        void ICollection<CharPoint>.CopyTo(CharPoint[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }

        bool ICollection<CharPoint>.IsReadOnly {
            get { return false; }
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="item">点</param>
        /// <returns></returns>
        public bool Remove(CharPoint item) {
            return _list.Remove(item);
        }

        #endregion

        #region classes
        class CustomPackagePackage : Symbol.IO.Packing.ICustomPackage {
            #region ICustomPackage 成员

            public object Load(byte[] buffer) {
                Symbol.IO.Packing.TreePackage data = Symbol.IO.Packing.TreePackage.Load(buffer);
                if (data == null)
                    return null;
                CharPointCollection collection = new CharPointCollection();
                List<CharPoint> list = data["0"] as List<CharPoint>;
                if (list != null)
                    collection._list.AddRange(list);

                return collection;
            }

            public byte[] Save(object instance) {
                CharPointCollection collection = instance as CharPointCollection;
                if (collection == null)
                    return new byte[0];
                Symbol.IO.Packing.TreePackage data = new Symbol.IO.Packing.TreePackage();
                data.Add("0", collection._list);
                return data.Save();
            }

            #endregion
        }

        #endregion
    }

}
