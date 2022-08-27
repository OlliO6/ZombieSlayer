namespace Leveling.Buffs;
using System;
using System.Collections.Generic;
using Additions;
using Godot;

public abstract class LevelBuff : Node
{
    [Signal] public delegate void Applied();
    [Export] public bool dontShow;
    public abstract void Apply();
    public abstract string GetBuffText();
}
