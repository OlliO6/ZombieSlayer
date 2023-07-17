using System;
using Godot;

public class Menu : CanvasLayer
{
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
