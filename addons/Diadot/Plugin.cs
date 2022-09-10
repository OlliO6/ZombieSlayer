#if TOOLS
namespace Diadot;
using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    public const string SkipInput = "Diadot/SkipInput";

    public override void _EnterTree()
    {
        AddProjectSettings();
    }

    private void AddProjectSettings()
    {
        const string DefaultSkipInput = "ui_accept";
        if (!ProjectSettings.HasSetting(SkipInput))
        {
            ProjectSettings.SetSetting(SkipInput, DefaultSkipInput);
            ProjectSettings.AddPropertyInfo(new()
                {
                    {"name", SkipInput},
                    {"tye", Variant.Type.String}
                });
            ProjectSettings.SetInitialValue(SkipInput, DefaultSkipInput);
        }
    }

    public override void _ExitTree()
    {

    }
}

#endif