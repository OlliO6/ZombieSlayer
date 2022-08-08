namespace Additions.Debugging;
using Godot;

public abstract class CommandCollection : Object
{
    public abstract string CollectionName { get; }
    public abstract string Description { get; }

    internal SceneTree sceneTree;
    internal Console console;

    protected void AddOutputLine(string line) => DebugOverlay.AddOutputLine(line, true);
    protected void ColorizeText(string text, Color color) => DebugOverlay.ColorizeText(text, color);
    protected void RefreshOutput() => console.RefreshOutput();
}