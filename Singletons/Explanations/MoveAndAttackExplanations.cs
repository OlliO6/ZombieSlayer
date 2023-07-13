namespace Explanations;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Enemies;
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

        player.PauseMode = PauseModeEnum.Process;
        GetTree().Paused = true;

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
            await new TimeAwaiter(this, 0.1f);
            walkDist += prevPos.DistanceTo(player.Position);
        }

        player.PauseMode = PauseModeEnum.Inherit;
        GetTree().Paused = false;

        // Fade out move tutorial
        tween.Kill();
        tween = CreateTween();
        tween.TweenProperty(moveTutorial, "modulate:a", 0f, moveFadeOutTime)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        // Wait until a zombie is close
        List<Zombie> zombies = new();
        while (zombies.Count is 0)
        {
            GetTree().CurrentScene.GetAllChildren<Zombie>(ref zombies);
            zombies = zombies.Where((Zombie zombie) => (zombie.GlobalPosition.DistanceTo(player.GlobalPosition) < minZombieDist)).ToList();
            if (IsQueuedForDeletion()) return;
            await new TimeAwaiter(this, 0.1f);
        }

        float prevDmg = player.damageMultiplier;
        player.damageMultiplier = 1000;

        player.PauseMode = PauseModeEnum.Process;
        GetTree().Paused = true;

        // Fade in attack tutorial
        attackTutorial.Show();
        tween = CreateTween();
        tween.TweenProperty(attackTutorial, "modulate:a", 1f, moveFadeInTime)
                .From(0f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        // Wait for attack
        await ToSignal(player.WeaponInv.CurrentWeapon, nameof(WeaponBase.AttackStarted));

        player.damageMultiplier = prevDmg;

        // Fade out attack tutorial
        tween.Kill();
        tween = CreateTween();
        tween.TweenProperty(attackTutorial, "modulate:a", 0f, moveFadeOutTime)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        ToSignal(tween, "finished")
                .OnCompleted(Finish);

        player.PauseMode = PauseModeEnum.Inherit;
        GetTree().Paused = false;
    }

    protected override void End()
    {
        Hide();
    }
}
