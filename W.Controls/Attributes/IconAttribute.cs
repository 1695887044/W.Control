using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace W.Controls.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IconAttribute:Attribute
    {
        public string Icon { get; set; }

        public double IconSize { get; set; } = double.NaN;

        public Brush IconColor { get; set; }

        public PropertyLayoutDirection Layout { get;set; }
    }
}
