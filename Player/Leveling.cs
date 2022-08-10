using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;


public class Leveling : Node
{
    [Signal] public delegate void XpChanged();
    [Signal] public delegate void LevelRaised();

    [Export] public int startLevelIndex = 0;
    public int currentLevel;

    private int exPoints;

    public int ExPoints
    {
        get => exPoints;
        set => SetXp(value);
    }

    private void SetXp(int to)
    {
        int difference = to - exPoints;

        switch (difference)
        {
            case > 0:

                int xpToNext = this.GetChildren<Level>().
                        Where((Level) => Level.GetIndex() <= currentLevel)
                        .Sum((level) => level.xpToNextLevelUp);

                if (to >= xpToNext)
                {
                    currentLevel++;
                    if (currentLevel >= GetChildCount()) break;

                    GetChild<Level>(currentLevel).ReachLevel();
                    EmitSignal(nameof(LevelRaised));
                }
                break;

            case 0: return;
        }

        exPoints = to;
        EmitSignal(nameof(XpChanged));
    }


    public override void _Ready()
    {
        currentLevel = startLevelIndex;
        if (currentLevel >= GetChildCount()) return;

        GetChild<Level>(currentLevel).ReachLevel();
        EmitSignal(nameof(LevelRaised));
    }
}
