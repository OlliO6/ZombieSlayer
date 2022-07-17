using Godot;
using System;
using Additions;

public class SceneManager : Node
{
    public static SceneManager instance;

    private PackedScene menuScene = GD.Load<PackedScene>("res://UI/Menu/Menu.tscn");

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    public override void _Ready()
    {
        instance = this;
    }


    public static void LoadMenu() => instance._LoadMenu();
    public static void ChangeScence(PackedScene scene) => instance._ChangeScence(scene);


    private void _LoadMenu() => _ChangeScence(menuScene);

    private async void _ChangeScence(PackedScene scene)
    {
        AnimationPlayer.Play("FadeStart");

        await ToSignal(AnimationPlayer, "animation_finished");

        GetTree().ChangeSceneTo(scene);
        AnimationPlayer.Play("FadeEnd");
        GetTree().Paused = false;
    }
}
