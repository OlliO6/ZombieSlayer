namespace Enemies;
using System.Collections.Generic;
using Additions;
using Godot;

public static class EnemyUtilities
{
    public const float InvisTime = 0.075f, StunTime = 0.15f;
    public static PackedScene coinScene = GD.Load<PackedScene>("res://Items/Coin/Coin.tscn");

    public static void BasicSetup(IEnemy enemy)
    {
        enemy.EnemyDied += (_) =>
        {
            if (Player.currentPlayer is null) return;
            Player.currentPlayer.Leveling.CurrentXp += enemy.ExPoints;
        };
    }

    public static void FlashSprite(Sprite sprite, float inTime = 0.05f, float outTime = 0.15f, float flashStrenght = 0.9f)
    {
        var tween = sprite.CreateTween();
        tween.Chain()
                .TweenProperty(sprite, "material:shader_param/flashStrenght", flashStrenght, inTime)
                .From(0f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(sprite, "material:shader_param/flashStrenght", 0f, outTime)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);
    }

    public static void SpawnCoins(Node2D enemy, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Coin coin = coinScene.Instance<Coin>();
            enemy.GetParent().CallDeferred("add_child", coin);
            coin.GlobalPosition = enemy.GlobalPosition;
            coin.Launch();
        }
    }

    public static Vector2 GetDirectionToPlayer(Node2D enemy)
    {
        if (Player.currentPlayer is null)
            return Vector2.Zero;

        return (Player.currentPlayer.GlobalPosition - enemy.GlobalPosition).Normalized();
    }
}
