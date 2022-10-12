namespace Leveling.Buffs;
using System;
using System.Collections.Generic;
using Additions;
using Godot;

public abstract class LevelBuff : Node
{
    public event Action Applied;
    public bool dontShow;
    public string overridingText = string.Empty;
    public abstract void Apply();
    public abstract string GetBuffText();
    public void InvokeApplied() => Applied?.Invoke();
}
