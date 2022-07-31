using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

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

    private Container storerForLabels;
    public Container Labels => this.LazyGetNode(ref storerForLabels, _Labels);
    [Export] private NodePath _Labels = "Labels";

    #endregion

    #region Logs Reference

    private Container storerForLogs;
    public Container Logs => this.LazyGetNode(ref storerForLogs, _Logs);
    [Export] private NodePath _Logs = "Logs";

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

    public static void Log(Godot.Object target, string message, float time = 2) => instance._LogT(target, message, time);
    public static void LogF(Godot.Object target, string message, int frames = 1, bool bottomLetf = false) => instance._LogF(target, message, frames, bottomLetf);
    public static void LogP(Godot.Object target, string message, int physicFrames = 1, bool bottomLetf = false) => instance._LogP(target, message, physicFrames, bottomLetf);

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

    private async void _LogT(Godot.Object target, string message, float time)
    {
        string name = GetTargetName(target);

        Label label = new Label()
        {
            Text = $"{(target is null ? "" : ($"{(name is "" or null ? "Thing" : name)}: "))}{message}"
        };

        Logs.AddChild(label);
        Tween tween = Logs.GetChild<Tween>(0);
        tween.InterpolateProperty(label, "modulate:a", 1, 0, time, Tween.TransitionType.Expo, Tween.EaseType.In);
        tween.Start();

        await new TimeAwaiter(this, time);

        label.QueueFree();
    }
    private async void _LogF(Godot.Object target, string message, int frames, bool bottomLetf)
    {
        string name = GetTargetName(target);

        Label label = new Label()
        {
            Text = $"{(target is null ? "" : ($"{(name is "" or null ? "Thing" : name)}: "))}{message}"
        };

        (bottomLetf ? Logs : Labels).AddChild(label);

        for (int i = 0; i < frames; i++) await ToSignal(GetTree(), "idle_frame");

        label.QueueFree();
    }
    private async void _LogP(Godot.Object target, string message, int frames, bool bottomLetf)
    {
        string name = GetTargetName(target);

        Label label = new Label()
        {
            Text = $"{(target is null ? "" : ($"{(name is "" or null ? "Thing" : name)}: "))}{message}"
        };

        (bottomLetf ? Logs : Labels).AddChild(label);
        label.Align = bottomLetf ? Label.AlignEnum.Left : Label.AlignEnum.Center;

        for (int i = 0; i < frames; i++) await ToSignal(GetTree(), "physics_frame");

        label.QueueFree();
    }
    private string GetTargetName(Godot.Object target)
    {
        if (target is Node node) return node.Name;
        if (target is Resource resource) return resource.ResourceName;
        return "";
    }
}
