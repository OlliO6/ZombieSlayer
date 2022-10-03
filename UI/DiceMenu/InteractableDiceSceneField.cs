using System;
using System.Collections.Generic;
using Additions;
using Godot;

[Tool]
public class InteractableDiceSceneField : DiceSceneField
{
    [Signal] public delegate void Interacted();

    public override void _Ready()
    {
        base._Ready();

        GetNode("Button").Connect("pressed", this, nameof(OnButtonPressed));
    }

    private void OnButtonPressed() => EmitSignal(nameof(Interacted));
}
