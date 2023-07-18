using System;
using Godot;

public class Menu : CanvasLayer
{
    public override void _Ready()
    {
        if (OS.GetName() is "HTML5")
        {
            GetNode<Button>("Control/CenterContainer/VBoxContainer/QuitButton").Visible = false;
        }
    }

    [TroughtEditor]
    private void OnPlayPressed()
    {
        GetTree().Paused = true;
        Transitions.StartTransition(Transitions.TransitionBlackFade, 0.2f, () =>
        {
            GetTree().Paused = false;
            GetTree().ChangeSceneTo(Scenes.Game);
            Transitions.EndTransition(Transitions.TransitionPixel);
        });
    }

    [TroughtEditor]
    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
