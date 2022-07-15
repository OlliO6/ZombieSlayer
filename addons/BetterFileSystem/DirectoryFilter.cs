using Godot;

namespace BetterFileSystem
{
    [Tool]
    public class DirectoryFilter : Node
    {
        [Export] public IncludeType includeType;
        [Export] public DirectoryFilterType filterType;
        [Export] public string filterString = "";
        [Export] public FilterState whenToFilter;
    }
}
