using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;

namespace W.Controls.Converters
{
    /// <summary>
    /// 字符串转可见性 (StringVisibilityConverter)
    /// </summary>
    public class StringToVisibilityConverter : BaseMarkupConverter
    {


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = !string.IsNullOrWhiteSpace(value as string);
            // 如果传入参数 'Inverse' 则反转逻辑
            if (parameter?.ToString() == "Inverse") isVisible = !isVisible;

            return isVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
