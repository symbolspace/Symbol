/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// Y点阵取字处理器
    /// </summary>
    [Const("Name", "YDotMatrix")]
    [Const("DisplayName", "Y点阵")]
    public class YDotMatrixTakeHandler : ITakeHandler {

        #region fields
        private Symbol.IO.Packing.TreePackage _data = null;
        #endregion

        #region ctor
        /// <summary>
        /// 创建Y点阵取字处理器的实例。
        /// </summary>
        public YDotMatrixTakeHandler() {
            _data = new Symbol.IO.Packing.TreePackage();
            _data.EncryptType = Symbol.IO.Packing.PackageEncryptTypes.BinaryWave_EmptyPassword;
        }
        #endregion

        #region ITakeHandler 成员
        /// <summary>
        /// 名称，唯一（英文）。
        /// </summary>
        public string Name {
            get { return "YDotMatrix"; }
        }
        /// <summary>
        /// 显示名称，界面上显示的中文名称。
        /// </summary>
        public string DisplayName {
            get { return "Y点阵"; }
        }

        /// <summary>
        /// 显示数据，用于预算配置数据。
        /// </summary>
        public string DisplayData {
            get { return string.Empty; }
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
        /// 执行处理
        /// </summary>
        /// <param name="image">需要处理的图像。</param>
        /// <param name="library">字库设置</param>
        /// <returns>返回提取的字符列表，若字库中有数据，会有识别结果。</returns>
        public unsafe System.Collections.Generic.List<CharInfo> Execute(System.Drawing.Bitmap image, CharLibrary library) {
            int width = image.Width;
            int height = image.Height;
            byte emptyColorR = library.EmptyColorR;
            int charMaxWidth = library.CharMaxWidth;
            int charWidth = library.CharWidth;
            int charHeight = library.CharHeight;
            bool needCenterMiddle = library.NeedCenterMiddle;

            float zool = library.Zool;
            if (zool != 1.0F && zool > 0F) {
                charWidth = (int)(charWidth * zool);
                charHeight = (int)(charHeight * zool);
                charMaxWidth = (int)(charMaxWidth * zool);
            }

            System.Collections.Generic.List<CharInfo> result = new System.Collections.Generic.List<CharInfo>();
            CharInfo currentChar = null;
            //对图像进行加锁
            System.Drawing.Imaging.BitmapData originalData = null;
            try {
                originalData = image.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                byte* originalDataScanX = (byte*)originalData.Scan0;//这行代码需要unsafe
                byte* originalDataScan0 = originalDataScanX;//这行代码需要unsafe
                int offset = originalData.Stride - width * 3;
                int currentCharWidth = 0;
                //横向扫描
                for (int x = 0; x < width; x++) {
                    bool spaceY = true;
                    //纵向扫描
                    for (int y = 0; y < height; y++) {
                        originalDataScan0 = originalDataScanX + x * 3 + y * width * 3 + y * offset;
                        if (originalDataScan0[0] == emptyColorR) {
                            continue;
                        }

                        spaceY = false;
                        CharPoint charPoint = new CharPoint() {
                            OriginalX = x,
                            OriginalY = y,
                            R = originalDataScan0[0],
                            G = originalDataScan0[1],
                            B = originalDataScan0[2],
                        };
                        if (currentChar == null) {
                            currentChar = new CharInfo();
                            result.Add(currentChar);
                        }
                        currentChar.Points.Add(charPoint);
                    }
                    if (!spaceY) {
                        currentCharWidth++;
                        if (currentCharWidth == charMaxWidth) {
                            spaceY = true;//超出了，就结束吧
                        }
                    }

                    if (spaceY && currentChar != null) {
                        if (currentChar.Points.Count < 8) {
                            result.Remove(currentChar);
                        } else {
                            currentChar.Points.YZeroOffset();
                            currentChar.Points.XZeroOffset();
                            currentChar.TiltCorrect();
                            if (needCenterMiddle)
                                currentChar.CenterMiddle(charWidth, charHeight);
                            //char? c = library.CharRecognition(currentChar);
                        }
                        currentChar = null;
                        currentCharWidth = 0;
                    }
                }
            } finally {
                if (originalData != null)
                    image.UnlockBits(originalData);
                //image.Dispose();
                //image = null;
            }
            return result;
        }

        #endregion
    }
}
