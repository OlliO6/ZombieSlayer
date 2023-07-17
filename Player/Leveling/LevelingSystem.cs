namespace Leveling;
using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;


public class LevelingSystem : Node
{
    [Signal] public delegate void XpChanged();
    [Signal] public delegate void LevelChanged();
    [Signal] public delegate void LevelUp();

    [Export] public int startLevelIndex = 0;
    [Export] public float xpRaiseOneLvlTweenTime = 0.15f;
    [Export] public int xpToLevelUpWhenReachedMaxLevel = 100;

    public int xpToNextLevel;
    public float interpolatedXp;
    public bool reachedMaxLevel = false;

    private SceneTreeTween tween;
    private int currentLevel;
    private int currentXp;

    #region Levels Reference

    private Node storerForLevels;
    public Node Levels => this.LazyGetNode(ref storerForLevels, "Levels");

    #endregion

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    public Level CurrentLevelNode => Levels.GetChild<Level>(CurrentLevelIndex);
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
            if (value >= Levels.GetChildCount())
            {
                reachedMaxLevel = true;
                xpToNextLevel += xpToLevelUpWhenReachedMaxLevel;
                return;
            }

            currentLevel = value;

            xpToNextLevel = Levels.GetChildren<Level>()
                    .Where((Level) => Level.GetIndex() <= CurrentLevelIndex)
                    .Sum((level) => level.xpToLevelUp);
        }
    }

    private void SetXp(int to)
    {
        if (to == currentXp || reachedMaxLevel) return;

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
        // Check for level ups
        while (currentXp >= xpToNextLevel)
        {
            LvlUp();
        }
    }

    public void LvlUp()
    {
        CurrentLevelIndex++;
        AnimationPlayer.Play("LevelUp");

        ToSignal(CurrentLevelNode, nameof(Level.LevelReached)).OnCompleted(() =>
        {
            EmitSignal(nameof(LevelChanged));
            EmitSignal(nameof(LevelUp));
        });
        if (!reachedMaxLevel)
            CurrentLevelNode.ReachLevel();
        StartTween();
    }

    public override void _Ready()
    {
        CallDeferred(nameof(Reset));
    }

    public void Reset()
    {
        CurrentXp = 0;
        CurrentLevelIndex = startLevelIndex;
        if (CurrentLevelIndex >= Levels.GetChildCount()) return;

        CurrentLevelNode.ReachLevel();
        EmitSignal(nameof(LevelChanged));
    }
}
