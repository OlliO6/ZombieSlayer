namespace RandomSceneNodes;
using System.Collections.Generic;
using System.Linq;
using Godot;

[Tool]
public class RandomScene : Node
{
    [Export] public PackedScene scene;
    [Export] public int priority = 1;

#if TOOLS
    [Export]
    private bool PrintChances
    {
        get => false;
        set
        {
            int GetTotalPriority(IEnumerable<RandomScene> randomScenes)
            {
                int totalPriority = 0;

                foreach (RandomScene randScene in randomScenes)
                {
                    totalPriority += randScene.priority;
                }

                return totalPriority;
            }

            if (value is false)
                return;

            IEnumerable<RandomScene> randomScenes = GetParent().GetChildren().OfType<RandomScene>();
            GD.Print($"{priority} / {GetTotalPriority(randomScenes)}");
        }
    }
#endif

    public virtual Node Instantiate()
    {
        return scene.Instance();
    }
}