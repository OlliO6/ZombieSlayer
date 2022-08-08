#if DEBUG
namespace Additions.Debugging;
using Godot;

public class Watcher : Label
{
    [Export] public Godot.Object target;
    [Export] public string property;
    [Export] private bool autoRemove, showTargetName;
    [Export] public bool customColor;
    [Export] public string optionalName;
    //TODO dasuidias
    public Watcher() { }

    public Watcher(Godot.Object target, string property, bool autoRemove = true, bool showTargetName = true, Color? color = null, string optionalName = "")
    {
        this.target = target;
        this.property = property;
        this.autoRemove = autoRemove;
        this.showTargetName = showTargetName;
        this.optionalName = optionalName;

        Modulate = DebugOverlay.GetTargetColor(target, color, false);
    }

    public override void _Ready()
    {
        Align = AlignEnum.Center;
    }

    public override void _Process(float delta)
    {
        if (!IsInstanceValid(target))
        {
            if (autoRemove)
            {
                QueueFree();
                return;
            }

            Text = "Target instance is not valid";
            return;
        }

        object value = target.GetIndexed(property);

        string name = optionalName is "" ? DebugOverlay.GetTargetName(target) : optionalName;
        if (DebugOverlay.instance is not null) name = DebugOverlay.instance.removeNameUniqeness ? DebugOverlay.RemoveNameUniqeness(name) : name;

        Text = $"{(showTargetName ? $"{name}:" : null)}{(optionalName is null or "" ? property : optionalName)}={(value is null ? "null" : value)}";
    }
}
#endif