/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 清除边框预处理器
    /// </summary>
    [Const("Name", "ClearBorder")]
    [Const("DisplayName", "清除边框")]
    public class ClearBorderPreHandler : IPreHandler {

        #region fields
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;
        private Symbol.IO.Packing.TreePackage _data = null;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置左边宽度。
        /// </summary>
        public int Left {
            get { return _left; }
            set {
                if (_left != value) {
                    _data["left"] = _left = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置上边宽度。
        /// </summary>
        public int Top {
            get { return _top; }
            set {
                if (_top != value) {
                    _data["top"] = _top = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置右边宽度。
        /// </summary>
        public int Right {
            get { return _right; }
            set {
                if (_right != value) {
                    _data["right"] = _right = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置下边宽度。
        /// </summary>
        public int Bottom {
            get { return _bottom; }
            set {
                if (_bottom != value) {
                    _data["bottom"] = _bottom = value;
                }
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建清除边框预处理器的实例。
        /// </summary>
        public ClearBorderPreHandler() {
            _data = new Symbol.IO.Packing.TreePackage();
            _data.EncryptType = Symbol.IO.Packing.PackageEncryptTypes.BinaryWave_EmptyPassword;
            _left = _top = _right = _bottom = 0;
            _data.Add("left", _left)
                 .Add("top", _top)
                 .Add("right", _right)
                 .Add("bottom", _bottom);
        }
        #endregion

        #region IPreHandler 成员
        /// <summary>
        /// 名称，唯一（英文）。
        /// </summary>
        public string Name {
            get { return "ClearBorder"; }
        }
        /// <summary>
        /// 显示名称，界面上显示的中文名称。
        /// </summary>
        public string DisplayName {
            get { return "清除边框"; }
        }

        /// <summary>
        /// 显示数据，用于预算配置数据。
        /// </summary>
        public string DisplayData {
            get { return string.Format("左:{0},上:{1},右:{2},下:{3}",_left,_top,_right,_bottom); }
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
            _left = TypeExtensions.Convert<int>(_data["left"], 0);
            _top = TypeExtensions.Convert<int>(_data["top"], 0);
            _right = TypeExtensions.Convert<int>(_data["right"], 0);
            _bottom = TypeExtensions.Convert<int>(_data["bottom"], 0);
        }

        /// <summary>
        /// 执行预处理
        /// </summary>
        /// <param name="image">需要预处理的图像。</param>
        public void Execute(System.Drawing.Bitmap image) {
            Drawing.BitmapHelper.ClearBorder(image,_left,_top,_right,_bottom);
        }



        #endregion
    }
}
