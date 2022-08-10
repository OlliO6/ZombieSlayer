using System.Collections.Generic;
using Additions;
using Godot;

public class VisibilityDisabler : VisibilityNotifier2D
{
    [Export] private bool showAndHide = true, disableProcess, disablePhysicsProcess, recursive;

    [Export] private NodePath optionalTarget = null;

    private Node2D target;
    private List<Node> allChilds = new();

    public Node2D Target
    {
        get => target;
        set
        {
            target = value;

            if (showAndHide)
            {
                Connect("screen_entered", Target, "show");
                Connect("screen_exited", Target, "hide");
            }

            if (disableProcess || disablePhysicsProcess)
            {
                Connect("screen_entered", this, nameof(EnableProcess));
                Connect("screen_exited", this, nameof(DisableProcess));
            }

            if (!IsOnScreen()) EmitSignal("screen_exited");
        }
    }

    public override void _Ready()
    {
        Target = optionalTarget is null ? GetParent() as Node2D : GetNode<Node2D>(optionalTarget);
    }

    private void DisableProcess()
    {
        if (disableProcess) Target.SetProcess(false);
        if (disablePhysicsProcess) Target.SetPhysicsProcess(false);

        if (!recursive) return;

        allChilds.Clear();
        this.GetAllChildren<Node>(ref allChilds);

        foreach (Node node in allChilds)
        {
            if (disableProcess) node.SetProcess(false);
            if (disablePhysicsProcess) node.SetPhysicsProcess(false);
        }
    }
    private void EnableProcess()
    {
        if (disableProcess) Target.SetProcess(true);
        if (disablePhysicsProcess) Target.SetPhysicsProcess(true);

        if (!recursive) return;

        foreach (Node node in allChilds)
        {
            if (disableProcess) node.SetProcess(true);
            if (disablePhysicsProcess) node.SetPhysicsProcess(true);
        }
    }
}
