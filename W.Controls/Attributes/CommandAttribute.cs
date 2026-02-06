using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace W.Controls.Attributes
{

    public class CommandAttribute:Attribute
    {
        public string? Command { get; set; }

        public Object? CommandParam { get; set; }

        public RelativeSourceMode Mode { get; set; } = RelativeSourceMode.Self;

        public int AncestorLevel { get; set; } = 1;

        public Type AncestorType { get; set; } = typeof(UIElement);
    }
}
