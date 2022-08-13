namespace RandomSceneNodes;
using System.Collections.Generic;
using System.Linq;
using Godot;

[Tool]
public class RandomSceneInstantiater : Node
{
    [Export] public int freeAfter;
    private bool forever = true;
    [Export] private uint Seed { get => (uint)rng.Seed; set => rng.Seed = value; }
    [Export]
    public bool Forever
    {
        get => forever;
        set
        {
            forever = value;
            GetParent()?.UpdateConfigurationWarning();
        }
    }

    public RandomNumberGenerator rng = new() { Seed = 0 };

    public override void _EnterTree()
    {
        if (Seed == 0 && !Engine.EditorHint)
            rng.Randomize();
    }

    public T Instantiate<T>() where T : class
    {
        #region Local Methods

        RandomScene GetChooseScene(IEnumerable<RandomScene> randomScenes, int prioIndex)
        {
            int currentPrioIndex = 0;

            foreach (RandomScene randScene in randomScenes)
            {
                currentPrioIndex += randScene.priority;

                if (prioIndex <= currentPrioIndex)
                {
                    return randScene;
                }
            }

            return null;
        }
        int GetTotalPriority(IEnumerable<RandomScene> randomScenes)
        {
            int totalPriority = 0;

            foreach (RandomScene randScene in randomScenes)
            {
                totalPriority += randScene.priority;
            }

            return totalPriority;
        }

        #endregion

        IEnumerable<RandomScene> randomScenes = GetChildren().OfType<RandomScene>();

        int totalPriority = GetTotalPriority(randomScenes);

        int prioIndex = rng.RandiRange(1, totalPriority);

        RandomScene randomScene = GetChooseScene(randomScenes, prioIndex);

        if (randomScene is null)
        {
            GD.PrintErr("Choosing random scene went wrong for some reason.");
            return null;
        }

        Node instance = randomScene.Instantiate();

        if (!Forever)
        {
            freeAfter--;
            if (freeAfter is 0) QueueFree();
        }

        return (T)(object)instance;
    }

    public override string _GetConfigurationWarning()
    {
        if (GetChildren().OfType<RandomScene>().Count() is 0) return "Should have at least one RandomScene as child";

        return "";
    }
}