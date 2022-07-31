using System.Collections.Generic;
using Additions;
using Godot;

public class BootSplash : CanvasLayer
{
    public override void _Ready()
    {
        GetNode<Control>("Control").SetShaderParam("useScaling", OptionsManager.IsUpscaling);
    }

    private void LoadMenu()
    {
        SceneManager.LoadMenu();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("SkipBootSplash"))
        {
            LoadMenu();
        }
    }
}
