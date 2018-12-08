/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 字库
    /// </summary>
    public class CharLibrary {

        #region fields
        private string _name;
        private string _version;
        private string _description;
        private int _width;
        private int _height;
        private int _charWidth;
        private int _charHeight;
        private int _charMaxWidth;
        private int _minPoints;
        private bool _needCenterMiddle;
        private byte _emptyColorR;
        private float _zool;
        private string _pageUrl;
        private string _codeUrl;
        private string _extension;

        private System.Collections.Generic.List<IPreHandler> _preHandlers = null;
        private ITakeHandler _takeHandler = null;
        private System.Collections.Generic.List<CharInfo> _chars = null;
        private ICharRecognizer _charRecognizer = null;

        private Symbol.IO.Packing.TreePackage _data = null;
        #endregion

        #region properties
        /// <summary>
        /// 获取或设置字库的名称。
        /// </summary>
        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _data["name"] = _name = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置字库的版本。
        /// </summary>
        public string Version {
            get { return _version; }
            set {
                if (_version != value) {
                    _data["version"] = _version = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置字库的描述。
        /// </summary>
        public string Description {
            get { return _description; }
            set {
                if (_description != value) {
                    _data["description"] = _description = value;
                }
            }

        }
        /// <summary>
        /// 获取或设置原始图像的宽度。
        /// </summary>
        public int Width {
            get { return _width; }
            set {
                if (_width != value) {
                    _data["width"] = _width = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置原始图像的高度。
        /// </summary>
        public int Height {
            get { return _height; }
            set {
                if (_height != value) {
                    _data["height"] = _height = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置字符的宽度。
        /// </summary>
        public int CharWidth {
            get { return _charWidth; }
            set {
                if (_charWidth != value) {
                    _data["charWidth"] = _charWidth = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置字符的高度。
        /// </summary>
        public int CharHeight {
            get { return _charHeight; }
            set {
                if (_charHeight != value) {
                    _data["charHeight"] = _charHeight = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置字符的最大宽度。
        /// </summary>
        public int CharMaxWidth {
            get { return _charMaxWidth; }
            set {
                if (_charMaxWidth != value) {
                    _data["charMaxWidth"] = _charMaxWidth = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置字符的最少像素点。
        /// </summary>
        public int MinPoints {
            get { return _minPoints; }
            set {
                if (_minPoints != value) {
                    _data["minPoints"] = _minPoints = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置字符的最少像素点。
        /// </summary>
        public bool NeedCenterMiddle {
            get { return _needCenterMiddle; }
            set {
                if (_needCenterMiddle != value) {
                    _data["needCenterMiddle"] = _needCenterMiddle = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置空白颜色R的值。
        /// </summary>
        public byte EmptyColorR {
            get { return _emptyColorR; }
            set {
                if (_emptyColorR != value) {
                    _data["emptyColorR"] = _emptyColorR = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置缩放比例。
        /// </summary>
        public float Zool {
            get { return _zool; }
            set {
                if (_zool != value) {
                    _data["zool"] = _zool = (value <= 0F ? 1.0F : value);
                }
            }
        }
        /// <summary>
        /// 获取或设置页面网址。
        /// </summary>
        public string PageUrl {
            get { return _pageUrl; }
            set {
                if (_pageUrl != value) {
                    _data["pageUrl"] = _pageUrl = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置验证码网址。
        /// </summary>
        public string CodeUrl {
            get { return _codeUrl; }
            set {
                if (_codeUrl != value) {
                    _data["codeUrl"] = _codeUrl = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置验证码扩展名。
        /// </summary>
        public string Extension {
            get { return _extension; }
            set {
                if (_extension != value) {
                    _data["extension"] = _extension = value;
                }
            }
        }
        /// <summary>
        /// 获取预处理器列表。
        /// </summary>
        public System.Collections.Generic.IEnumerable<IPreHandler> PreHandlers {
            get { return _preHandlers; }
        }
        /// <summary>
        /// 获取或设置字符识别器，不能设置为null。
        /// </summary>
        public ICharRecognizer CharRecognizer { 
            get { return _charRecognizer; }
            set {
                CommonException.CheckArgumentNull(value, "CharRecognizer");
                _charRecognizer = value;
            }
        }
        /// <summary>
        /// 获取或设置取字处理器，不能设置为null。
        /// </summary>
        public ITakeHandler TakeHandler {
            get { return _takeHandler; }
            set {
                CommonException.CheckArgumentNull(value, "TakeHandler");
                _takeHandler = value;
            }
        }
        /// <summary>
        /// 获取字符列表。
        /// </summary>
        public System.Collections.Generic.IEnumerable<CharInfo> Chars {
            get { return _chars; }
        }
        #endregion

        #region ctor
        /// <summary>
        /// 创建字库实例，会创建一个字库模板。
        /// </summary>
        public CharLibrary() {
            _name = "新建字库";
            _version = DateTime.Today.ToString("yyyyMMdd");
            _description = "";
            _width = 100;
            _height = 20;
            _charWidth = 12;
            _charHeight = 18;
            _charMaxWidth = 14;
            _minPoints = 10;
            _needCenterMiddle = true;
            _emptyColorR = 255;
            _zool = 1.0F;
            _pageUrl = "";
            _codeUrl = "";
            _extension = ".jpg";
            _preHandlers = new System.Collections.Generic.List<IPreHandler>();
            _takeHandler = TakeHandlerHelper.CreateInstance("YDotMatrix");

            _chars = new System.Collections.Generic.List<CharInfo>();
            _charRecognizer = CharRecognizerHelper.CreateInstance("Weight3x3");
            _data = new Symbol.IO.Packing.TreePackage();
            _data.Add("name", _name)
                 .Add("version", _version)
                 .Add("description", _description)
                 .Add("width", _width)
                 .Add("height", _height)
                 .Add("charWidth", _charWidth)
                 .Add("charHeight", _charHeight)
                 .Add("charMaxWidth", _charMaxWidth)
                 .Add("minPoints", _minPoints)
                 .Add("needCenterMiddle", _needCenterMiddle)
                 .Add("emptyColorR", _emptyColorR)
                 .Add("zool",_zool)
                 .Add("pageUrl", _pageUrl)
                 .Add("codeUrl", _codeUrl)
                 .Add("extension", _extension)
                 .Add("preHandlers", 0)
                 .Add("chars",0);
        }
        #endregion

        #region methods

        #region AddPreHandler
        /// <summary>
        /// 添加预处理器
        /// </summary>
        /// <param name="handler">预处理器实例。</param>
        /// <returns>返回false表示已经存在。</returns>
        public bool AddPreHandler(IPreHandler handler) {
            if (_preHandlers.Contains(handler))
                return false;
            _preHandlers.Add(handler);
            return true;
        }
        #endregion
        #region RemovePreHandler
        /// <summary>
        /// 移除预处理器
        /// </summary>
        /// <param name="handler">预处理器实例。</param>
        /// <returns>返回false表示不存在。</returns>
        public bool RemovePreHandler(IPreHandler handler) {
            return _preHandlers.Remove(handler);
        }
        #endregion
        #region PreHandlerMovePrev
        /// <summary>
        /// 预处理器上移
        /// </summary>
        /// <param name="handler">处理器</param>
        public void PreHandlerMovePrev(IPreHandler handler) {
            int index = _preHandlers.IndexOf(handler);
            if (index == -1 || index==0)
                return;
            index--;
            _preHandlers.Remove(handler);
            _preHandlers.Insert(index, handler);
        }
        #endregion
        #region PreHandlerMoveNext
        /// <summary>
        /// 预处理器下移
        /// </summary>
        /// <param name="handler">处理器</param>
        public void PreHandlerMoveNext(IPreHandler handler) {
            int index = _preHandlers.IndexOf(handler);
            if (index == -1 || index == _preHandlers.Count-1)
                return;
            index++;
            _preHandlers.Remove(handler);
            _preHandlers.Insert(index, handler);
        }
        #endregion

        #region comment codes
        //#region AddTakeHandler
        ///// <summary>
        ///// 添加取字处理器
        ///// </summary>
        ///// <param name="handler">取字处理器实例。</param>
        ///// <returns>返回false表示已经存在。</returns>
        //public bool AddTakeHandler(ITakeHandler handler) {
        //    if (handler == null)
        //        return false;
        //    if (_takeHandlers.Contains(handler))
        //        return false;
        //    _takeHandlers.Add(handler);
        //    return true;
        //}
        //#endregion
        //#region RemoveTakeHandler
        ///// <summary>
        ///// 移除取字处理器
        ///// </summary>
        ///// <param name="handler">取字处理器实例。</param>
        ///// <returns>返回false表示不存在。</returns>
        //public bool RemoveTakeHandler(ITakeHandler handler) {
        //    if (handler == null)
        //        return false;
        //    return _takeHandlers.Remove(handler);
        //}
        //#endregion
        //#region TakeHandlerMovePrev
        //public void TakeHandlerMovePrev(ITakeHandler handler) {
        //    int index = _takeHandlers.IndexOf(handler);
        //    if (index == -1 || index==0)
        //        return;
        //    index--;
        //    _takeHandlers.Remove(handler);
        //    _takeHandlers.Insert(index, handler);
        //}
        //#endregion
        //#region TakeHandlerMoveNext
        //public void TakeHandlerMoveNext(ITakeHandler handler) {
        //    int index = _takeHandlers.IndexOf(handler);
        //    if (index == -1 || index == _preHandlers.Count-1)
        //        return;
        //    index++;
        //    _takeHandlers.Remove(handler);
        //    _takeHandlers.Insert(index, handler);
        //}
        //#endregion
        #endregion

        #region ClearChars
        /// <summary>
        /// 清空字库数据。
        /// </summary>
        public void ClearChars() {
            _chars.Clear();
        }
        #endregion
        #region AddChar
        /// <summary>
        /// 添加新字符（由于会做同字符数据比较，所以会有一些慢。）
        /// </summary>
        /// <param name="charInfo">字符数据</param>
        /// <returns>返回false表示已经存在或charInfo参数为null。</returns>
        public bool AddChar(CharInfo charInfo) {
            if (charInfo == null)
                return false;
            CharInfo original = _chars.Find(p => {
                if (p == charInfo)//同一个
                    return true;
                //不同字符，或不同点数，就肯定不一样。
                if (p.Value != charInfo.Value || p.Points.Count != charInfo.Points.Count)
                    return false;
                for (int i = 0; i < p.Points.Count; i++) {
                    CharPoint p2 = p.Points[i];
                    CharPoint p3 = charInfo.Points[i];
                    //坐标不同，颜色不同都可以认为是不同
                    if (p2.X != p3.X || p2.Y != p3.Y || p2.R != p3.R)
                        return false;
                }
                //既然数据一样，就是同一个
                return true;
            });
            if (original == null) {
                _chars.Add(charInfo);
                return true;
            }
            return false;
        }
        #endregion

        #region Save
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="password">密码，为空将会是默认密码。</param>
        /// <returns>返回保存后的数据。</returns>
        public byte[] Save(byte[] password = null) {
            SaveBefore();
            return _data.Save(password);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="writer">树型包写入器实例</param>
        /// <param name="password">密码，为空将会是默认密码。</param>
        public void Save(System.IO.Stream writer, byte[] password = null) {
            SaveBefore();
            _data.Save(writer, password);
        }
        #region SaveBefore
        void SaveBefore() {
            _data["preHandlers"] = _preHandlers.Count;
            string[] oriKeys = LinqHelper.ToArray(LinqHelper.Where(_data.Keys, p => p.StartsWith("pre_")));
            foreach (string key in oriKeys) { _data.Remove(key, false); }
            for (int i = 0; i < _preHandlers.Count; i++) {
                _data.Add("pre_" + i + "_" + _preHandlers[i].Name, _preHandlers[i].Save());
            }

            //_data["takeHandlers"] = _takeHandlers.Count;
            //string[] oriKeys2 = LinqHelper.ToArray(LinqHelper.Where(_data.Keys, p => p.StartsWith("take_")));
            //foreach (string key in oriKeys2) { _data.Remove(key,false); }
            //for (int i = 0; i < _takeHandlers.Count; i++) {
            //    _data.Add("take_" + i + "_" + _takeHandlers[i].Name, _takeHandlers[i].Save());
            //}

            string[] oriKeys3 = LinqHelper.ToArray(LinqHelper.Where(_data.Keys, p => p.StartsWith("char_")));
            foreach (string key in oriKeys3) { _data.Remove(key, false); }
            _data["chars"] = _chars.Count;
            for (int i = 0; i < _chars.Count; i++) {
                if (_chars[i].Value == null)
                    continue;
                Symbol.IO.Packing.TreePackage charData = new Symbol.IO.Packing.TreePackage() { EncryptType = Symbol.IO.Packing.PackageEncryptTypes.BinaryWave_EmptyPassword };
                CharPointCollection collection = _chars[i].Points;
                charData.Add("count",collection.Count);
                for(int j=0;j<collection.Count;j++){
                    string keyBefore = j + "_";
                    charData.Add(keyBefore + "OriginalX", collection[j].OriginalX);
                    charData.Add(keyBefore + "OriginalY", collection[j].OriginalY);
                    charData.Add(keyBefore + "X", collection[j].X);
                    charData.Add(keyBefore + "Y", collection[j].Y);
                    charData.Add(keyBefore + "R", collection[j].R);
                    charData.Add(keyBefore + "G", collection[j].G);
                    charData.Add(keyBefore + "B", collection[j].B);
                }
                _data.Add("char_" + i + "_" + _chars[i].Value, charData);
            }
            string takeHandlerKeyBefore = "takeHandler_";
            string takeHandlerKeyOri = LinqHelper.FirstOrDefault(_data.Keys, p => p.StartsWith(takeHandlerKeyBefore));
            if (!string.IsNullOrEmpty(takeHandlerKeyOri)) {
                _data.Remove(takeHandlerKeyOri, false);
            }
            string takeHandlerKey = takeHandlerKeyBefore + _takeHandler.Name;
            _data.Add(takeHandlerKey, _takeHandler.Save());

            string charRecognizerKeyBefore = "charRecognizer_";
            string charRecognizerKeyOri = LinqHelper.FirstOrDefault(_data.Keys, p => p.StartsWith(charRecognizerKeyBefore));
            if (!string.IsNullOrEmpty(charRecognizerKeyOri)) {
                _data.Remove(charRecognizerKeyOri, false);
            }
            string charRecognizerKey = charRecognizerKeyBefore + _charRecognizer.Name;
            _data.Add(charRecognizerKey, _charRecognizer.Save());
        }
        #endregion

        #endregion
        #region Load
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="password">密码，为空将会是默认密码。</param>
        public static CharLibrary Load(byte[] buffer, byte[] password = null) {
            Symbol.IO.Packing.TreePackage data = Symbol.IO.Packing.TreePackage.Load(buffer, password);
            if (data == null || data.Count < 5 || !data.ContainsKey("name"))
                return null;
            CharLibrary result = new CharLibrary();
            LoadValues(data, result);
            return result;
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="reader">树型包读取器实例</param>
        /// <param name="password">密码，为空将会是默认密码。</param>
        /// <returns>返回加载后的字库。</returns>
        public static CharLibrary Load(System.IO.Stream reader, byte[] password = null) {
            Symbol.IO.Packing.TreePackage data = Symbol.IO.Packing.TreePackage.Load(reader, password);
            if (data == null || data.Count < 5 || !data.ContainsKey("name"))
                return null;
            CharLibrary result = new CharLibrary();
            LoadValues(data, result);
            return result;
        }

        #region LoadValues
        static void LoadValues(Symbol.IO.Packing.TreePackage data, CharLibrary result) {
            result._data = data;
            result._name = data["name"] as string;
            result._version = data["version"] as string;
            result._description = data["description"] as string;
            result._width = TypeExtensions.Convert<int>(data["width"], 0);
            result._height = TypeExtensions.Convert<int>(data["height"], 0);
            result._charWidth = TypeExtensions.Convert<int>(data["charWidth"], 0);
            result._charHeight = TypeExtensions.Convert<int>(data["charHeight"], 0);
            result._charMaxWidth = TypeExtensions.Convert<int>(data["charMaxWidth"], 0);
            result._minPoints = TypeExtensions.Convert<int>(data["minPoints"], 0);
            result._needCenterMiddle = TypeExtensions.Convert<bool>(data["needCenterMiddle"], false);
            result._emptyColorR = TypeExtensions.Convert<byte>(data["emptyColorR"], 0);
            result._zool = TypeExtensions.Convert<float>(data["zool"], 1.0F);
            if (result._zool <= 0F)
                result._zool = 1.0F;
            result._pageUrl = data["pageUrl"] as string;
            result._codeUrl = data["codeUrl"] as string;
            result._extension = data["extension"] as string;

            int preCount = TypeExtensions.Convert<int>(data["preHandlers"], 0);
            result._preHandlers.Clear();
            if (preCount > 0) {
                for (int i = 0; i < preCount; i++) {
                    string keyBefore = "pre_" + i + "_";
                    string key = LinqHelper.FirstOrDefault(data.Keys, p => p.StartsWith(keyBefore));
                    if (string.IsNullOrEmpty(key))
                        continue;
                    IPreHandler handler = PreHandlerHelper.CreateInstance(key.Substring(keyBefore.Length));
                    if (handler == null)
                        continue;
                    handler.Load((Symbol.IO.Packing.TreePackage)data[key]);
                    result._preHandlers.Add(handler);
                }
            }

            //int takeCount = TypeExtensions.Convert<int>(data["takeHandlers"], 0);
            //result._takeHandlers.Clear();
            //if (takeCount > 0) {
            //    for (int i = 0; i < takeCount; i++) {
            //        string keyBefore = "take_" + i + "_";
            //        string key = LinqHelper.FirstOrDefault(data.Keys, p => p.StartsWith(keyBefore));
            //        if (string.IsNullOrEmpty(key))
            //            continue;
            //        ITakeHandler handler = TakeHandlerHelper.CreateInstance(key.Substring(keyBefore.Length));
            //        if (handler == null)
            //            continue;
            //        handler.Load((Symbol.IO.Packing.TreePackage)data[key]);
            //        result._takeHandlers.Add(handler);
            //    }
            //}
            //if (result._takeHandlers.Count == 0) {
            //    result._takeHandlers.Add(TakeHandlerHelper.CreateInstance("YDotMatrix"));
            //}

            int charCount = TypeExtensions.Convert<int>(data["chars"], 0);
            result._chars.Clear();
            if (charCount > 0) {
                for (int i = 0; i < charCount; i++) {
                     string keyBefore = "char_" + i + "_";
                    string key = LinqHelper.FirstOrDefault(data.Keys, p => p.StartsWith(keyBefore));
                    if (string.IsNullOrEmpty(key) || key.Length == keyBefore.Length)
                        continue;
                    Symbol.IO.Packing.TreePackage charData = (Symbol.IO.Packing.TreePackage)data[key];
                    if (charData == null)
                        continue;

                    CharInfo charInfo = new CharInfo() { Value = key.Substring(keyBefore.Length)[0] };
                    int pointCount = TypeExtensions.Convert<int>(charData["count"], 0);
                    if (pointCount > 0) {
                        for (int j = 0; j < pointCount; j++) {
                            string keyBefore2 = j + "_";
                            CharPoint charPoint = new CharPoint() {
                                OriginalX = TypeExtensions.Convert<int>(charData[keyBefore2 + "OriginalX"], 0),
                                OriginalY = TypeExtensions.Convert<int>(charData[keyBefore2 + "OriginalY"], 0),
                                X = TypeExtensions.Convert<int>(charData[keyBefore2 + "X"], 0),
                                Y = TypeExtensions.Convert<int>(charData[keyBefore2 + "Y"], 0),
                                R = TypeExtensions.Convert<byte>(charData[keyBefore2 + "R"], 0),
                                G = TypeExtensions.Convert<byte>(charData[keyBefore2 + "G"], 0),
                                B = TypeExtensions.Convert<byte>(charData[keyBefore2 + "B"], 0),
                            };
                            charInfo.Points.Add(charPoint);
                        }
                    }
                    result._chars.Add(charInfo);
                }
            }
            string takeHandlerKeyBefore = "takeHandler_";
            string takeHandlerKey = LinqHelper.FirstOrDefault(data.Keys, p => p.StartsWith(takeHandlerKeyBefore));
            if (!string.IsNullOrEmpty(takeHandlerKey) && takeHandlerKey.Length != takeHandlerKeyBefore.Length) {
                result._takeHandler = TakeHandlerHelper.CreateInstance(takeHandlerKey.Substring(takeHandlerKeyBefore.Length));
                if (result._takeHandler != null) {
                    Symbol.IO.Packing.TreePackage takeHandlerData = (Symbol.IO.Packing.TreePackage)data[takeHandlerKey];
                    result._takeHandler.Load(takeHandlerData);
                }
            }
            if (result._takeHandler == null) {
                result._takeHandler = TakeHandlerHelper.CreateInstance("YDotMatrix");
            }

            string charRecognizerKeyBefore = "charRecognizer_";
            string charRecognizerKey= LinqHelper.FirstOrDefault(data.Keys,p=>p.StartsWith(charRecognizerKeyBefore));
            if (!string.IsNullOrEmpty(charRecognizerKey) && charRecognizerKey.Length!= charRecognizerKeyBefore.Length) {
                result._charRecognizer = CharRecognizerHelper.CreateInstance(charRecognizerKey.Substring(charRecognizerKeyBefore.Length));
                if (result._charRecognizer != null) {
                    Symbol.IO.Packing.TreePackage charRecognizerData = (Symbol.IO.Packing.TreePackage)data[charRecognizerKey];
                    result._charRecognizer.Load(charRecognizerData);
                }
            }
            if (result._charRecognizer == null) {
                result._charRecognizer = CharRecognizerHelper.CreateInstance("Weight3x3");
            }
        }
        #endregion

        #endregion

        #region Pre
        /// <summary>
        /// 预（会抛弃原有图像）
        /// </summary>
        /// <param name="image">图像</param>
        /// <returns></returns>
        public System.Drawing.Bitmap Pre(System.Drawing.Bitmap image) {
            if (_zool != 1.0F) {
                return System.Drawing.BitmapExtensions.Zool(image, (int)(image.Width * _zool), (int)(image.Height * _zool));
            }
            return image;
        }
        #endregion

        #region PreExecute
        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="image">图像</param>
        public void PreExecute(System.Drawing.Bitmap image) {
            if (_preHandlers.Count == 0)
                return;
            foreach (IPreHandler handler in _preHandlers) {
                handler.Execute(image);
            }
        }
        #endregion

        #region TakeChars
        /// <summary>
        /// 字符点阵提取
        /// </summary>
        /// <param name="image">图像</param>
        /// <returns></returns>
        public System.Collections.Generic.List<CharInfo> TakeChars(System.Drawing.Bitmap image) {
            return _takeHandler.Execute(image, this);
        }
        #endregion

        #region CharRecognition
        /// <summary>
        /// 字符识别
        /// </summary>
        /// <param name="charInfo">字符信息</param>
        /// <returns></returns>
        public char? CharRecognition(CharInfo charInfo) {
            return _charRecognizer.Execute(charInfo, this);
        }
        #endregion

        #region CharMatch
        /// <summary>
        /// 字符匹配度
        /// </summary>
        /// <param name="charInfo">字符信息</param>
        /// <returns></returns>
        public float CharMatch(CharInfo charInfo) {
            return _charRecognizer.Match(charInfo, this);
        }
        #endregion

        #endregion
    }

}
