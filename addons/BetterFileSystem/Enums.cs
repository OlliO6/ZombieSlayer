namespace BetterFileSystem
{
    public enum FilterState
    {
        OnEnabled,
        OnDisabled
    }
    public enum IncludeType
    {
        Include,
        Exclude
    }
    public enum FileFilterType
    {
        DerivedType,
        MatchType,
        PathContains,
        PathMatch,
        NameContains,
        NameMatch,
    }
    public enum DirectoryFilterType
    {
        PathMatch,
        PathContains,
        NameMatch,
        NameContains
    }
}