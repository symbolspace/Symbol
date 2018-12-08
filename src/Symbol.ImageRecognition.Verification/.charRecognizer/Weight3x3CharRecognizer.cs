/*
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 加权3x3字符识别器
    /// </summary>
    [Const("Name", "Weight3x3")]
    [Const("DisplayName", "加权3x3")]
    public class Weight3x3CharRecognizer : ICharRecognizer {

        #region fields
        private Symbol.IO.Packing.TreePackage _data = null;
        #endregion

        #region ctor
        /// <summary>
        /// 创建加权3x3字符识别器的实例。
        /// </summary>
        public Weight3x3CharRecognizer() {
            _data = new Symbol.IO.Packing.TreePackage();
            _data.EncryptType = Symbol.IO.Packing.PackageEncryptTypes.BinaryWave_EmptyPassword;
        }
        #endregion

        #region ITakeHandler 成员
        /// <summary>
        /// 名称，唯一（英文）。
        /// </summary>
        public string Name {
            get { return "Weight3x3"; }
        }
        /// <summary>
        /// 显示名称，界面上显示的中文名称。
        /// </summary>
        public string DisplayName {
            get { return "加权3x3"; }
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
        /// 匹配度
        /// </summary>
        /// <param name="charInfo">需要识别的字符信息。</param>
        /// <param name="library">字库设置</param>
        /// <returns>返回匹配度。</returns>
        public float Match(CharInfo charInfo, CharLibrary library){
            float? item = LinqHelper.FirstOrDefault(LinqHelper.OrderByDescending(LinqHelper.Select(library.Chars, p => (float?)CharInfoCompare_3x3(charInfo, p)),p=>p));
            if (item == null)
                return 0F;
            return item.Value;
        }
        /// <summary>
        /// 识别字符
        /// </summary>
        /// <param name="charInfo">需要识别的字符信息。</param>
        /// <param name="library">字库设置</param>
        /// <returns>返回识别出来的字符，为null时表示未识别；根据相似度，在字库完善前会识别成相似的字符。</returns>
        public char? Execute(CharInfo charInfo, CharLibrary library) {
            CharInfo item = LinqHelper.FirstOrDefault(LinqHelper.OrderByDescending(library.Chars, p => CharInfoCompare_3x3(charInfo, p)));
            if (item == null)
                return null;
            return item.Value;
            //float lastN = 0F;
            //CharInfo item = null;
            //foreach (CharInfo p in library.Chars) {
            //    float n = CharInfoCompare_3x3(charInfo, p);
            //    if (n == 0.5F) {//完全匹配了
            //        return p.Value;
            //    }
            //    if (n > lastN) {
            //        lastN = n;
            //        item = p;
            //    }
            //}
            //if (item == null)
            //    return null;
            //return item.Value;

            //System.Collections.Generic.List<CharInfoOrder> xs = new System.Collections.Generic.List<CharInfoOrder>();
            //foreach (CharInfo p in library.Chars) {
            //    float n = CharInfoCompare_3x3(charInfo, p);
            //    if (n ==0.5F) {//完全匹配了
            //        return p.Value;
            //    }
            //    xs.Add(new CharInfoOrder() { n = n, p = p });
            //}
            //if (xs.Count == 0)
            //    return null;
            //xs.Sort((x, y) => y.n.CompareTo(x.n));
            //return xs[0].p.Value;
        }
        #region _weight3x3
        //9宫格加权法
        //private static readonly float[] _weight3x3 = new float[] { 0.2F, 0.2F, 0.2F, 0.2F, 1F, 0.2F, 0.2F, 0.2F, 0.2F };
        private static readonly float[] _weight3x3 = new float[] {
            0.2F, 0.2F, 0.2F,
            0.2F, 1F,   0.2F,
            0.2F, 0.2F, 0.2F };
        private static readonly int[][] _weight3x3_finds = new int[][] { 
                     //left|top               top            right|top
                     //  0                     1                 2
            new int[]{-1,-1},      new int[]{ 0,-1},  new int[]{ 1,-1 },
                     //left                 center             right
                     //  3                     4                 5
            new int[]{ 1, 0},      new int[]{ 0, 0},  new int[]{ 1, 0 },
                     //left|bottom          bottom           right|bottom
                     //  6                     7                 8
            new int[]{-1, 1},      new int[]{ 0, 1},  new int[]{ 1, 1 },
        };
        #endregion
        #region CharInfoCompare_3x3_Weight
        float CharInfoCompare_3x3_Weight(CharInfo charInfo, CharInfo target, CharPoint item,bool add=true) {
            CharPoint itemTarget = target.Points.FindPoint(item.X, item.Y);
            if (itemTarget != null)//中
                return add ? _weight3x3[4] : 0F;
            float result = 0F;
            for (int i = 0; i < _weight3x3_finds.Length; i++) {
                if (i == 4)
                    continue;
                int[] pos = _weight3x3_finds[i];
                CharPoint item1 = charInfo.Points.FindPoint(item.X + pos[0], item.Y + pos[1]);
                CharPoint itemTarget1 = target.Points.FindPoint(item.X + pos[0], item.Y + pos[1]);
                if (item1 != null && itemTarget1 != null) {
                    if (result < _weight3x3[i])
                        result = _weight3x3[i];
                }
            }
            if (add)
                return result;
            else
                return result > 0F ? 0F : 0.2F;
        }
        #endregion
        #region CharInfoCompare_3x3
        float CharInfoCompare_3x3(CharInfo charInfo, CharInfo target) {
            float n = 0F;
            foreach (CharPoint item in charInfo.Points) {
                n += CharInfoCompare_3x3_Weight(charInfo, target, item);
            }
            float n2 = 0F;
            foreach (CharPoint item in target.Points) {
                n2 += CharInfoCompare_3x3_Weight(target, charInfo, item, true);
            }
            n -= (n2 / 2F);
            //if (n2 < n) {
            //    float n3 = n2 - n;
            //    n -= n3;
            //}
            //return n;
            return n / (float)(charInfo.Points.Count + target.Points.Count);
        }
        #endregion

        #endregion
    }
}
