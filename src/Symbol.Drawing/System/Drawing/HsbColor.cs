/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace System.Drawing {
    /// <summary>
    /// Hsb颜色
    /// </summary>
    public struct HsbColor {

        #region fields
        private static readonly System.Collections.Generic.IEqualityComparer<HsbColor> _comparer = new HsbColorEqualityComparer();

        private float _hue;
        private float _saturation;
        private float _brightness;

        private byte _red;
        private byte _green;
        private byte _blue;


        #endregion

        #region properties
        /// <summary>
        /// 获取比较器。
        /// </summary>
        public static System.Collections.Generic.IEqualityComparer<HsbColor> Comparer {
            get { return _comparer; }
        }

        /// <summary>
        /// 获取或设置色相Cyan
        /// </summary>
        public float Hue {
            get { return _hue; }
            set {
                _hue = value;
                ResetRGB();
            }
        }
        /// <summary>
        /// 获取或设置品饱和度Saturation
        /// </summary>
        public float Saturation {
            get { return _saturation; }
            set {
                _saturation = value;
                ResetRGB();
            }
        }
        /// <summary>
        /// 获取或设置亮度Yellow
        /// </summary>
        public float Brightness {
            get { return _brightness; }
            set {
                _brightness = value;
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
                ResetHsb();
            }
        }
        /// <summary>
        /// 获取或设置绿色Green
        /// </summary>
        public byte Green {
            get { return _green; }
            set {
                _green = value;
                ResetHsb();
            }
        }
        /// <summary>
        /// 获取或设置蓝色Blue
        /// </summary>
        public byte Blue {
            get { return _blue; }
            set {
                _blue = value;
                ResetHsb();
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建HsbColor实例
        /// </summary>
        /// <param name="color">颜色</param>
        public HsbColor(System.Drawing.Color color)
            : this(color.R, color.G, color.B) {
        }
        /// <summary>
        /// 创建HsbColor实例
        /// </summary>
        /// <param name="red">红色Red</param>
        /// <param name="green">绿色Green</param>
        /// <param name="blue">蓝色Blue</param>
        public HsbColor(byte red, byte green, byte blue) {
            _red = red;
            _green = green;
            _blue = blue;

            _hue = _saturation = _brightness = 0;
            ResetHsb();
        }
        /// <summary>
        /// 创建HsbColor实例
        /// </summary>
        /// <param name="hue">色相Hue</param>
        /// <param name="saturation">饱和度Saturation</param>
        /// <param name="yellow">亮度Brightness</param>
        public HsbColor(float hue, float saturation, float yellow) {
            _hue = hue;
            _saturation = saturation;
            _brightness = yellow;

            _red = _green = _blue = 0;
            ResetRGB();
        }
        #endregion


        #region methods

        #region ResetRGB
        void ResetRGB() {
            float r = 0F;
            float g = 0F;
            float b = 0F;
            if (_saturation == 0F) {
                r = g = b = _brightness;
                goto lb_End;
            }
            // the color wheel consists of 6 sectors. Figure out which sector you're in.
            float sectorPos = _hue / 60.0F;
            int sectorNumber = (int)(System.Math.Floor(sectorPos));
            // get the fractional part of the sector
            float fractionalSector = sectorPos - sectorNumber;

            // calculate values for the three axes of the color. 
            float p = (float)(_brightness * (1.0 - _saturation));
            float q = (float)(_brightness * (1.0 - (_saturation * fractionalSector)));
            float t = (float)(_brightness * (1.0 - (_saturation * (1 - fractionalSector))));

            // assign the fractional colors to r, g, and b based on the sector the angle is in.
            switch (sectorNumber) {
                case 0:
                    r = _brightness;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = _brightness;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = _brightness;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = _brightness;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = _brightness;
                    break;
                case 5:
                    r = _brightness;
                    g = p;
                    b = q;
                    break;
            }

        lb_End:
            _red = (byte)(r * 255);
            _green = (byte)(g * 255);
            _blue = (byte)(b * 255);
        }
        #endregion
        #region ResetHsb
        void ResetHsb() {

            float r = _red / 255F;
            float g = _green / 255F;
            float b = _blue / 255F;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            _hue = 0.0F;
            if (max == r && g >= b) {
                if (max - min == 0)
                    _hue = 0.0F;
                else
                    _hue = 60 * (g - b) / (max - min);
            } else if (max == r && g < b) {
                _hue = 60 * (g - b) / (max - min) + 360;
            } else if (max == g) {
                _hue = 60 * (b - r) / (max - min) + 120;
            } else if (max == b) {
                _hue = 60 * (r - g) / (max - min) + 240;
            }

            _saturation = (max == 0) ? 0.0F : (1.0F - ((float)min / (float)max));
            _brightness = max;
            
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
        /// <param name="hue">色相Hue</param>
        /// <param name="saturation">饱和度Saturation</param>
        /// <param name="yellow">亮度Brightness</param>
        /// <returns>返回颜色</returns>
        public static System.Drawing.Color ToColor(float hue, float saturation, float yellow) {
            return new HsbColor(hue, saturation, yellow).ToColor();
        }
        #endregion

        #region FromColor
        /// <summary>
        /// 来自Color
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>返回Hsb颜色</returns>
        public static HsbColor FromColor(System.Drawing.Color color) {
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
        /// <returns>返回Hsb颜色</returns>
        public static HsbColor FromRGB(byte red, byte green, byte blue) {
            return new HsbColor(red, green, blue);
        }
        #endregion

        #region ToString
        /// <summary>
        /// RGB(r,g,b) HSB(hsb)
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"RGB({Red},{Green},{Blue}) HSB({Hue},{Saturation},{Brightness})";
        }
        #endregion

        #region implicit
        /// <summary>
        /// 从Color转换为HsbColor
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator HsbColor(System.Drawing.Color value) {
            return new HsbColor(value);
        }
        /// <summary>
        /// 从Argb转换为HsbColor
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator HsbColor(int value) {
            var bytes = ColorHelper.ToBytes(value);
            return new HsbColor(bytes[2], bytes[1], bytes[0]);
        }
        /// <summary>
        /// 从CMYKColor转换为HsbColor
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator HsbColor(CMYKColor value) {
            return new HsbColor(value.Red, value.Green, value.Blue);
        }

        #endregion
        #region explicit
        /// <summary>
        /// 从HsbColor转换为Color
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator System.Drawing.Color(HsbColor value) {
            return value.ToColor();
        }
        /// <summary>
        /// 从HsbColor转换为Argb
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator int(HsbColor value) {
            return value.ToArgb();
        }
        /// <summary>
        /// 从HsbColor转换为CMYKColor
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator CMYKColor(HsbColor value) {
            return new CMYKColor(value.Red, value.Blue, value.Green);
        }

        #endregion

        #endregion

        #region types
        class HsbColorEqualityComparer : System.Collections.Generic.IEqualityComparer<HsbColor> {
            public bool Equals(HsbColor x, HsbColor y) {
                return x.Red == y.Red
                    && x.Green == y.Green
                    && x.Blue == y.Blue;
            }
            public int GetHashCode(HsbColor obj) {
                return obj.ToArgb();
            }
        }

        #endregion
    }
}
