using Godot;
using System.Collections.Generic;
using Additions;

public class BootSplash : CanvasLayer
{
    public override void _Ready()
    {
        GetNode<Control>("Control").SetShaderParam("useScaling", OptionsManager.useUpscaling);
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
