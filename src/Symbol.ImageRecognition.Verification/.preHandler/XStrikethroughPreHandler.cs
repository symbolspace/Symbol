/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 清除背景色预处理器
    /// </summary>
    [Const("Name", "XStrikethrough")]
    [Const("DisplayName", "横向删除线")]
    public class XStrikethroughPreHandler : IPreHandler {

        #region fields
        private Symbol.IO.Packing.TreePackage _data = null;

        private float _span;
        private int _continuousDegree;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置跨度（百分比小数）。
        /// </summary>
        public float Span {
            get { return _span; }
            set {
                if (_span != value) {
                    _data["span"] = _span = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置连续度（像素）。
        /// </summary>
        public int ContinuousDegree {
            get { return _continuousDegree; }
            set {
                if (_continuousDegree != value) {
                    _data["continuousDegree"] = _continuousDegree = value;
                }
            }
        }

        #endregion

        #region ctor
        /// <summary>
        /// 创建清除边框预处理器的实例。
        /// </summary>
        public XStrikethroughPreHandler() {
            _data = new Symbol.IO.Packing.TreePackage();
            _data.EncryptType = Symbol.IO.Packing.PackageEncryptTypes.BinaryWave_EmptyPassword;

            _span = 0.1F;
            _continuousDegree = 3;
            _data.Add("span", _span)
                 .Add("continuousDegree", _continuousDegree);
        }
        #endregion

        #region IPreHandler 成员
        /// <summary>
        /// 名称，唯一（英文）。
        /// </summary>
        public string Name {
            get { return "XStrikethrough"; }
        }
        /// <summary>
        /// 显示名称，界面上显示的中文名称。
        /// </summary>
        public string DisplayName {
            get { return "横向删除线"; }
        }

        /// <summary>
        /// 显示数据，用于预算配置数据。
        /// </summary>
        public string DisplayData {
            get { return string.Format("跨度:{0}%,连续性:{1}px",_span*100,_continuousDegree); }
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
            _span = TypeExtensions.Convert<float>(_data["span"], 0F);
            _continuousDegree = TypeExtensions.Convert<int>(_data["continuousDegree"], 0);

        }

        /// <summary>
        /// 执行预处理
        /// </summary>
        /// <param name="image">需要预处理的图像。</param>
        public void Execute(System.Drawing.Bitmap image) {
            Drawing.BitmapHelper.XStrikethrough(image,_span,_continuousDegree,System.Drawing.Color.White);
        }



        #endregion
    }
}
/*
 
 X跨度> n%  Span
连续度:0-10 px  Continuous degree
忽略颜色：背景色
continuousDegree
L=(x1-x2)^2+(y1-y2)^2
 */