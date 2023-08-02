using System.Collections.Generic;
using Additions;
using Godot;

[Tool]
public class DiceScenesContainer : GridContainer
{
    [Signal] public delegate void Interacted(int index);
    [Signal] public delegate void LostFocus();

    [Export] private PackedScene diceSceneFieldScene;

    [Export] public bool interactableFields;
    [Export] public string interactionSignalName = nameof(InteractableDiceSceneField.Interacted);

    private List<PackedScene> scenes = new();

    public List<PackedScene> Scenes
    {
        get => scenes;
        set
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
        int? focusedField = null;

        for (int i = 0; i < GetChildCount(); i++)
        {
            var sceneField = GetChild<DiceSceneField>(i);
            if (sceneField.HasFocus())
                focusedField = i;

            sceneField.QueueFree();
        }

        foreach (DiceSceneField child in GetChildren())
            RemoveChild(child);

        if (scenes is null || scenes.Count is 0) return;

        for (int i = 0; i < scenes.Count; i++)
        {
            PackedScene scene = scenes[i];
            DiceSceneField sceneField = diceSceneFieldScene.Instance<DiceSceneField>();
            sceneField.Scene = scene;

            AddChild(sceneField);

            if (focusedField == i)
                sceneField.GrabFocus();

            if (interactableFields) sceneField.Connect(interactionSignalName, this, nameof(OnFieldInteracted), new(sceneField.GetIndex()));
        }

        if (focusedField.HasValue && GetFocusOwner() is null)
            EmitSignal(nameof(LostFocus));
    }

    private void OnFieldInteracted(int index) => EmitSignal(nameof(Interacted), index);
}
