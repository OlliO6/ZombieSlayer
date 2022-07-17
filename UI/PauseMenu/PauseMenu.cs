using Godot;
using System;

public class PauseMenu : Control
{
    private bool paused;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Pause"))
        {
            if (paused) Unpause();
            else Pause();
        }
    }

    private void Unpause()
    {
        paused = false;
        GetTree().Paused = false;
        Visible = false;
    }

    private void Pause()
    {
        paused = true;
        GetTree().Paused = true;
        Visible = true;
    }

    [TroughtSignal]
    private void OnMenuButtonPressed()
    {
        SceneManager.LoadMenu();
    }
}
