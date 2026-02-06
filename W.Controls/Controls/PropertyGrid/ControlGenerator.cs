using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using W.Controls.Attributes;

namespace W.Controls.Controls.PropertyGrid
{
    public interface IControlGenerator
    {
        // 优先级
        int Priority { get; }

        // 判断当前属性是否由该生成器处理
        bool CanProcess(PropertyInfo prop, Type targetType);

        // 创建控件的具体逻辑
        FrameworkElement Create(PropertyInfo prop, object bindingSource);
    }

    /// <summary>
    /// 生成枚举类型的控件
    /// </summary>
    public class EnumGenerator : IControlGenerator
    {
        public int Priority => 100;

        public bool CanProcess(PropertyInfo prop, Type targetType)
        {
            // 只读判断：没有 Setter 或者 标记了 [ReadOnly(true)]
            bool isReadOnly =
                !prop.CanWrite
                || prop.GetCustomAttribute<System.ComponentModel.ReadOnlyAttribute>()?.IsReadOnly
                    == true;
            return prop.PropertyType.IsEnum && !isReadOnly;
        }

        public FrameworkElement Create(PropertyInfo prop, object bindingSource)
        {
            var comboBox = new ComboBox
            {
                // Style = (Style)Application.Current.FindResource("ModernComboBox"),
            };
            comboBox.ItemsSource = Enum.GetValues(prop.PropertyType)
                .Cast<object>()
                .Select(value => new
                {
                    Key = value,
                    Value = ControlGeneratorExtensions.GetEnumDisplayName(prop.PropertyType, value),
                });
            comboBox.DisplayMemberPath = "Value";
            comboBox.SelectedValuePath = "Key";
            ControlGeneratorExtensions.SetTwoWayBinding(
                comboBox,
                ComboBox.SelectedValueProperty,
                prop,
                bindingSource
            );
            return comboBox;
        }
    }

    /// <summary>
    /// 生成TextBlock控件
    /// </summary>
    public class TextBlockGenerator : IControlGenerator
    {
        public int Priority => 100;

        public bool CanProcess(PropertyInfo prop, Type targetType)
        {
            // 排除布尔值（布尔值通常用 CheckBox 或 Switch）
            if (targetType == typeof(bool))
                return false;

            // 核心判断：字符串或基础数值类型
            bool isBasicType =
                targetType == typeof(string)
                || targetType.IsPrimitive
                || targetType == typeof(decimal);

            // 只读判断：没有 Setter 或者 标记了 [ReadOnly(true)]
            bool isReadOnly =
                !prop.CanWrite
                || prop.GetCustomAttribute<System.ComponentModel.ReadOnlyAttribute>()?.IsReadOnly
                    == true;

            return isBasicType && isReadOnly;
        }

        public FrameworkElement Create(PropertyInfo prop, object bindingSource)
        {
            var ctl = new TextBlock
            {
                //Style = (Style)Application.Current.FindResource("PropertyTextBoxStyle"),
            };
            ControlGeneratorExtensions.SetTwoWayBinding(
                ctl,
                TextBlock.TextProperty,
                prop,
                bindingSource
            );
            return ctl;
        }
    }

    /// <summary>
    /// 生成输入框控件
    /// </summary>
    public class TextBoxGenerator : IControlGenerator
    {
        public int Priority => 100;

        public bool CanProcess(PropertyInfo prop, Type targetType)
        {
            if (targetType == typeof(bool))
                return false;

            // 核心判断：字符串或基础数值类型
            bool isBasicType =
                targetType == typeof(string)
                || targetType.IsPrimitive
                || targetType == typeof(decimal);

            // 只读判断：没有 Setter 或者 标记了 [ReadOnly(true)]
            bool isReadOnly =
                !prop.CanWrite
                || prop.GetCustomAttribute<System.ComponentModel.ReadOnlyAttribute>()?.IsReadOnly
                    == true;

            return isBasicType && !isReadOnly;
        }

        public FrameworkElement Create(PropertyInfo prop, object bindingSource)
        {
            var ctl = new ModernTextBox
            {
                //Style = (Style)Application.Current.FindResource("PropertyTextBoxStyle"),
            };
            ControlGeneratorExtensions.SetTwoWayBinding(
                ctl,
                ModernTextBox.TextProperty,
                prop,
                bindingSource
            );
            return ctl;
        }
    }

    /// <summary>
    /// 生成选择框控件
    /// </summary>
    public class CheckBoxGenerator : IControlGenerator
    {
        public int Priority => 100;

        public bool CanProcess(PropertyInfo prop, Type targetType)
        {
            if (targetType != typeof(bool))
                return false;

            // 只读判断：没有 Setter 或者 标记了 [ReadOnly(true)]
            bool isReadOnly =
                !prop.CanWrite
                || prop.GetCustomAttribute<System.ComponentModel.ReadOnlyAttribute>()?.IsReadOnly
                    == true;
            return !isReadOnly;
        }

        public FrameworkElement Create(PropertyInfo prop, object bindingSource)
        {
            var ctl = new CheckBox
            {
                //Style = (Style)Application.Current.FindResource("PropertyTextBoxStyle"),
            };
            ControlGeneratorExtensions.SetTwoWayBinding(
                ctl,
                CheckBox.IsCheckedProperty,
                prop,
                bindingSource
            );
            return ctl;
        }
    }

    /// <summary>
    /// 日期
    /// </summary>
    public class DatePickerGenerator : IControlGenerator
    {
        public int Priority => 100;

        public bool CanProcess(PropertyInfo prop, Type targetType)
        {
            bool isReadOnly =
                !prop.CanWrite
                || prop.GetCustomAttribute<System.ComponentModel.ReadOnlyAttribute>()?.IsReadOnly
                    == false;
            return prop.PropertyType == typeof(DateTime)
                || prop.PropertyType == typeof(DateTime?) && !isReadOnly;
        }

        public FrameworkElement Create(PropertyInfo prop, object bindingSource)
        {
            var ctl = new DatePicker
            {
                //Style = (Style)Application.Current.FindResource("PropertyTextBoxStyle"),
            };
            ControlGeneratorExtensions.SetTwoWayBinding(
                ctl,
                DatePicker.SelectedDateProperty,
                prop,
                bindingSource
            );
            return ctl;
        }
    }

    public class BoolStateGenerator : IControlGenerator
    {
        public int Priority => 100;

        public bool CanProcess(PropertyInfo prop, Type targetType)
        {
            if (targetType != typeof(bool))
                return false;

            // 只读判断：没有 Setter 或者 标记了 [ReadOnly(true)]
            bool isReadOnly =
                !prop.CanWrite
                || prop.GetCustomAttribute<System.ComponentModel.ReadOnlyAttribute>()?.IsReadOnly
                    == true;
            return isReadOnly;
        }

        public FrameworkElement Create(PropertyInfo prop, object bindingSource)
        {
            var ctl = new StatusBadge
            {
                //Style = (Style)Application.Current.FindResource("PropertyTextBoxStyle"),
            };
            ControlGeneratorExtensions.SetTwoWayBinding(
                ctl,
                StatusBadge.IsActiveProperty,
                prop,
                bindingSource,
                System.Windows.Data.BindingMode.OneWay
            );
            return ctl;
        }
    }

    public class TypeGenerator : IControlGenerator
    {
        PropertyItemAttribute? att;
        public int Priority => 0;

        public bool CanProcess(PropertyInfo prop, Type targetType)
        {
            att = prop.GetCustomAttribute<PropertyItemAttribute>();
            return att != null;
        }

        public FrameworkElement Create(PropertyInfo prop, object bindingSource)
        {
            if (Activator.CreateInstance(att.Type) is FrameworkElement frm)
            {
                frm.DataContext = prop;
                return frm;
            }
            return new TextBlock
            {
                Text = $"Unsupported: {prop.PropertyType.Name}",
                Foreground = Brushes.Red,
            };
        }
    }

    public class NumericRangeGenerator : IControlGenerator
    {
        // 设置较高优先级，确保它在通用 TextBox 之前执行
        public int Priority => 10;

        public bool CanProcess(PropertyInfo property,Type targetType)
        {
            // 只要属性标有 NumericRangeAttribute 就可以生成
            return property.GetCustomAttribute<NumericRangeAttribute>() != null;
        }


        public FrameworkElement Create(PropertyInfo property, object source)
        {
            var attr = property.GetCustomAttribute<NumericRangeAttribute>();

            // 1. 实例化我们优化过的 UI 控件
            var numericInput = new ModernNumericInput
            {
                Minimum = attr.Minimum,
                Maximum = attr.Maximum,
                Increment = attr.Increment,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 120 // 或者根据需求自适应
            };

            // 2. 创建绑定
            // 注意：UpdateSourceTrigger=PropertyChanged 配合我们的长按加速效果更佳
            Binding binding = new Binding(property.Name)
            {
                Source = source,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnExceptions = true,
                NotifyOnValidationError = true
            };

            // 3. 应用绑定到控件的 Value 依赖属性
            BindingOperations.SetBinding(numericInput, ModernNumericInput.ValueProperty, binding);

            return numericInput;
        }
    }
}
