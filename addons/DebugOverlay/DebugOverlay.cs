namespace Additions.Debugging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

[LogName("Debug")]
public class DebugOverlay : CanvasLayer
{
#if DEBUG
    public static DebugOverlay instance;
    public static Queue<string> outputLines = new();

    [Export] public bool turnOffCommonLogs;
    [Export] public bool removeNameUniqeness;

    private bool fpsShown = true;
    private bool overlayIsOpen, isConsoleOpen;
    private bool gameWasPaused;

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
    #region Menu Reference

    private DebugMenu storerForMenu;
    public DebugMenu Menu => this.LazyGetNode(ref storerForMenu, _Menu);
    [Export] private NodePath _Menu = "Menu";

    #endregion
    #region Console Reference

    private Console storerForConsole;
    public Console Console => this.LazyGetNode(ref storerForConsole, _Console);
    [Export] private NodePath _Console = "Console";

    #endregion

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

    public override void _Ready()
    {
#if TOOLS
        AddOutputLine(ColorizeText($"{ProjectSettings.GetSetting("application/config/name")} <Debug Editor> started", Color.ColorN("white", 0.5f)), true);
#else
        AddOutputLine(ColorizeText($"{ProjectSettings.GetSetting("application/config/name")} <Debug Export> started", Color.ColorN("white", 0.5f)), true);
#endif
    }

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _ExitTree()
    {
        if (instance == this) instance = null;
        Save();
    }
    public void Save()
    {
        bool prevMenuVisibiliy = Menu.Visible;
        bool prevConsoleVisibiliy = Console.Visible;

        Menu.Visible = false;
        Console.Visible = false;

        PackedScene scene = new();
        scene.Pack(this);
        ResourceSaver.Save(Filename, scene);

        Menu.Visible = prevMenuVisibiliy;
        Console.Visible = prevConsoleVisibiliy;
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsPressed()) return;
        if (@event is not InputEventKey keyInput) return;

        if (keyInput.Scancode is (uint)KeyList.Escape)
        {
            if (isConsoleOpen)
            {
                if (Console.isReading)
                {
                    Console.CancelReading();
                    return;
                }
                CloseConsole();
                return;
            }
            if (overlayIsOpen)
            {
                HideMenu();
                return;
            }
            return;
        }

        if (keyInput.Scancode is (uint)KeyList.D && keyInput.Alt)
        {
            if (overlayIsOpen)
                HideMenu();
            else
                ShowMenu();
            return;
        }

        if (keyInput.Scancode is (uint)KeyList.C && keyInput.Alt)
        {
            if (isConsoleOpen)
                CloseConsole();
            else
                OpenConsole();
        }
    }

    internal static void RefreshOutput() => instance.Console.RefreshOutput();
    internal static void AddOutputLine(string line, bool uncommon, float msec = -1) => outputLines.Enqueue($"{(uncommon ? "#UNCOMMON#:" : "#COMMON#:")}{(msec != -1 ? $"#TIME:{System.TimeSpan.FromMilliseconds(Time.GetTicksMsec()).ToString(@"mm\:ss")}#" : "")}{line}");
    // Weird string fotmatting so that its not doing a , instead of a . in deutschland :{
    internal static string ColorizeText(string what, Color color) => $"[tint r={color.r.ToString(System.Globalization.CultureInfo.InvariantCulture)} g={color.g.ToString(System.Globalization.CultureInfo.InvariantCulture)} b={color.b.ToString(System.Globalization.CultureInfo.InvariantCulture)} a={color.a.ToString(System.Globalization.CultureInfo.InvariantCulture)}]{what}[/tint]";

    public void ShowMenu()
    {
        if (overlayIsOpen) return;

        overlayIsOpen = true;
        if (!isConsoleOpen) gameWasPaused = GetTree().Paused;
        GetTree().Paused = true;
        Menu.Show();
        Menu.LoadState();
    }
    public void HideMenu()
    {
        if (!overlayIsOpen) return;

        overlayIsOpen = false;
        if (!gameWasPaused && !isConsoleOpen) GetTree().Paused = false;
        Menu.stepCancellation.Cancel();
        Menu.Hide();
        Save();
    }
    public void OpenConsole()
    {
        if (isConsoleOpen) return;

        isConsoleOpen = true;
        if (!overlayIsOpen) gameWasPaused = GetTree().Paused;
        GetTree().Paused = true;
        Console.Show();
        Console.Setup();
    }
    public void CloseConsole()
    {
        if (!isConsoleOpen) return;

        isConsoleOpen = false;
        if (!gameWasPaused && !overlayIsOpen) GetTree().Paused = false;
        Console.Hide();
        Save();
    }

    internal void AddWatcher(Godot.Object target, NodePath property, bool autoRemove, bool showTargetName, Color? color, string optionalName)
    {
        Labels.AddChild(new Watcher(target, property, autoRemove, showTargetName, color, optionalName));
    }
    internal void RemoveWatcher(Godot.Object target, NodePath property)
    {
        IEnumerable<Watcher> matchingWatchers = Labels.GetChildren<Watcher>().Where((Watcher watcher) => watcher.target == target && watcher.property == property);

        foreach (Watcher watcher in matchingWatchers)
        {
            watcher.QueueFree();
        }
    }
    internal void RemoveWatchersWithTarget(Godot.Object target)
    {
        IEnumerable<Watcher> matchingWatchers = Labels.GetChildren<Watcher>().Where((Watcher watcher) => watcher.target == target);

        foreach (Watcher watcher in matchingWatchers)
        {
            watcher.QueueFree();
        }
    }

    internal async void LogT(Godot.Object target, string message, float time, bool uncommon, Color? color, string optionalName, bool alsoPrint)
    {

        string name = optionalName is "" ? GetTargetName(target) : optionalName;
        string output = $"{(target is null ? "" : ($"{(name is "" or null ? "Thing" : removeNameUniqeness ? RemoveNameUniqeness(name) : name)}: "))}{message}";
        Color outputColor = GetTargetColor(target, color, uncommon);

        AddOutputLine(ColorizeText(output, outputColor), uncommon, Time.GetTicksMsec());

        if (turnOffCommonLogs && !uncommon) return;

        Label label = new Label() { Text = output };

        if (alsoPrint) GD.Print($"[{System.TimeSpan.FromMilliseconds(Time.GetTicksMsec()).ToString(@"mm\:ss")}] {label.Text}");

        Logs.AddChild(label);
        label.Modulate = outputColor;

        CreateTween()
            .SetTrans(Tween.TransitionType.Expo)
            .SetEase(Tween.EaseType.In)
            .TweenProperty(label, "modulate:a", 0f, time);

        await new TimeAwaiter(this, time);

        label.QueueFree();
    }

    internal async void LogFrame(Godot.Object target, string message, bool physicsFrame, int frames, Color? color, string optionalName, bool bottomLetf, bool alsoPrint)
    {
        string name = optionalName is "" ? GetTargetName(target) : optionalName;

        Label label = new Label()
        {
            Text = $"{(target is null ? "" : ($"{(name is "" or null ? "Thing" : removeNameUniqeness ? RemoveNameUniqeness(name) : name)}: "))}{message}"
        };

        if (alsoPrint) GD.Print(label.Text);

        (bottomLetf ? Logs : Labels).AddChild(label);
        label.Align = bottomLetf ? Label.AlignEnum.Left : Label.AlignEnum.Center;
        label.Modulate = GetTargetColor(target, color, false);

        for (int i = 0; i < frames; i++) await ToSignal(GetTree(), physicsFrame ? "physics_frame" : "idle_frame");

        label.QueueFree();
    }

    internal static string GetTargetName(Godot.Object target)
    {
        LogNameAttribute logNameAttribute = target.GetType().GetCustomAttribute<LogNameAttribute>();

        if (logNameAttribute is not null) return logNameAttribute.name;
        if (target is Node node) return node.Name;
        if (target is Resource resource) return resource.ResourceName;
        return target.GetType().Name;
    }
    internal static string RemoveNameUniqeness(string name)
    {
        if (!name.StartsWith("@")) return name;

        string result;

        result = name.Remove(name.FindLast("@"));
        result = result.Remove(0, 1);

        return result;
    }
    internal static Color GetTargetColor(Godot.Object target, Color? color, bool uncommon)
    {
        if (color is null)
        {
            if (target is not null)
            {
                foreach (var attribute in target.GetType().GetCustomAttributes(true))
                {
                    if (attribute is DefaultColorAttribute colorAttribute)
                        return uncommon ? colorAttribute.unommonColor : colorAttribute.commonColor;
                }
            }
            return uncommon ? Colors.LightCyan : Colors.White;
        }
        return color.Value;
    }
#else
    public override void _Ready() => QueueFree();
#endif
}