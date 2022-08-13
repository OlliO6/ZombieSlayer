using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;


public class Leveling : Node
{
    [Signal] public delegate void XpChanged();
    [Signal] public delegate void LevelChanged();

    [Export] public int startLevelIndex = 0;

    public int xpToNextLevel;
    private int currentLevel;
    private int currentXp;

    public Level CurrentLevelNode => GetChild<Level>(CurrentLevelIndex);
    public int CurrentXp
    {
        get => currentXp;
        set => SetXp(value);
    }
    public int CurrentLevelIndex
    {
        get => currentLevel;
        set
        {
            currentLevel = value;

            xpToNextLevel = this.GetChildren<Level>()
                    .Where((Level) => Level.GetIndex() <= CurrentLevelIndex)
                    .Sum((level) => level.xpToLevelUp);
        }
    }

    private void SetXp(int to)
    {
        int difference = to - currentXp;

        switch (difference)
        {
            case > 0:
                if (to >= xpToNextLevel)
                {
                    CurrentLevelIndex++;
                    if (CurrentLevelIndex >= GetChildCount()) break;

                    GetChild<Level>(CurrentLevelIndex).ReachLevel();
                    EmitSignal(nameof(LevelChanged));
                }
                break;

            case 0: return;
        }

        currentXp = to;
        EmitSignal(nameof(XpChanged));
    }

    public override void _Ready()
    {
        CallDeferred(nameof(Reset));
    }

    public void Reset()
    {
        CurrentXp = 0;
        CurrentLevelIndex = startLevelIndex;
        if (CurrentLevelIndex >= GetChildCount()) return;

        GetChild<Level>(CurrentLevelIndex).ReachLevel();
        EmitSignal(nameof(LevelChanged));
    }
}
