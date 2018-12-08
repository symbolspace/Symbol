/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace System.Drawing {
    /// <summary>
    /// CMYK颜色（印刷色彩模式）
    /// </summary>
    public struct CMYKColor {

        #region fields
        private static readonly System.Collections.Generic.IEqualityComparer<CMYKColor> _comparer = new CMYKColorEqualityComparer();

        private float _cyan;
        private float _magenta;
        private float _yellow;
        private float _k;

        private byte _red;
        private byte _green;
        private byte _blue;
        #endregion

        #region properties
        /// <summary>
        /// 获取比较器。
        /// </summary>
        public static System.Collections.Generic.IEqualityComparer<CMYKColor> Comparer {
            get { return _comparer; }
        }

        /// <summary>
        /// 获取或设置青色Cyan
        /// </summary>
        public float Cyan {
            get { return _cyan; }
            set {
                _cyan = value;
                ResetRGB();
            }
        }
        /// <summary>
        /// 获取或设置品红色Magenta
        /// </summary>
        public float Magenta {
            get { return _magenta; }
            set {
                _magenta = value;
                ResetRGB();
            }
        }
        /// <summary>
        /// 获取或设置黄色Yellow
        /// </summary>
        public float Yellow {
            get { return _yellow; }
            set {
                _yellow = value;
                ResetRGB();
            }
        }
        /// <summary>
        /// 获取或设置K
        /// </summary>
        public float K {
            get { return _k; }
            set {
                _k = value;
                ResetRGB();
            }
        }
        /// <summary>
        /// 获取或设置红色Red
        /// </summary>
        public byte Red {
            get { return _red; }
            set {
                _red = value;
                ResetCMYK();
            }
        }
        /// <summary>
        /// 获取或设置绿色Green
        /// </summary>
        public byte Green {
            get { return _green; }
            set {
                _green = value;
                ResetCMYK();
            }
        }
        /// <summary>
        /// 获取或设置蓝色Blue
        /// </summary>
        public byte Blue {
            get { return _blue; }
            set {
                _blue = value;
                ResetCMYK();
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建CMYKColor实例
        /// </summary>
        /// <param name="color">颜色</param>
        public CMYKColor(System.Drawing.Color color)
            : this(color.R, color.G, color.B) {
        }
        /// <summary>
        /// 创建CMYKColor实例
        /// </summary>
        /// <param name="red">红色Red</param>
        /// <param name="green">绿色Green</param>
        /// <param name="blue">蓝色Blue</param>
        public CMYKColor(byte red, byte green, byte blue) {
            _red = red;
            _green = green;
            _blue = blue;

            _cyan = _magenta = _yellow = _k = 0;
            ResetCMYK();
        }
        /// <summary>
        /// 创建CMYKColor实例
        /// </summary>
        /// <param name="cyan">青色Cyan</param>
        /// <param name="magenta">品红色Magenta</param>
        /// <param name="yellow">黄色Yellow</param>
        /// <param name="k">K</param>
        public CMYKColor(float cyan, float magenta, float yellow, float k) {
            _cyan = cyan;
            _magenta = magenta;
            _yellow = yellow;
            _k = k;

            _red = _green = _blue = 0;
            ResetRGB();
        }
        #endregion


        #region methods

        #region ResetRGB
        void ResetRGB() {
            _red = (byte)((1.0 - _cyan) * (1.0 - _k) * 255.0);
            _green = (byte)((1.0 - _magenta) * (1.0 - _k) * 255.0);
            _blue = (byte)((1.0 - _yellow) * (1.0 - _k) * 255.0);
        }
        #endregion
        #region ResetCMYK
        void ResetCMYK() {
            _cyan = (float)(255 - _red) / 255;
            _magenta = (float)(255 - _green) / 255;
            _yellow = (float)(255 - _blue) / 255;

            _k = System.Math.Min(_cyan, System.Math.Min(_magenta, _yellow));
            if (_k == 1.0) {
                _cyan = _magenta = _yellow = 0;
            } else {
                _cyan = (_cyan - _k) / (1 - _k);
                _magenta = (_magenta - _k) / (1 - _k);
                _yellow = (_yellow - _k) / (1 - _k);
            }
        }
        #endregion

        #region ToColor
        /// <summary>
        /// 将argb四个值转换为int值
        /// </summary>
        /// <returns>返回Color.ToArgb的值。</returns>
        public int ToArgb() {
            return ColorHelper.ToArgb(255, _red, _green, _blue);
        }
        /// <summary>
        /// 转换为Color
        /// </summary>
        /// <returns>返回颜色</returns>
        public System.Drawing.Color ToColor() {
            return System.Drawing.Color.FromArgb(_red, _green, _blue);
        }
        /// <summary>
        /// 转换为Color
        /// </summary>
        /// <param name="cyan">青色Cyan</param>
        /// <param name="magenta">品红色Magenta</param>
        /// <param name="yellow">黄色Yellow</param>
        /// <param name="k">K</param>
        /// <returns>返回颜色</returns>
        public static System.Drawing.Color ToColor(float cyan, float magenta, float yellow, float k) {
            return System.Drawing.Color.FromArgb(
                (int)((1.0 - cyan) * (1.0 - k) * 255.0),
                (int)((1.0 - magenta) * (1.0 - k) * 255.0),
                (int)((1.0 - yellow) * (1.0 - k) * 255.0));
        }
        #endregion

        #region FromColor
        /// <summary>
        /// 来自Color
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>返回CMYK颜色</returns>
        public static CMYKColor FromColor(System.Drawing.Color color) {
            return FromRGB(color.R, color.G, color.B);
        }
        #endregion
        #region FromRGB
        /// <summary>
        /// 来自RGB
        /// </summary>
        /// <param name="red">红色Red</param>
        /// <param name="green">绿色Green</param>
        /// <param name="blue">蓝色Blue</param>
        /// <returns>返回CMYK颜色</returns>
        public static CMYKColor FromRGB(byte red, byte green, byte blue) {
            return new CMYKColor(red, green, blue);
        }
        #endregion

        #region ToString
        /// <summary>
        /// RGB(r,g,b) CMYK(c,m,y,k)
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"RGB({Red},{Green},{Blue}) CMYK({Cyan},{Magenta},{Yellow},{K})";
        }
        #endregion

        #region implicit
        /// <summary>
        /// 从Color转换为CMYKColor
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator CMYKColor(System.Drawing.Color value) {
            return new CMYKColor(value);
        }
        /// <summary>
        /// 从Argb转换为CMYKColor
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator CMYKColor(int value) {
            var bytes = ColorHelper.ToBytes(value);
            return new CMYKColor(bytes[2], bytes[1], bytes[0]);
        }
        /// <summary>
        /// 从HsbColor转换为CMYKColor
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator CMYKColor(HsbColor value) {
            return new CMYKColor(value.Red, value.Green, value.Blue);
        }
        #endregion
        #region explicit
        /// <summary>
        /// 从CMYKColor转换为Color
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator System.Drawing.Color(CMYKColor value) {
            return value.ToColor();
        }
        /// <summary>
        /// 从CMYKColor转换为Argb
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator int(CMYKColor value) {
            return value.ToArgb();
        }
        /// <summary>
        /// 从CMYKColor转换为HsbColor
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator HsbColor(CMYKColor value) {
            return new HsbColor(value.Red, value.Blue, value.Green);
        }
        #endregion

        #endregion

        #region types
        class CMYKColorEqualityComparer : System.Collections.Generic.IEqualityComparer<CMYKColor> {
            public bool Equals(CMYKColor x, CMYKColor y) {
                return x.Red == y.Red
                    && x.Green == y.Green
                    && x.Blue == y.Blue;
            }
            public int GetHashCode(CMYKColor obj) {
                return obj.ToArgb();
            }
        }

        #endregion
    }
}
