using Godot;
using System;

public static class Utils
{
    public static string GetDescriptionForScene(this PackedScene scene)
    {
        if (scene is null) return "";

        SceneState sceneState = scene.GetState();

        int propCount = sceneState.GetNodePropertyCount(0);

        if (propCount > 1 && sceneState.GetNodePropertyValue(0, 1) is CSharpScript script)
        {
            return script.Call("GetDescription") as string;
        }

        return "";
    }
}
