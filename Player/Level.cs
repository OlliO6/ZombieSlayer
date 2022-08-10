using System;
using System.Collections.Generic;
using Additions;
using Godot;

[Additions.Debugging.DefaultColor(nameof(Colors.Aquamarine))]
public class Level : Node
{
    [Export] public int xpToNextLevelUp = 100;

    [Signal] public delegate void LevelReached();

    public void ReachLevel()
    {
        Reached();
        EmitSignal(nameof(LevelReached));
        Debug.LogU(this, "Reached");
    }

    protected virtual void Reached() { }
}
