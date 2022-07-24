using Godot;
using System;

public class FPSLabel : Label
{
    public override void _Process(float delta)
    {
        Text = $"FPS:{Engine.GetFramesPerSecond()}";
    }
}
