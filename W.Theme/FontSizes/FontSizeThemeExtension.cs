

using System.Windows.Markup;
using W.Themes.Extensions;

namespace W.Themes.FontSizes;

public class FontSizeThemeExtension : MarkupExtension
{
    public FontSizeThemeType Type { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this.Type.GetFontSizeResource();
    }
}
