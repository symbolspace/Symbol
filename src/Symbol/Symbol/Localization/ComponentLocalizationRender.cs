using Symbol.Localization;
using System;
using System.ComponentModel;
using System.Reflection;

[assembly: LocalizationRender(typeof(Component), typeof(ComponentLocalizationRender))]

namespace Symbol.Localization
{
    class ComponentLocalizationRender : ILocalizationRender
    {
        public void BindLocalization(ILocalizationRenderManager localizationRenderManager, object element)
        {

        }
        public void RenderLocalization(ILocalizationRenderManager localizationRenderManager, object rootElement, object element)
        {
            Type type = element.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.NonPublic);

            for (var i = 0; i < fields.Length; i++)
            {
                object child = fields[i].GetValue(element);
                if (element == null)
                    continue;
                localizationRenderManager.Render(child, element);
            }
        }
    }
}
