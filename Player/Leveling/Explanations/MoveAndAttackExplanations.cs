using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class MoveAndAttackExplanations : Explanation
{
    [Export] private float startDelay, minWalkDist, minZombieDist;

    [Export] private float moveFadeInTime = 0, moveFadeOutTime = 0;
    [Export] private float attackFadeInTime = 0, attackFadeOutTime = 0;

    protected override async void Start()
    {
        var player = Player.currentPlayer;
        Control moveTutorial = GetNode<Control>("MoveTutorial");
        Control attackTutorial = GetNode<Control>("AttackTutorial");

        GetTree().Paused = true;
        player.PauseMode = PauseModeEnum.Process;

        Show();
        moveTutorial.Hide();
        attackTutorial.Hide();

        await new TimeAwaiter(this, startDelay);

        // Fade in move tutorial
        moveTutorial.Show();
        var tween = CreateTween();
        tween.TweenProperty(moveTutorial, "modulate:a", 1f, moveFadeInTime)
                .From(0f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        // Wait for player to move a bit
        float walkDist = 0;
        while (walkDist < minWalkDist)
        {
            Vector2 prevPos = player.Position;
            await ToSignal(GetTree(), "physics_frame");
            walkDist += prevPos.DistanceTo(player.Position);
        }

        GetTree().Paused = false;
        player.PauseMode = PauseModeEnum.Inherit;

        // Fade out move tutorial
        tween.Kill();
        tween = CreateTween();
        tween.TweenProperty(moveTutorial, "modulate:a", 0f, moveFadeOutTime)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        // Wait until a zombie is close
        List<Zombie> zombies = new();
        while (!zombies.Any((Zombie zombie) => (zombie.GlobalPosition.DistanceTo(player.GlobalPosition) < minZombieDist)))
        {
            GetTree().CurrentScene.GetAllChildren<Zombie>(ref zombies);
            await ToSignal(GetTree(), "physics_frame");
        }

        // Fade in attack tutorial
        attackTutorial.Show();
        tween = CreateTween();
        tween.TweenProperty(attackTutorial, "modulate:a", 1f, moveFadeInTime)
                .From(0f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        // Wait for attack
        await ToSignal(player.WeaponInv.CurrentWeapon, nameof(WeaponBase.Attacked));

        // Fade out attack tutorial
        tween.Kill();
        tween = CreateTween();
        tween.TweenProperty(attackTutorial, "modulate:a", 0f, moveFadeOutTime)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        ToSignal(tween, "finished")
                .OnCompleted(Finish);
    }

    protected override void End()
    {
        Hide();
    }
}