namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

[Tool]
public class GamePropertySet : LevelBuff
{
    [Export] public string propertyName = "";
    [Export] public Godot.Collections.Array value = new(new object[] { null });

    public override void Apply()
    {
        (GetTree().CurrentScene as GameState).Set(propertyName, value[0]);
    }

    public override string GetBuffText() => "";

    public override string _GetConfigurationWarning()
    {
        return value.Count is 1 ? string.Empty : "Value needs a size of one";
    }
}