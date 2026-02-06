using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W.Controls.Attributes
{
    public enum PropertyLayoutDirection
    {
        Left,   // 标签在左 (经典左右)
        Right,  // 标签在右 (较少见，特定场景)
        Top,    // 标签在上 (现代响应式)
        Bottom  // 标签在下
    }
    public class GroupAttribute:Attribute
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public int ColumnWeight { get; set; } = 12;
    }
}
