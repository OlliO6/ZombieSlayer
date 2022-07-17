using Godot;
using System;

public class WaveSpawner : Node
{
    [Export] private NodePath spawnPositionsHolder;
    [Export] private float minWaveInterval;
    [Export] private Vector2 waveIntervalMultiplierRange = new Vector2(1.8f, 2.5f), spawnIntervalRange = new Vector2(0.1f, 0.5f);
    [Export] private float enemyCountMultiplier = 3, enemyCountPower = 0.8f;
    [Export] private PackedScene[] enemies;

    [Signal] public delegate void OnWaveStarted();

    public int currentWave;

    public override void _Ready()
    {
        StartWave();
    }

    private async void StartWave()
    {
        RandomNumberGenerator rng = new();
        rng.Randomize();

        currentWave++;
        EmitSignal(nameof(OnWaveStarted));

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        int enemyCount = Mathf.RoundToInt(Mathf.Pow(currentWave * enemyCountMultiplier, enemyCountPower));

        SpawnEnemy(enemyCount, rng);

        await ToSignal(GetTree().CreateTimer((enemyCount * rng.RandfRange(waveIntervalMultiplierRange.x, waveIntervalMultiplierRange.y) + minWaveInterval)), "timeout");

        StartWave();
    }

    private async void SpawnEnemy(int enemyCount, RandomNumberGenerator rng)
    {
        if (enemyCount <= 0) return;

        enemyCount--;

        Node2D enemy = enemies[rng.RandiRange(0, enemies.Length - 1)].Instance<Node2D>();
        GetParent().AddChild(enemy);

        Node spawnPositions = GetNode(spawnPositionsHolder);

        enemy.GlobalPosition = spawnPositions.GetChild<Node2D>(rng.RandiRange(0, spawnPositions.GetChildCount() - 1)).GlobalPosition;

        await ToSignal(GetTree().CreateTimer(rng.RandfRange(spawnIntervalRange.x, spawnIntervalRange.y)), "timeout");

        SpawnEnemy(enemyCount, rng);
    }
}
