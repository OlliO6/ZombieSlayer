using System.Collections.Generic;
using Additions;
using Godot;

[Tool]
public class DiceScenesContainer : GridContainer
{
    [Signal] public delegate void Interacted(int index);
    [Export] private PackedScene diceSceneFieldScene;

    [Export] public bool interactableFields;
    [Export] public string interactionSignalName = "Interacted";

    private List<PackedScene> scenes = new();

    public List<PackedScene> Scenes
    {
        get => scenes; set
        {
            scenes = value;
            UpdateDiceScenes();
        }
    }

    public void AddScene(PackedScene scene)
    {
        scenes.Add(scene);
        UpdateDiceScenes();
    }

    public void RemoveScene(PackedScene scene)
    {
        scenes.Remove(scene);
        UpdateDiceScenes();
    }

    public void UpdateDiceScenes()
    {
        foreach (DiceSceneField sceneField in GetChildren())
        {
            RemoveChild(sceneField);
            sceneField.QueueFree();
        }

        if (scenes is null || scenes.Count is 0) return;

        foreach (PackedScene scene in scenes)
        {
            DiceSceneField sceneField = diceSceneFieldScene.Instance<DiceSceneField>();
            sceneField.Scene = scene;

            AddChild(sceneField);

            if (interactableFields) sceneField.Connect(interactionSignalName, this, nameof(OnFieldInteracted), new(sceneField.GetIndex()));
        }
    }

    private void OnFieldInteracted(int index) => EmitSignal(nameof(Interacted), index);
}
