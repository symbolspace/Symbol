/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 字符识别器接口
    /// </summary>
    public interface ICharRecognizer {
        /// <summary>
        /// 名称，唯一（英文）。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 显示名称，界面上显示的中文名称。
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// 显示数据，用于预算配置数据。
        /// </summary>
        string DisplayData { get; }
        /// <summary>
        /// 保存数据。
        /// </summary>
        /// <returns>保存配置到数据包。</returns>
        Symbol.IO.Packing.TreePackage Save();
        /// <summary>
        /// 加载数据。
        /// </summary>
        /// <param name="data">从数据包中加载。</param>
        void Load(Symbol.IO.Packing.TreePackage data);

        /// <summary>
        /// 识别字符
        /// </summary>
        /// <param name="charInfo">需要识别的字符信息。</param>
        /// <param name="library">字库设置</param>
        /// <returns>返回识别出来的字符，为null时表示未识别；根据相似度，在字库完善前会识别成相似的字符。</returns>
        char? Execute(CharInfo charInfo,CharLibrary library);

        /// <summary>
        /// 匹配度
        /// </summary>
        /// <param name="charInfo">需要识别的字符信息。</param>
        /// <param name="library">字库设置</param>
        /// <returns>返回匹配度。</returns>
        float Match(CharInfo charInfo, CharLibrary library);
    }
}
