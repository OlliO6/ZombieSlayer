using System;
using System.Collections.Generic;
using Additions;
using Godot;

[Tool]
public class InteractableDiceSceneField : DiceSceneField
{
    [Signal] public delegate void Interacted();

    public override void _GuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") || (@event is InputEventMouseButton mouseInput && mouseInput.IsPressed() && mouseInput.ButtonIndex is (int)ButtonList.Left))
        {
            EmitSignal(nameof(Interacted));
        }
    }
}
