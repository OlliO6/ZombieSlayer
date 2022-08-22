using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;


public class Leveling : Node
{
    [Signal] public delegate void XpChanged();
    [Signal] public delegate void LevelChanged();

    [Export] public int startLevelIndex = 0;
    [Export] public float xpRaiseOneLvlTweenTime = 0.15f;

    public int xpToNextLevel;
    public float interpolatedXp;
    private SceneTreeTween tween;
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
        private set
        {
            currentLevel = value;

            xpToNextLevel = this.GetChildren<Level>()
                    .Where((Level) => Level.GetIndex() <= CurrentLevelIndex)
                    .Sum((level) => level.xpToLevelUp);
        }
    }

    private void SetXp(int to)
    {
        if (to == currentXp) return;

        currentXp = to;
        StartTween();
        EmitSignal(nameof(XpChanged));
    }

    private void StartTween()
    {
        tween?.Kill();
        tween = CreateTween();
        float finalVal = (((float)(CurrentLevelNode.xpToLevelUp - ((float)xpToNextLevel - CurrentXp))) / (float)CurrentLevelNode.xpToLevelUp).Clamp01();
        tween.TweenProperty(this, nameof(interpolatedXp), finalVal, (interpolatedXp - finalVal).Abs() * xpRaiseOneLvlTweenTime)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Cubic)
                .Connect("finished", this, nameof(TweenFinished));
    }

    private void TweenFinished()
    {
        // Check for level up
        while (currentXp >= xpToNextLevel)
        {
            CurrentLevelIndex++;
            if (CurrentLevelIndex >= GetChildCount()) break;

            CurrentLevelNode.ReachLevel();
            EmitSignal(nameof(LevelChanged));

            StartTween();
        }
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
