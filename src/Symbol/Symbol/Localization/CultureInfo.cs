namespace Symbol.Localization
{
    /// <summary>
    /// 区域信息。
    /// </summary>
    public class CultureInfo
    {
        /// <summary>
        /// 获取或设置区域名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取或设置区域文本。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 输出为文本，便于显示。
        /// </summary>
        /// <returns>输出：<see cref="Text"/>。</returns>
        public override string ToString()
        {
            return Text;
        }
    }

}