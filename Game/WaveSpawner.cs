using Godot;
using System;
using Additions;

public class WaveSpawner : Node
{
    [Export] private NodePath spawnPositionsHolder;
    [Export] private float minWaveInterval;
    [Export] private Vector2 waveIntervalMultiplierRange = new Vector2(1.8f, 2.5f), spawnIntervalRange = new Vector2(0.1f, 0.5f);
    [Export] private float enemyCountMultiplier = 3, enemyCountPower = 0.8f;
    [Export] private PackedScene[] enemies;

    [Signal] public delegate void OnWaveStarted();

    #region WaveTimer Reference

    private Timer storerForWaveTimer;
    public Timer WaveTimer => this.LazyGetNode(ref storerForWaveTimer, "WaveTimer");

    #endregion

    #region SpawnTimer Reference

    private Timer storerForSpawnTimer;
    public Timer SpawnTimer => this.LazyGetNode(ref storerForSpawnTimer, "SpawnTimer");

    #endregion

    public int currentWave;

    private RandomNumberGenerator rng = new();

    private int enemyCount;

    [TroughtSignal]
    private void StartWave()
    {
        rng.Randomize();

        currentWave++;
        EmitSignal(nameof(OnWaveStarted));

        enemyCount = Mathf.RoundToInt(Mathf.Pow(currentWave * enemyCountMultiplier, enemyCountPower));

        WaveTimer.Start((enemyCount * rng.RandfRange(waveIntervalMultiplierRange.x, waveIntervalMultiplierRange.y) + minWaveInterval));
        SpawnTimer.Start(rng.RandfRange(spawnIntervalRange.x, spawnIntervalRange.y));
    }

    [TroughtSignal]
    private void SpawnEnemy()
    {
        enemyCount--;

        Node2D enemy = enemies[rng.RandiRange(0, enemies.Length - 1)].Instance<Node2D>();
        GetParent().AddChild(enemy);

        Node spawnPositions = GetNode(spawnPositionsHolder);

        enemy.GlobalPosition = spawnPositions.GetChild<Node2D>(rng.RandiRange(0, spawnPositions.GetChildCount() - 1)).GlobalPosition;

        if (enemyCount <= 0) return;

        SpawnTimer.Start(rng.RandfRange(spawnIntervalRange.x, spawnIntervalRange.y));
    }
}
