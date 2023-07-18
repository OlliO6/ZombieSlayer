#if DEBUG
namespace Additions.Debugging;
using Godot;

public class CommandCollection : Object
{
    public virtual string CollectionName { get; }
    public virtual string Description { get; }

    internal SceneTree sceneTree;
    internal Console console;

    protected void AddOutputLine(string line) => DebugOverlay.AddOutputLine(line, true);
    protected string ColorizeText(string text, Color color) => DebugOverlay.ColorizeText(text, color);
    protected void RefreshOutput() => console.RefreshOutput();
}
#endif