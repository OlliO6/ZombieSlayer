using Godot;
using System;

public class Watcher : Label
{
    [Export] public Godot.Object target;
    [Export] public string property;
    [Export] private bool autoRemove, showTargetName;
    [Export] public string optionalName;

    public Watcher() { }

    public Watcher(Godot.Object target, string property, bool autoRemove = true, bool showTargetName = true, string optionalName = "")
    {
        this.target = target;
        this.property = property;
        this.autoRemove = autoRemove;
        this.showTargetName = showTargetName;
        this.optionalName = optionalName;
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

        object value = target.Get(property);

        Text = $"{(showTargetName ? $"{GetTargetName()}:" : null)}{(optionalName is null or "" ? property : optionalName)}={(value is null ? "null" : value)}";
    }

    private string GetTargetName()
    {
        if (target is Node node) return node.Name;
        if (target is Resource resource) return resource.ResourceName;
        return "";
    }
}
