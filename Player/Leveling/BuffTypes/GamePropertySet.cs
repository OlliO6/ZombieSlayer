namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class GamePropertySet : LevelBuff
{
    [Export] public string propertyName = "";
    [Export] public object value;

    public override void Apply()
    {
        (GetTree().CurrentScene as GameState).Set(propertyName, value);
    }

    public override string GetBuffText() => "";
}