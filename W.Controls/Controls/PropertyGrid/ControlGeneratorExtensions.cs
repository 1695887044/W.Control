using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace W.Controls.Controls.PropertyGrid
{
    internal static class ControlGeneratorExtensions
    {
        // 帮助方法：设置双向绑定
        public static void SetTwoWayBinding(FrameworkElement element, DependencyProperty dp, PropertyInfo prop, object sourceObject,BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new System.Windows.Data.Binding(prop.Name)
            {
                Source = sourceObject,
                Mode = mode,
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            };
            element.SetBinding(dp, binding);

        }
        public static string GetEnumDisplayName(Type enumType, object value)
        {
            var field = enumType.GetField(value.ToString());
            var displayAttr = (DisplayAttribute)
                Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
            return displayAttr?.Name ?? value.ToString();
        }

    }
}
