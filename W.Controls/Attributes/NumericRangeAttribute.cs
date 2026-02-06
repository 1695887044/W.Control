using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W.Controls.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NumericRangeAttribute : Attribute
    {
        public double Minimum { get; }
        public double Maximum { get; }
        public double Increment { get; }

        public NumericRangeAttribute(double min, double max, double increment = 0)
        {
            Minimum = min;
            Maximum = max;
            Increment = increment;
        }
    }
}
