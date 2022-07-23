using Additions;
using Godot;
using System;
using System.Collections.Generic;

public class DiceMenu : Control
{
    [Export] private PackedScene diceFieldScene;


    [Signal] public delegate void OnOpened();
    [Signal] public delegate void OnOpenStarted();
    [Signal] public delegate void OnClosed();

    private bool isOpen;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    #region DiceContainer Reference

    private Container storerForDiceContainer;
    public Container DiceContainer => this.LazyGetNode(ref storerForDiceContainer, _DiceContainer);
    [Export] private NodePath _DiceContainer = "DiceContainer";

    #endregion

    #region Open and close 

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("DiceMenu"))
        {
            if (@event.IsPressed()) Open();
            else Close();
        }
    }

    private async void Open()
    {
        if (GetTree().Paused == true) return;

        EmitSignal(nameof(OnOpenStarted));

        AnimationPlayer.Play("Open");
        AnimationPlayer.Advance(GetProcessDeltaTime());
        Visible = true;

        SignalAwaiter signalAwaiter = ToSignal(AnimationPlayer, "animation_finished");
        await signalAwaiter;
        if (signalAwaiter.GetResult()[0] is not ("Open")) return;

        isOpen = true;
        GetTree().Paused = true;
        EmitSignal(nameof(OnOpened));
    }

    private void Close()
    {
        isOpen = false;
        GetTree().Paused = false;
        EmitSignal(nameof(OnClosed));

        float animationPosition = AnimationPlayer.CurrentAnimationLength - AnimationPlayer.CurrentAnimationPosition;
        AnimationPlayer.Play("Close");
        AnimationPlayer.Advance(animationPosition);
    }

    #endregion

    public override void _Ready()
    {
        Connect(nameof(OnOpenStarted), this, nameof(UpdateDices));
    }

    [TroughtSignal]
    private void UpdateDices()
    {
        if (Player.currentPlayer is null) return;

        IEnumerable<Dice> dices = Player.currentPlayer.GetWorkingDices();

        foreach (Node child in DiceContainer.GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        foreach (Dice dice in dices)
        {
            DiceField diceField = diceFieldScene.Instance<DiceField>();

            diceField.dice = dice;

            DiceContainer.AddChild(diceField);
        }
    }

    public void ShowDiceScenes(Dice dice)
    {

    }
}
