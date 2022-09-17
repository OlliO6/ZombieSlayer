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
        _ = ProjectSettingsControl.SkipInput;
        _ = ProjectSettingsControl.DefaultDelay;
        _ = ProjectSettingsControl.DelayFactorWhenSkipPressed;

        AddCustomType(nameof(Dialog), nameof(Node), GD.Load<CSharpScript>("res://addons/Diadot/Dialog.cs"), GD.Load<Texture>("res://addons/Diadot/Icons/Bubble.png"));
        AddCustomType(nameof(RedirectDialog), nameof(Node), GD.Load<CSharpScript>("res://addons/Diadot/RedirectDialog.cs"), GD.Load<Texture>("res://addons/Diadot/Icons/Redirect.png"));
    }

    public override void _ExitTree()
    {
        ClearProjectSettings();
        RemoveCustomType(nameof(Dialog));
        RemoveCustomType(nameof(RedirectDialog));
    }

    private void ClearProjectSettings()
    {
        ProjectSettings.Clear("Diadot/SkipInput");
        ProjectSettings.Clear("Diadot/DefaultDelay");
        ProjectSettings.Clear("Diadot/DelayFactorWhenSkipPressed");
    }
}
#endif

public class ProjectSettingsControl
{
    private static string skipInput;
    private static float? defaultDelay;
    private static float? delayFactorWhenSkipPressed;

    public static string SkipInput
    {
        get
        {
            skipInput ??= GDAdditions.GetOrAddProjectSetting("Diadot/SkipInput", "ui_accept", Variant.Type.String);
            return skipInput;
        }
    }
    public static float DefaultDelay
    {
        get
        {
            defaultDelay ??= GDAdditions.GetOrAddProjectSetting("Diadot/DefaultDelay", 0.05f, Variant.Type.Real);
            return defaultDelay.Value;
        }
    }
    public static float DelayFactorWhenSkipPressed
    {
        get
        {
            delayFactorWhenSkipPressed ??= GDAdditions.GetOrAddProjectSetting("Diadot/DelayFactorWhenSkipPressed", 0.02f, Variant.Type.Real);
            return delayFactorWhenSkipPressed.Value;
        }
    }
}