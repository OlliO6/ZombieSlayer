using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;
using RandomSceneNodes;

[Tool]
public class DifficultyLevel : Node
{
    public EnemyInstantiater Instantiater => GetChild<EnemyInstantiater>(0);

    public virtual float GetTimeToNextSpawn() => Random.FloatRange(Instantiater.timeBetweenSpawnignRange);

    public override string _GetConfigurationWarning()
    {
        if (GetChildCount() is 0 || GetChild(0) is not EnemyInstantiater)
            return "First child must be an EnemyInstantiater";

        if (this.GetChildren<EnemyInstantiater>().Last().Forever is false)
            return "The last instantiater must be forever";

        return "";
    }
}
