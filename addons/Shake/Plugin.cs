using System;
using Godot;

[Tool]
public class Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        GD.Print("Hello world");
        AddCustomType(nameof(PlayerCamShakeInducer), nameof(Node), GD.Load<CSharpScript>("res://addons/Shake/PlayerCamShakeInducer.cs"), null);
    }

    public override void _ExitTree()
    {
        RemoveCustomType(nameof(PlayerCamShakeInducer));
    }
}
