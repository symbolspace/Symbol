/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */


namespace Symbol.ImageRecognition.Verification {
    /// <summary>
    /// 取字处理器接口
    /// </summary>
    public interface ITakeHandler {
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
        /// 执行处理
        /// </summary>
        /// <param name="image">需要处理的图像。</param>
        /// <param name="library">字库设置</param>
        /// <returns>返回提取的字符列表，若字库中有数据，会有识别结果。</returns>
        System.Collections.Generic.List<CharInfo> Execute(System.Drawing.Bitmap image,CharLibrary library);
    }
}
