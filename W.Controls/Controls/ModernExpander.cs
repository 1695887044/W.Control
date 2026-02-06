using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace W.Controls.Controls
{
    public class ModernExpander : Expander
    {
        static ModernExpander()
        {
            // 关联默认样式
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ModernExpander),
                new FrameworkPropertyMetadata(typeof(ModernExpander))
            );
        }

        #region 扩展属性

        // 图标（使用 Segoe Fluent Icons 编码）
        //public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        //    nameof(Icon),
        //    typeof(string),
        //    typeof(ModernExpander),
        //    new PropertyMetadata(default(string))
        //);
        public string Icon { get; set; }


        // 圆角
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(ModernExpander),
                new PropertyMetadata(new CornerRadius(8))
            );

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        // 强调色（用于图标和悬停状态）
        public static readonly DependencyProperty AccentColorProperty = DependencyProperty.Register(
            nameof(AccentColor),
            typeof(Brush),
            typeof(ModernExpander),
            new PropertyMetadata(
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"))
            )
        );

        public Brush AccentColor
        {
            get => (Brush)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        #endregion
    }
}
