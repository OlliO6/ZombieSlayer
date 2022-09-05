using System;
using System.Collections.Generic;
using Additions;
using Explanations;
using Godot;

public class ExplanationsManager : CanvasLayer
{
    public static ExplanationsManager instance;
    public static bool disableExplanations;
    public static Explanation currentExplanation;

    public override void _EnterTree()
    {
        instance = this;
    }

    public static bool HasExplanation(string name) => instance.HasNode(name) || (instance.GetTree().CurrentScene.HasNode("Explanations") && instance.GetTree().CurrentScene.GetNode("Explanations").HasNode(name));
    public static void StartExplanation(string name)
    {
        if (disableExplanations) return;

        Debug.LogU(instance, $"Started {name}");

        var newExplanation = instance.GetNodeOrNull<Explanation>(name);
        if (newExplanation is null)
        {
            newExplanation = instance.GetTree().CurrentScene.GetNodeOrNull("Explanations")?.GetNodeOrNull<Explanation>(name);
            if (newExplanation is null) return;
        }
        if (IsInstanceValid(currentExplanation) && currentExplanation.running) currentExplanation.Finish();
        currentExplanation = newExplanation;
        currentExplanation.Beginn();
    }

    public static void ConnectExplanationToSignal(string explanation, Godot.Object emitter, string signal, bool once = true)
    {
        emitter.Connect(signal, instance, nameof(StartExplanation), new() { explanation }, once ? (uint)ConnectFlags.Oneshot : 0);
    }
}