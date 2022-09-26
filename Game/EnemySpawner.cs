using System.Collections.Generic;
using System.Linq;
using Additions;
using Enemies;
using Godot;

public class EnemySpawner : Node
{
    [Signal] public delegate void WaveStarted();

    #region SpawnPositionsHolder Reference

    private Node storerForSpawnPositionsHolder;
    public Node SpawnPositionsHolder => this.LazyGetNode(ref storerForSpawnPositionsHolder, _SpawnPositionsHolder);
    [Export] private NodePath _SpawnPositionsHolder = "SpawnPositionsHolder";

    #endregion

    public DifficultyLevel difficultyLevel;
    private int enemyCount, difficultyIndex;

    private float timeToNextSpawn;

    private List<Node2D> _spawnPositions;
    private List<Node2D> SpawnPositions => SpawnPositionsHolder.LazyGetChildren(ref _spawnPositions, false);

    public int DifficultyIndex
    {
        get => difficultyIndex;
        set
        {
            difficultyIndex = value;
            DifficultyLevel newLevel = GetNodeOrNull<DifficultyLevel>($"Level{difficultyIndex}");
            Debug.LogU(this, newLevel is null);
            if (newLevel is null) return;
            difficultyLevel = newLevel;
            timeToNextSpawn = difficultyLevel.GetTimeToNextSpawn();
        }
    }

    public override void _Ready()
    {
        Player.currentPlayer?.Connect(nameof(Player.LevelChanged), this, nameof(OnLevelChanged));

        this.AddWatcher(nameof(enemyCount));
    }

    private void OnLevelChanged(int to)
    {
        DifficultyIndex = to;
        Debug.Log(this, $"Difficulty changed to {difficultyLevel.Name}");
    }

    public override void _Process(float delta)
    {
        if (difficultyLevel is null || (enemyCount >= difficultyLevel.Instantiater.maxEnemyCount && difficultyLevel.Instantiater.maxEnemyCount is not -1)) return;

        timeToNextSpawn -= delta;

        if (timeToNextSpawn < 0)
        {
            SpawnEnemy();
            timeToNextSpawn = difficultyLevel.GetTimeToNextSpawn();
        }
    }

    private void SpawnEnemy()
    {
        enemyCount++;

        IEnemy enemy = difficultyLevel.Instantiater.Instantiate<IEnemy>();
        enemy.EnemyDied += OnEnemyDied;

        if (enemy is not Node enemyNode) return;
        GetParent().AddChild(enemyNode);

        if (enemy is Node2D enemy2D)
        {
            // Get a position where the enemy can spawn
            if (Player.currentPlayer is null)
            {
                enemy2D.GlobalPosition = SpawnPositions[Random.IntRange(0, SpawnPositionsHolder.GetChildCount() - 1)].GlobalPosition;
                return;
            }
            const float maxDistToPlayer = 90;
            List<Node2D> possiblePositions = SpawnPositions.Where((node) => Player.currentPlayer.GlobalPosition.DistanceTo(node.GlobalPosition) > maxDistToPlayer).ToList();

            enemy2D.GlobalPosition = possiblePositions[Random.IntRange(0, possiblePositions.Count - 1)].GlobalPosition;
            return;
        }
    }

    private void OnEnemyDied(IEnemy enemy)
    {
        enemyCount--;
    }
}
