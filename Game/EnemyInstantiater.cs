using System.Collections.Generic;
using Additions;
using Godot;
using RandomSceneNodes;

[Tool]
public class EnemyInstantiater : RandomSceneInstantiater
{
    [Export] public int maxEnemyCount = -1;
    [Export] public Vector2 timeBetweenSpawnignRange;
}
