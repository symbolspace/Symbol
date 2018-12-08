/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 亮度预处理器
    /// </summary>
    [Const("Name", "Light")]
    [Const("DisplayName", "亮度")]
    public class LightPreHandler : IPreHandler {

        #region fields
        private int _value;
        private Symbol.IO.Packing.TreePackage _data = null;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置亮度。
        /// </summary>
        public int Value {
            get { return _value; }
            set {
                if (_value != value) {
                    _data["value"] = _value = value;
                }
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建亮度预处理器的实例。
        /// </summary>
        public LightPreHandler() {
            _data = new Symbol.IO.Packing.TreePackage();
            _data.EncryptType = Symbol.IO.Packing.PackageEncryptTypes.BinaryWave_EmptyPassword;
            _value = 0;
            _data.Add("value", _value);
        }
        #endregion

        #region IPreHandler 成员
        /// <summary>
        /// 名称，唯一（英文）。
        /// </summary>
        public string Name {
            get { return "Light"; }
        }
        /// <summary>
        /// 显示名称，界面上显示的中文名称。
        /// </summary>
        public string DisplayName {
            get { return "亮度"; }
        }

        /// <summary>
        /// 显示数据，用于预算配置数据。
        /// </summary>
        public string DisplayData {
            get { return "阀值：" + _value; }
        }
        /// <summary>
        /// 保存数据。
        /// </summary>
        /// <returns>保存配置到数据包。</returns>
        public Symbol.IO.Packing.TreePackage Save() {
            return _data;
        }
        /// <summary>
        /// 加载数据。
        /// </summary>
        /// <param name="data">从数据包中加载。</param>
        public void Load(Symbol.IO.Packing.TreePackage data) {
            _data = data;
            _value = TypeExtensions.Convert<int>(_data["value"], 0);
        }

        /// <summary>
        /// 执行预处理
        /// </summary>
        /// <param name="image">需要预处理的图像。</param>
        public void Execute(System.Drawing.Bitmap image) {
            Drawing.BitmapHelper.Light(image, _value);
        }

        #endregion
    }
}
