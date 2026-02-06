using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W.Controls.Converters
{
    /// <summary>
    /// 数值转布尔
    /// </summary>
    public class NumericToBoolConverter : BaseMarkupConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            return System.Convert.ToDouble(value) > 0;
        }
    }
}
