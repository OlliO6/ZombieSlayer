using Additions;
using Godot;
using System;

public class DiceMenu : Control
{
    [Signal] public delegate void OnOpened();
    [Signal] public delegate void OnOpenStarted();
    [Signal] public delegate void OnClosed();

    private bool isOpen;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

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
}
