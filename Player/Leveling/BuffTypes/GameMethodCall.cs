namespace Leveling.Buffs;
using System.Collections;
using Additions;
using Godot;

public class GameMethodCall : LevelBuff
{
    public string methodName = "";
    public Godot.Collections.Array args = new();

    public override void Apply()
    {
        (GetTree().CurrentScene as GameState).Callv(methodName, args);
    }

    public override string GetBuffText() => "";
}