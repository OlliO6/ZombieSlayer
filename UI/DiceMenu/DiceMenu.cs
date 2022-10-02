using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class DiceMenu : Control
{
    [Export] private PackedScene diceFieldScene;

    [Signal] public delegate void Opened();
    [Signal] public delegate void OpenStarted();
    [Signal] public delegate void Closed();

    private bool isOpen;
    private int slectedCount;
    private DiceField watchedField;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion
    #region DiceContainer Reference

    private Container storerForDiceContainer;
    public Container DiceContainer => this.LazyGetNode(ref storerForDiceContainer, "%DiceContainer");

    #endregion
    #region DiceScenesContainer Reference

    private DiceScenesContainer storerForDiceScenesContainer;
    public DiceScenesContainer DiceScenesContainer => this.LazyGetNode(ref storerForDiceScenesContainer, "%DiceScenesContainer");

    #endregion
    #region ThrowAllButton Reference

    private Button storerForThrowAllButton;
    public Button ThrowAllButton => this.LazyGetNode(ref storerForThrowAllButton, "%ThrowAllButton");

    #endregion
    #region ThrowSelectedButton Reference

    private Button storerForThrowSelectedButton;
    public Button ThrowSelectedButton => this.LazyGetNode(ref storerForThrowSelectedButton, "%ThrowSelectedButton");

    #endregion

    public override void _Ready()
    {
        Connect(nameof(OpenStarted), this, nameof(UpdateDices));
        ThrowAllButton.Connect("pressed", this, nameof(OnThrowAllPressed));
        ThrowSelectedButton.Connect("pressed", this, nameof(OnThrowSelectedPressed));
    }

    public override void _EnterTree()
    {
        InputManager.DiceMenuPressed += OnDiceMenuPressed;
        InputManager.UICancelPressed += OnUICancelPressed;
    }
    public override void _ExitTree()
    {
        InputManager.DiceMenuPressed -= OnDiceMenuPressed;
        InputManager.UICancelPressed -= OnUICancelPressed;
    }

    private void OnDiceMenuPressed()
    {
        if (isOpen) Close();
        else Open();
    }
    private void OnUICancelPressed()
    {
        if (isOpen) Close();
    }

    #region Open and close 

    private async void Open()
    {
        if (GetTree().Paused == true || isOpen) return;

        EmitSignal(nameof(OpenStarted));

        AnimationPlayer.Stop();
        AnimationPlayer.Play("Open");
        AnimationPlayer.Advance(GetProcessDeltaTime());
        Visible = true;

        SignalAwaiter signalAwaiter = ToSignal(AnimationPlayer, "animation_finished");
        await signalAwaiter;
        if (signalAwaiter.GetResult()[0] is not ("Open")) return;

        isOpen = true;
        GetTree().Paused = true;
        EmitSignal(nameof(Opened));
    }

    private void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        GetTree().Paused = false;
        EmitSignal(nameof(Closed));

        float animationPosition = AnimationPlayer.CurrentAnimationLength - AnimationPlayer.CurrentAnimationPosition;
        AnimationPlayer.Play("Close");
        AnimationPlayer.Advance(animationPosition);
    }

    #endregion

    [TroughtEditor]
    private void UpdateDices()
    {
        if (Player.currentPlayer is null) return;

        slectedCount = 0;

        IEnumerable<Dice> dices = Player.currentPlayer.GetWorkingDices();

        foreach (Node child in DiceContainer.GetChildren())
        {
            DiceContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (Dice dice in dices)
        {
            DiceField diceField = diceFieldScene.Instance<DiceField>();

            diceField.watchable = true;
            diceField.IsSelected = false;
            diceField.IsWatched = false;
            diceField.dice = dice;

            diceField.Connect(nameof(DiceField.Selected), this, nameof(OnDiceFieldSelected));
            diceField.Connect(nameof(DiceField.Deselected), this, nameof(OnDiceFieldDeselected));
            diceField.Connect(nameof(DiceField.WatchStarted), this, nameof(OnDiceFieldWatched));

            DiceContainer.AddChild(diceField);
        }

        ShowDiceScenes(null);

        ThrowSelectedButton.Disabled = true;
        ThrowAllButton.Disabled = DiceContainer.GetChildCount() > 0 ? false : true;
    }

    [TroughtEditor]
    public void OnDiceFieldSelected(DiceField diceField)
    {
        slectedCount++;

        if (slectedCount is 1)
        {
            ThrowSelectedButton.Disabled = false;
        }
    }

    [TroughtEditor]
    public void OnDiceFieldDeselected(DiceField diceField)
    {
        slectedCount--;

        if (slectedCount is <= 0)
        {
            slectedCount = 0;
            ThrowSelectedButton.Disabled = true;
        }
    }

    [TroughtEditor]
    public void OnDiceFieldWatched(DiceField diceField)
    {
        ShowDiceScenes(diceField);
        watchedField = diceField;
    }

    public void ShowDiceScenes(DiceField diceField)
    {
        Dice dice = diceField is null ? null : diceField.dice;

        if (watchedField is not null) watchedField.IsWatched = false;
        watchedField = diceField;

        HSeparator separator = GetNode<HSeparator>("%Separator");

        if (dice is null || dice.scenes is null || dice.scenes.Length is 0)
        {
            separator.Visible = false;
            return;
        }
        separator.Visible = true;
        DiceScenesContainer.Scenes = dice.scenes.ToList();
    }

    [TroughtEditor]
    private void OnThrowAllPressed()
    {
        Close();

        foreach (DiceField diceField in DiceContainer.GetChildren())
        {
            diceField.dice.Throw();
        }
    }

    [TroughtEditor]
    private void OnThrowSelectedPressed()
    {
        Close();

        foreach (DiceField diceField in DiceContainer.GetChildren())
        {
            if (diceField.IsSelected) diceField.dice.Throw();
        }
    }
}
