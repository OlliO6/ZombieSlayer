using System;
using Godot;

public class Menu : CanvasLayer
{
    public override void _Ready()
    {
        if (OS.GetName() is "HTML5")
        {
            GetNode<Button>("Control/QuitButton").Visible = false;
        }
    }

    [TroughtEditor]
    private void OnPlayPressed()
    {
        Transitions.StartTransition(Transitions.TransitionPixel, 0.2f, () =>
        {
            GetTree().ChangeSceneTo(Scenes.Game);
            Transitions.EndTransition(Transitions.TransitionPixel);
        });
    }

    [TroughtEditor]
    private void OnQuitPressed()
    {
        Transitions.StartTransition(Transitions.TransitionPixel, 0.5f, () =>
        {
            GetTree().Quit();
        }, false);
    }
}
