#if TOOLS
namespace Diadot;
using System;
using Additions;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    private const string SkipInputSetting = "Diadot/SkipInput";

    public override void _EnterTree()
    {
        GDAdditions.AddProjectSetting(SkipInputSetting, "ui_accept", Variant.Type.String);
    }

    public override void _ExitTree()
    {
        ProjectSettings.Clear(SkipInputSetting);
    }
}

#endif