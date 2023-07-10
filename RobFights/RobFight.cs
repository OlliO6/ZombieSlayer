using Godot;
using System;
using Additions;
using Enemies;

public class RobFight : Node2D
{
    [Export] private NodePath _rob;

    public override async void _Ready()
    {
        GetTree().Paused = false;
        var rob = GetNode<FirstAngryRob>(_rob);
        rob.EnemyDied += (_) => EndFight();

        rob.Enabled = false;
        InputManager.ProcessInput = false;
        var anim = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
        anim.Play("StartFight");

        await ToSignal(anim, "animation_finished");
        InputManager.ProcessInput = true;
        rob.Enabled = true;
    }

    private void EndFight()
    {
        Debug.LogU(this, "Fight finished");
    }
}
