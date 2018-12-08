/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.ComponentModel.Design.Serialization;
#pragma warning disable 1591

namespace Symbol.Drawing {
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    [System.ComponentModel.TypeConverter(typeof(PaddingConverter))]
    public struct Padding {

        #region fields
        private bool _all;
        private int _top;
        private int _left;
        private int _right;
        private int _bottom;
        public static readonly Padding Empty;

        #endregion

        #region properties

        [System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public int All {
            get {
                if (!this._all) {
                    return -1;
                }
                return this._top;
            }
            set {
                if (!this._all || (this._top != value)) {
                    this._all = true;
                    this._top = this._left = this._right = this._bottom = value;
                }
            }
        }
        [System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public int Bottom {
            get {
                if (this._all) {
                    return this._top;
                }
                return this._bottom;
            }
            set {
                if (this._all || (this._bottom != value)) {
                    this._all = false;
                    this._bottom = value;
                }
            }
        }
        [System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public int Left {
            get {
                if (this._all) {
                    return this._top;
                }
                return this._left;
            }
            set {
                if (this._all || (this._left != value)) {
                    this._all = false;
                    this._left = value;
                }
            }
        }
        [System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public int Right {
            get {
                if (this._all) {
                    return this._top;
                }
                return this._right;
            }
            set {
                if (this._all || (this._right != value)) {
                    this._all = false;
                    this._right = value;
                }
            }
        }
        [System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public int Top {
            get {
                return this._top;
            }
            set {
                if (this._all || (this._top != value)) {
                    this._all = false;
                    this._top = value;
                }
            }
        }
        [System.ComponentModel.Browsable(false)]
        public int Horizontal {
            get {
                return (this.Left + this.Right);
            }
        }
        [System.ComponentModel.Browsable(false)]
        public int Vertical {
            get {
                return (this.Top + this.Bottom);
            }
        }
        [System.ComponentModel.Browsable(false)]
        public System.Drawing.Size Size {
            get {
                return new System.Drawing.Size(this.Horizontal, this.Vertical);
            }
        }
        #endregion

        #region cctor
        static Padding() {
            Empty = new Padding(0);
        }
        #endregion
        #region ctor
        public Padding(int all) {
            this._all = true;
            this._top = this._left = this._right = this._bottom = all;
        }

        public Padding(int left, int top, int right, int bottom) {
            this._top = top;
            this._left = left;
            this._right = right;
            this._bottom = bottom;
            this._all = ((this._top == this._left) && (this._top == this._right)) && (this._top == this._bottom);
        }
        #endregion

        #region methods

        public static Padding Add(Padding p1, Padding p2) {
            return (p1 + p2);
        }

        public static Padding Subtract(Padding p1, Padding p2) {
            return (p1 - p2);
        }

        public override bool Equals(object other) {
            return ((other is Padding) && (((Padding)other) == this));
        }

        public static Padding operator +(Padding p1, Padding p2) {
            return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }

        public static Padding operator -(Padding p1, Padding p2) {
            return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
        }

        public static bool operator ==(Padding p1, Padding p2) {
            return ((((p1.Left == p2.Left) && (p1.Top == p2.Top)) && (p1.Right == p2.Right)) && (p1.Bottom == p2.Bottom));
        }

        public static bool operator !=(Padding p1, Padding p2) {
            return !(p1 == p2);
        }

        public override int GetHashCode() {
            return (((this.Left ^ WindowsFormsUtils.RotateLeft(this.Top, 8)) ^ WindowsFormsUtils.RotateLeft(this.Right, 16)) ^ WindowsFormsUtils.RotateLeft(this.Bottom, 24));
        }

        public override string ToString() {
            return ("{Left=" + this.Left.ToString(System.Globalization.CultureInfo.CurrentCulture) + ",Top=" + this.Top.ToString(System.Globalization.CultureInfo.CurrentCulture) + ",Right=" + this.Right.ToString(System.Globalization.CultureInfo.CurrentCulture) + ",Bottom=" + this.Bottom.ToString(System.Globalization.CultureInfo.CurrentCulture) + "}");
        }

        private void ResetAll() {
            this.All = 0;
        }

        private void ResetBottom() {
            this.Bottom = 0;
        }

        private void ResetLeft() {
            this.Left = 0;
        }

        private void ResetRight() {
            this.Right = 0;
        }

        private void ResetTop() {
            this.Top = 0;
        }

        internal void Scale(float dx, float dy) {
            this._top = (int)(this._top * dy);
            this._left = (int)(this._left * dx);
            this._right = (int)(this._right * dx);
            this._bottom = (int)(this._bottom * dy);
        }

        internal bool ShouldSerializeAll() {
            return this._all;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void Debug_SanityCheck() {
            if (this._all) {
            }
        }
        #endregion

        #region classes
        public sealed class WindowsFormsUtils {
            public static int RotateLeft(int value, int nBits) {
                nBits = nBits % 32;
                return ((value << nBits) | (value >> (32 - nBits)));
            }
        }
        public class PaddingConverter : System.ComponentModel.TypeConverter {
            // Methods
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) {
                return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
                return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
                string str = value as string;
                if (str == null) {
                    return base.ConvertFrom(context, culture, value);
                }
                str = str.Trim();
                if (str.Length == 0) {
                    return null;
                }
                if (culture == null) {
                    culture = System.Globalization.CultureInfo.CurrentCulture;
                }
                char ch = culture.TextInfo.ListSeparator[0];
                string[] strArray = str.Split(new char[] { ch });
                int[] numArray = new int[strArray.Length];
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(int));
                for (int i = 0; i < numArray.Length; i++) {
                    numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);
                }
                if (numArray.Length != 4) {
                    throw new System.ArgumentException(string.Format("{0}长度必须为4位，当前为{1}，参考格式：{2}。", new object[] { "value", str, "left, top, right, bottom" }));
                }
                return new Padding(numArray[0], numArray[1], numArray[2], numArray[3]);
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
                if (destinationType == null) {
                    throw new System.ArgumentNullException("destinationType");
                }
                if (value is Padding) {
                    if (destinationType == typeof(string)) {
                        Padding padding = (Padding)value;
                        if (culture == null) {
                            culture = System.Globalization.CultureInfo.CurrentCulture;
                        }
                        string separator = culture.TextInfo.ListSeparator + " ";
                        System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(int));
                        string[] strArray = new string[4];
                        int num = 0;
                        strArray[num++] = converter.ConvertToString(context, culture, padding.Left);
                        strArray[num++] = converter.ConvertToString(context, culture, padding.Top);
                        strArray[num++] = converter.ConvertToString(context, culture, padding.Right);
                        strArray[num++] = converter.ConvertToString(context, culture, padding.Bottom);
                        return string.Join(separator, strArray);
                    }
                    if (destinationType == typeof(InstanceDescriptor)) {
                        Padding padding2 = (Padding)value;
                        if (padding2.ShouldSerializeAll()) {
                            return new InstanceDescriptor(typeof(Padding).GetConstructor(new System.Type[] { typeof(int) }), new object[] { padding2.All });
                        }
                        return new InstanceDescriptor(typeof(Padding).GetConstructor(new System.Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }), new object[] { padding2.Left, padding2.Top, padding2.Right, padding2.Bottom });
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary propertyValues) {
                if (context == null) {
                    throw new System.ArgumentNullException("context");
                }
                if (propertyValues == null) {
                    throw new System.ArgumentNullException("propertyValues");
                }
                Padding padding = (Padding)context.PropertyDescriptor.GetValue(context.Instance);
                int all = (int)propertyValues["All"];
                if (padding.All != all) {
                    return new Padding(all);
                }
                return new Padding((int)propertyValues["Left"], (int)propertyValues["Top"], (int)propertyValues["Right"], (int)propertyValues["Bottom"]);
            }

            public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return true;
            }

            public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributes) {
                return System.ComponentModel.TypeDescriptor.GetProperties(typeof(Padding), attributes).Sort(new string[] { "All", "Left", "Top", "Right", "Bottom" });
            }

            public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return true;
            }
        }
        #endregion

    }
}

#pragma warning restore 1591
