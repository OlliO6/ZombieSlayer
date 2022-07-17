using Godot;
using System;

public class Dice : Node2D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Interact"))
        {
            
        }
    }
}
