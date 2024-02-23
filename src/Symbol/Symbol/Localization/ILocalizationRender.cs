namespace Symbol.Localization
{
    /// <summary>
    /// 接口：本地化渲染器。
    /// </summary>
    public interface ILocalizationRender
    {
        /// <summary>
        /// 本地化绑定。
        /// </summary>
        /// <param name="localizationRenderManager">本地化渲染器管理。</param>
        /// <param name="element">当前元素。</param>
        /// <returns></returns>
        void BindLocalization(ILocalizationRenderManager localizationRenderManager, object element);

        /// <summary>
        /// 本地化渲染。
        /// </summary>
        /// <param name="localizationRenderManager">本地化渲染器管理。</param>
        /// <param name="rootElement">根元素，定义或容纳当前元素的对象。</param>
        /// <param name="element">当前元素。</param>
        /// <remarks>若当前元素继承自<see cref="ILocalizationRender"/>，则element为this。</remarks>
        void RenderLocalization(ILocalizationRenderManager localizationRenderManager, object rootElement, object element);
    }

}