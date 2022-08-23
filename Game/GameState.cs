using System.Collections.Generic;
using Additions;
using Godot;

public class GameState : YSort
{
    public override void _Ready()
    {
        ExplanationsManager.StartExplanation("MoveAndAttack");
    }
}
