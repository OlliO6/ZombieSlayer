namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class JustText : LevelBuff
{
    [Export] public string text;
    public override void Apply() { }
    public override string GetBuffText() => text;
}
