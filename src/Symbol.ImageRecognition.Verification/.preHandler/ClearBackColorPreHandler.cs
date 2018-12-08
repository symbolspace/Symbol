/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 清除背景色预处理器
    /// </summary>
    [Const("Name", "ClearBackColor")]
    [Const("DisplayName", "清除背景色")]
    public class ClearBackColorPreHandler : IPreHandler {

        #region fields
        private Symbol.IO.Packing.TreePackage _data = null;
        #endregion

        #region properties

        #endregion

        #region ctor
        /// <summary>
        /// 创建清除边框预处理器的实例。
        /// </summary>
        public ClearBackColorPreHandler() {
            _data = new Symbol.IO.Packing.TreePackage();
            _data.EncryptType = Symbol.IO.Packing.PackageEncryptTypes.BinaryWave_EmptyPassword;
        }
        #endregion

        #region IPreHandler 成员
        /// <summary>
        /// 名称，唯一（英文）。
        /// </summary>
        public string Name {
            get { return "ClearBackColor"; }
        }
        /// <summary>
        /// 显示名称，界面上显示的中文名称。
        /// </summary>
        public string DisplayName {
            get { return "清除背景色"; }
        }

        /// <summary>
        /// 显示数据，用于预算配置数据。
        /// </summary>
        public string DisplayData {
            get { return ""; }
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
        }

        /// <summary>
        /// 执行预处理
        /// </summary>
        /// <param name="image">需要预处理的图像。</param>
        public void Execute(System.Drawing.Bitmap image) {
            Drawing.BitmapHelper.ClearBackColor(image, System.Drawing.Color.White);
        }



        #endregion
    }
}
