using Godot;
using System;

public class SelectionKnobs : Control
{
    public AnimationPlayer anim;
    public RngAudioPlayer selectAudioPlayer;

    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        selectAudioPlayer = GetNode<RngAudioPlayer>("SelectAudio");
    }

    public void Select() => anim.Play("Selected");

    public void Deselect()
    {
        anim.Play("Deselected", 0.1f);
        Visible = false;
    }

    public void Up() => anim.Play("Up");

    public void Down() => anim.Play("Down", 0.1f);

    public void Idle() => anim.Play("Idle", 0.1f);
}
