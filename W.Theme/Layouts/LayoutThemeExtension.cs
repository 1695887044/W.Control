

using System.Windows.Markup;
using W.Themes.Extensions;

namespace W.Themes.Layouts;

public class LayoutThemeExtension : MarkupExtension
{
    public LayoutThemeType Type { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this.Type.GetLayoutResource();
    }
}
