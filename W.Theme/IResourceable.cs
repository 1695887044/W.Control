
namespace W.Themes;

public interface IResourceable
{
    string Name { get; }
    ResourceDictionary Resource { get; }
}
