using Godot;


namespace BetterFileSystem
{
    [Tool]
    public class AddFilter : IconButton
    {
        PackedScene filtersScene = GD.Load<PackedScene>("res://addons/BetterFileSystem/Filters.tscn");

        private void OnPressed()
        {
            BetterFileSystemPlugin.instance.GetEditorInterface().OpenSceneFromPath("res://addons/BetterFileSystem/Filters.tscn");
        }
    }
}