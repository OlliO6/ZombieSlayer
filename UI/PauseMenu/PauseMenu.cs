using Godot;
using System;

public class PauseMenu : Control
{
    [Signal] public delegate void InventoryPressed();

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
        if (GetNode<Control>("OptionsMenu").Visible) return;

        paused = false;
        GetTree().Paused = false;
        Visible = false;
    }

    private void Pause()
    {
        if (GetTree().Paused) return;

        paused = true;
        GetTree().Paused = true;
        Visible = true;
    }

    [TroughtSignal]
    private void OnMenuButtonPressed() => SceneManager.LoadMenu();

    [TroughtSignal]
    private void OnInventoryButtonPressed()
    {
        Unpause();
        EmitSignal(nameof(InventoryPressed));
    }
}
