using Godot;
using Additions;
using System.Collections.Generic;

public class VisibilityDisabler : VisibilityNotifier2D
{
    [Export] private bool showAndHide = true, disableProcess, disablePhysicsProcess, recursive;

    [Export] private NodePath optionalTarget = null;

    public Node2D target;

    private List<Node> allChilds = new();


    public override void _Ready()
    {
        target = optionalTarget is null ? Owner as Node2D : GetNode<Node2D>(optionalTarget);

        if (showAndHide)
        {
            Connect("screen_entered", target, "show");
            Connect("screen_exited", target, "hide");
        }

        if (disableProcess || disablePhysicsProcess)
        {
            Connect("screen_entered", this, nameof(EnableProcess));
            Connect("screen_exited", this, nameof(DisableProcess));
        }

        if (!IsOnScreen()) EmitSignal("screen_exited");
    }

    private void DisableProcess()
    {
        if (disableProcess) target.SetProcess(false);
        if (disablePhysicsProcess) target.SetPhysicsProcess(false);

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
        if (disableProcess) target.SetProcess(true);
        if (disablePhysicsProcess) target.SetPhysicsProcess(true);

        if (!recursive) return;

        foreach (Node node in allChilds)
        {
            if (disableProcess) node.SetProcess(true);
            if (disablePhysicsProcess) node.SetPhysicsProcess(true);
        }
    }
}
