using Godot;
using System.Collections.Generic;
using System.Linq;
using Additions;

public class DebugOverlay : CanvasLayer
{
    public static DebugOverlay instance;

    private bool fpsShown = true;

    [Export]
    public bool IsShowingFps
    {
        get => fpsShown;
        set
        {
            fpsShown = value;

            if (Labels is null)
            {
                ToSignal(this, "ready").OnCompleted(() =>
                {
                    Labels.GetNode<CanvasItem>("FPSLabel").Visible = value;
                });
                return;
            }
            Labels.GetNode<CanvasItem>("FPSLabel").Visible = value;
        }
    }


    #region Labels Reference

    private VBoxContainer storerForDebugLabels;
    public VBoxContainer Labels => this.LazyGetNode(ref storerForDebugLabels, "DebugLabels");

    #endregion

    public override void _Ready()
    {
        if (OS.IsDebugBuild())
        {
            instance = this;
            return;
        }

        instance = null;
        QueueFree();
    }

    public static void AddWatcher(Godot.Object target, string property, bool autoRemove = true, bool showTargetName = true, string optionalName = "") => instance?._AddWatcher(target, property, autoRemove, showTargetName, optionalName);
    public static void RemoveWatcher(Godot.Object target, string property) => instance?._RemoveWatcher(target, property);
    public static void RemoveWatchersWithTarget(Godot.Object target) => instance?._RemoveWatchersWithTarget(target);

    private void _AddWatcher(Godot.Object target, string property, bool autoRemove, bool showTargetName, string optionalName)
    {
        Labels.AddChild(new Watcher(target, property, autoRemove, showTargetName, optionalName));
    }
    private void _RemoveWatcher(Godot.Object target, string property)
    {
        IEnumerable<Watcher> matchingWatchers = Labels.GetChildren<Watcher>().Where((Watcher watcher) => watcher.target == target && watcher.property == property);

        foreach (Watcher watcher in matchingWatchers)
        {
            watcher.QueueFree();
        }
    }
    private void _RemoveWatchersWithTarget(Godot.Object target)
    {
        IEnumerable<Watcher> matchingWatchers = Labels.GetChildren<Watcher>().Where((Watcher watcher) => watcher.target == target);

        foreach (Watcher watcher in matchingWatchers)
        {
            watcher.QueueFree();
        }
    }
}
