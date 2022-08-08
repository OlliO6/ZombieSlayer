#if TOOLS
namespace Additions.Debugging;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        AddAutoloadSingleton("Debug", "res://addons/DebugOverlay/DebugOverlay.tscn");
    }
    public override void _ExitTree()
    {
        RemoveAutoloadSingleton("Debug");
    }
}
#endif