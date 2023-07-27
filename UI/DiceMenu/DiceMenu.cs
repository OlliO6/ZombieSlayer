using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class DiceMenu : Control
{
    [Signal] public delegate void Opened();
    [Signal] public delegate void OpenStarted();
    [Signal] public delegate void Closed();

    [Export] private PackedScene diceFieldScene;

    private bool isOpen;
    private DiceField selectedField;

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    private Container storerForDiceContainer;
    public Container DiceContainer => this.LazyGetNode(ref storerForDiceContainer, "%DiceContainer");

    private DiceScenesContainer storerForDiceScenesContainer;
    public DiceScenesContainer DiceScenesContainer => this.LazyGetNode(ref storerForDiceScenesContainer, "%DiceScenesContainer");

    private Button storerForThrowAllButton;
    public Button ThrowAllButton => this.LazyGetNode(ref storerForThrowAllButton, "%ThrowAllButton");

    private Button storerForThrowSelectedButton;
    public Button ThrowSelectedButton => this.LazyGetNode(ref storerForThrowSelectedButton, "%ThrowSelectedButton");

    public override void _Ready()
    {
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
        if (isOpen)
            Close();
        else
            Open();
    }
    private void OnUICancelPressed()
    {
        if (isOpen)
            Close();
    }

    private async void Open()
    {
        if (GetTree().Paused == true || isOpen) return;

        UpdateDices();
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

    private void UpdateDices()
    {
        if (Player.currentPlayer is null) return;

        IEnumerable<Dice> dices = Player.currentPlayer.GetWorkingDices();

        foreach (Node child in DiceContainer.GetChildren())
        {
            DiceContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (Dice dice in dices)
        {
            DiceField diceField = diceFieldScene.Instance<DiceField>();

            diceField.IsSelected = false;
            diceField.autoSelect = true;
            diceField.dice = dice;

            diceField.Connect(nameof(DiceField.Selected), this, nameof(OnDiceFieldSelected));

            DiceContainer.AddChild(diceField);
        }

        bool hasAtLeastOneDice = DiceContainer.GetChildCount() > 0;

        if (hasAtLeastOneDice)
            DiceContainer.GetChild<DiceField>(0).IsSelected = true;
        else
            OnDiceFieldSelected(null);

        ThrowAllButton.Disabled = !hasAtLeastOneDice;
        ThrowSelectedButton.Disabled = !hasAtLeastOneDice;
    }

    private void OnDiceFieldSelected(DiceField diceField)
    {

        if (IsInstanceValid(selectedField))
            selectedField.IsSelected = false;

        selectedField = diceField;

        HSeparator separator = GetNode<HSeparator>("%Separator");
        Dice dice = diceField is null ? null : diceField.dice;

        if (dice is null || dice.scenes is null || dice.scenes.Length is 0)
        {
            DiceScenesContainer.Scenes = null;
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

        selectedField?.dice.Throw();
    }
}
