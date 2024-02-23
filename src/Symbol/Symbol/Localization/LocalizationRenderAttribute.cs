using System;

namespace Symbol.Localization
{
    /// <summary>
    /// 特性：本地化渲染器。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class LocalizationRenderAttribute : Attribute
    {
        /// <summary>
        /// 获取元素类型。
        /// </summary>
        public Type ElementType { get; private set; }
        /// <summary>
        /// 获取渲染器类型。
        /// </summary>
        public Type RenderType { get; private set; }

        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="elementType">元素类型。</param>
        /// <param name="renderType">渲染器类型。</param>
        public LocalizationRenderAttribute(Type elementType, Type renderType)
        {
            Throw.CheckArgumentNull(elementType, nameof(elementType));
            Throw.CheckArgumentNull(renderType, nameof(renderType));
            if (!TypeExtensions.IsInheritFrom(renderType, typeof(ILocalizationRender)))
            {
                Throw.InvalidCast(string.Format("“{0}”未继承自“{1}”。", renderType.FullName, typeof(ILocalizationRender).FullName));
            }
            ElementType = elementType;
            RenderType = renderType;
        }

    }

}