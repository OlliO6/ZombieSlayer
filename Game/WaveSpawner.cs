using Additions;
using Godot;

public class WaveSpawner : Node
{
    [Export] private NodePath spawnPositionsHolder;
    [Export] private float startDelay, minWaveInterval;
    [Export] private Vector2 waveIntervalMultiplierRange = new Vector2(1.8f, 2.5f), spawnIntervalRange = new Vector2(0.1f, 0.5f);
    [Export] private float enemyCountMultiplier = 3, enemyCountPower = 0.8f;
    [Export] private PackedScene[] enemies;

    [Signal] public delegate void WaveStarted();

    public int currentWave;

    private int enemyCount;

    public override void _Ready()
    {
        new TimeAwaiter(this, startDelay,
                onCompleted: () => StartWave());
    }

    private void StartWave()
    {
        currentWave++;
        EmitSignal(nameof(WaveStarted));

        int targetEnemyCount = Mathf.RoundToInt(Mathf.Pow(currentWave * enemyCountMultiplier, enemyCountPower));

        SpawnEnemies(targetEnemyCount);

        new TimeAwaiter(this, targetEnemyCount * Random.FloatRange(waveIntervalMultiplierRange.x, waveIntervalMultiplierRange.y) + minWaveInterval,
                onCompleted: () => { StartWave(); });
    }

    private async void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await new TimeAwaiter(this, Random.FloatRange(spawnIntervalRange.x, spawnIntervalRange.y),
                    onCompleted: () => { SpawnEnemy(); });
        }
    }

    [TroughtSignal]
    private void SpawnEnemy()
    {
        enemyCount++;

        Node2D enemy = enemies[Random.IntRange(0, enemies.Length - 1)].Instance<Node2D>();
        GetParent().AddChild(enemy);

        enemy.Connect("Died", this, nameof(OnEnemyDied));

        Node spawnPositions = GetNode(spawnPositionsHolder);

        enemy.GlobalPosition = spawnPositions.GetChild<Node2D>(Random.IntRange(0, spawnPositions.GetChildCount() - 1)).GlobalPosition;
    }

    [TroughtSignal]
    private void OnEnemyDied()
    {
        enemyCount--;
        GD.Print(enemyCount);
    }
}
