using Godot;
using System;

public class PauseMenu : Control
{
    bool paused;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Pause"))
        {
            paused = !paused;

            if (paused) Pause();
            else Unpause();
        }
    }

    private void Unpause()
    {
        GetTree().Paused = false;
        Visible = false;
    }

    private void Pause()
    {
        GetTree().Paused = true;
        Visible = true;
    }
}
