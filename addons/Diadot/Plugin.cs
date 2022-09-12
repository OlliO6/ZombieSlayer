namespace Diadot;
using System;
using Additions;
using Godot;

#if TOOLS
[Tool]
public class Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        GDAdditions.AddProjectSetting(ProjectSettingsControl.SkipInputSetting, "ui_accept", Variant.Type.String);
        AddCustomType(nameof(Dialog), nameof(Node), GD.Load<CSharpScript>("res://addons/Diadot/Dialog.cs"), null);
    }

    public override void _ExitTree()
    {
        ProjectSettings.Clear(ProjectSettingsControl.SkipInputSetting);
        RemoveCustomType(nameof(Dialog));
    }
}
#endif

public class ProjectSettingsControl
{
    public const string SkipInputSetting = "Diadot/SkipInput";

    public static string SkipInput
    {
        get
        {
            skipInput ??= GDAdditions.GetOrAddProjectSetting(SkipInputSetting, "ui_accept", Variant.Type.String);
            return skipInput;
        }
    }
    private static string skipInput;
}