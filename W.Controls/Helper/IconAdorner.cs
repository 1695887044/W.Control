using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace W.Controls.Helper
{
    public class IconAdorner : Adorner
    {
        private readonly string _code;
        private readonly Brush _color;
        private readonly bool _isRight;

        public IconAdorner(UIElement element, string code, Brush color, bool isRight) : base(element)
        {
            _code = code;
            _color = color;
            _isRight = isRight;
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext dc)
        {
            var ctrl = AdornedElement as Control;
            if (ctrl == null || string.IsNullOrEmpty(_code)) return;

            var typeface = new Typeface(new FontFamily("Segoe Fluent Icons, Segoe MDL2 Assets"),
                                       FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            double customSize = IconElement.GetIconSize(ctrl);
            // 如果设置了 IconSize 就用设置值，否则用控件的 FontSize
            double finalSize = double.IsNaN(customSize) ? ctrl.FontSize : customSize;

            var formattedText = new FormattedText(_code, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, typeface, finalSize, _color, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            // 计算位置：根据左右放置在 Padding 区域内
            double x = _isRight ? ctrl.RenderSize.Width - 20 : 6;
            double y = (ctrl.RenderSize.Height - formattedText.Height) / 2;

            dc.DrawText(formattedText, new Point(x, y));
        }
    }
}
