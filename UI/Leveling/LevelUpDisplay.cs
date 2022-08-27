using System.Collections.Generic;
using Additions;
using Godot;
using Leveling;
using Leveling.Buffs;

public class LevelUpDisplay : PanelContainer
{
    [Signal] public delegate void Opened();
    [Signal] public delegate void Closed();

    private bool open;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion
    #region Content Reference

    private Control storerForContent;
    public Control Content => this.LazyGetNode(ref storerForContent, "%Content");

    #endregion
    #region Title Reference

    private Label storerForTitle;
    public Label Title => this.LazyGetNode(ref storerForTitle, "%Title");

    #endregion

    public void SetTitle(string title) => Title.Text = title;

    public override void _Ready()
    {
        GetNode("%OkButton").Connect("pressed", this, nameof(Close));
        Hide();
    }

    public void AddBuffText(LevelBuff buff)
    {
        if (buff.dontShow) return;

        Content.AddChild(new Label()
        {
            Text = $"- {buff.GetBuffText()}"
        });
    }

    public async void Open()
    {
        if (open) return;

        GetTree().Paused = true;
        AnimationPlayer.Play("Open");

        ToSignal(GetTree(), "idle_frame")
                .OnCompleted(Show);

        await ToSignal(AnimationPlayer, "animation_finished");
        open = true;
        EmitSignal(nameof(Opened));
    }

    public async void Close()
    {
        if (!open) return;

        open = false;
        GetTree().Paused = false;
        AnimationPlayer.Play("Close");

        await ToSignal(AnimationPlayer, "animation_finished");
        Hide();
        EmitSignal(nameof(Closed));
    }
}
