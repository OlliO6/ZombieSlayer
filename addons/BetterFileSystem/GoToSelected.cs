using Godot;

namespace BetterFileSystem
{
    [Tool]
    public class GoToSelected : IconButton
    {
        private void OnPressed()
        {
            BetterFileSystemPlugin.instance.GoToSelected();
            BetterFileSystemPlugin.instance.ManuelUpdate();
        }
    }
}