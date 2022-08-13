namespace RandomSceneNodes;
using Godot;

[Tool]
public class plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        AddCustomType("RandomSceneInstantiater", "Node", GD.Load<Script>("res://addons/Random Scene Node/RandomSceneInstantiater.cs"), null);
        AddCustomType("RandomScene", "Node", GD.Load<Script>("res://addons/Random Scene Node/RandomScene.cs"), null);
    }
    public override void _ExitTree()
    {
        RemoveCustomType("RandomSceneInstantiater");
        RemoveCustomType("RandomScene");
    }
}
