

using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace W.Themes.Colors;

[Display(Name = "浅色（推荐）", GroupName = "强力推荐", Description = "纯色", Order = 10, Prompt = "强力推荐")]
public class LightColorResource : ColorResourceBase
{
    public LightColorResource()
    {
        this.IsDark = false;
    }
    public override ResourceDictionary Resource => new ResourceDictionary()
    {
        Source = new Uri("pack://application:,,,/W.Theme;component/Colors/Light.xaml")
    };
}
