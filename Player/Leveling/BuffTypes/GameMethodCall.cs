namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class GameMethodCall : LevelBuff
{
    [Export] public string methodName = "";
    [Export] public Godot.Collections.Array args = new();

    public override void Apply()
    {
        (GetTree().CurrentScene as GameState).Callv(methodName, args);
    }

    public override string GetBuffText() => "";
}