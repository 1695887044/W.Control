using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using W.Controls.Attributes;
using W.Controls.Helper;

namespace W.Controls.Helper
{
    public static class IconElement
    {
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.RegisterAttached(
                "IconSize",
                typeof(double),
                typeof(IconElement),
                new PropertyMetadata(double.NaN, OnIconChanged)
            ); // NaN 表示默认跟随控件

        public static void SetIconSize(DependencyObject element, double value) =>
            element.SetValue(IconSizeProperty, value);

        public static double GetIconSize(DependencyObject element) =>
            (double)element.GetValue(IconSizeProperty);

        // --- 附加属性：图标编码 ---
        public static readonly DependencyProperty IconCodeProperty =
            DependencyProperty.RegisterAttached(
                "IconCode",
                typeof(string),
                typeof(IconElement),
                new PropertyMetadata(null, OnIconChanged)
            );

        public static void SetIconCode(DependencyObject element, string value) =>
            element.SetValue(IconCodeProperty, value);

        public static string GetIconCode(DependencyObject element) =>
            (string)element.GetValue(IconCodeProperty);

        // --- 附加属性：图标颜色 ---
        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.RegisterAttached(
                "IconColor",
                typeof(Brush),
                typeof(IconElement),
                new PropertyMetadata(Brushes.Black)
            );

        public static void SetIconColor(DependencyObject element, Brush value) =>
            element.SetValue(IconColorProperty, value);

        public static Brush GetIconColor(DependencyObject element) =>
            (Brush)element.GetValue(IconColorProperty);

        // --- 附加属性：位置 ---
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached(
                "Placement",
                typeof(PropertyLayoutDirection),
                typeof(IconElement),
                new PropertyMetadata(PropertyLayoutDirection.Left, OnIconChanged)
            );

        public static void SetPlacement(DependencyObject element, PropertyLayoutDirection value) =>
            element.SetValue(PlacementProperty, value);

        public static PropertyLayoutDirection GetPlacement(DependencyObject element) =>
            (PropertyLayoutDirection)element.GetValue(PlacementProperty);

        // 内部备份属性
        private static readonly DependencyProperty OriginalPaddingProperty =
            DependencyProperty.RegisterAttached(
                "OriginalPadding",
                typeof(Thickness?),
                typeof(IconElement),
                new PropertyMetadata(null)
            );

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (element.IsLoaded)
                    UpdateIcon(element);
                else
                    element.Loaded += (s, arg) => UpdateIcon(element);
            }
        }

        private static void UpdateIcon(FrameworkElement element)
        {
            string code = GetIconCode(element);

            // 分流处理：TextBlock 走 Inline，其他(Button, TextBox等)走 Adorner
            if (element is TextBlock tb)
            {
                UpdateTextBlockInlines(tb, code);
            }
            else if (element is Button  ctrl)
            {
                UpdateControlAdorner(ctrl, code);
            }
            else if (element is ToggleButton ctrl1)
            {
                UpdateControlAdorner(ctrl1, code);
            }
        }

        // --- 方案 A: TextBlock 的 Inline 插入 ---
        private static void UpdateTextBlockInlines(TextBlock tb, string code)
        {
            // 清理旧图标 Run
            var existing = tb.Inlines.Where(i => i.Tag?.ToString() == "IconPart").ToList();
            foreach (var item in existing)
                tb.Inlines.Remove(item);

            if (string.IsNullOrEmpty(code))
                return;

            var iconRun = new Run(code)
            {
                Tag = "IconPart",
                FontFamily = new FontFamily("Segoe Fluent Icons, Segoe MDL2 Assets"),
                Foreground = GetIconColor(tb),
            };
            var spaceRun = new Run(" ") { Tag = "IconPart" };

            if (GetPlacement(tb) == PropertyLayoutDirection.Left)
            {
                tb.Inlines.InsertBefore(tb.Inlines.FirstInline, spaceRun);
                tb.Inlines.InsertBefore(spaceRun, iconRun);
            }
            else
            {
                tb.Inlines.Add(spaceRun);
                tb.Inlines.Add(iconRun);
            }
        }

        // --- 方案 B: Control 的 Adorner 劫持 ---
        private static void UpdateControlAdorner(Control ctrl, string code)
        {
            var layer = AdornerLayer.GetAdornerLayer(ctrl);
            if (layer == null)
                return;

            // 移除旧装饰器
            var existing = layer.GetAdorners(ctrl);
            if (existing != null)
                foreach (var ad in existing)
                    if (ad is IconAdorner)
                        layer.Remove(ad);

            if (!string.IsNullOrEmpty(code))
            {
                // 备份并修改 Padding
                if (ctrl.GetValue(OriginalPaddingProperty) == null)
                    ctrl.SetValue(OriginalPaddingProperty, ctrl.Padding);

                Thickness origin = (Thickness)ctrl.GetValue(OriginalPaddingProperty);
                bool isRight = GetPlacement(ctrl) == PropertyLayoutDirection.Right;
                ctrl.Padding = isRight
                    ? new Thickness(origin.Left, origin.Top, origin.Right + 25, origin.Bottom)
                    : new Thickness(origin.Left + 25, origin.Top, origin.Right, origin.Bottom);

                layer.Add(new IconAdorner(ctrl, code, GetIconColor(ctrl), isRight));
            }
            else if (ctrl.GetValue(OriginalPaddingProperty) is Thickness origin)
            {
                ctrl.Padding = origin;
            }
        }
    }
}
