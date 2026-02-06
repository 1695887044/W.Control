using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace W.Controls.Controls
{
    public class StatusBadge : Control
    {
        static StatusBadge()
        {
            // 告诉 WPF 去 Themes/Generic.xaml 找默认样式
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBadge), new FrameworkPropertyMetadata(typeof(StatusBadge)));
        }

        // 1. 核心状态：是否激活 (True/False)
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(StatusBadge), new PropertyMetadata(false));

        // 2. 显示文字
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(StatusBadge), new PropertyMetadata(string.Empty));

        // 3. 激活时的颜色 (默认绿色)
        public Brush ActiveColor
        {
            get { return (Brush)GetValue(ActiveColorProperty); }
            set { SetValue(ActiveColorProperty, value); }
        }
        public static readonly DependencyProperty ActiveColorProperty =
            DependencyProperty.Register("ActiveColor", typeof(Brush), typeof(StatusBadge), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(46, 204, 113))));

        // 4. 未激活时的颜色 (默认灰色)
        public Brush InactiveColor
        {
            get { return (Brush)GetValue(InactiveColorProperty); }
            set { SetValue(InactiveColorProperty, value); }
        }
        public static readonly DependencyProperty InactiveColorProperty =
            DependencyProperty.Register("InactiveColor", typeof(Brush), typeof(StatusBadge), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(149, 165, 166))));

        // 5. 圆角大小 (方便微调样式)
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(StatusBadge), new PropertyMetadata(new CornerRadius(4)));
    }
}
