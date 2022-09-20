namespace Diadot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Additions;
using Godot;

public class CallAction : Action
{
    [Export] private NodePath node;
    [Export] private string method;
    [Export] private Godot.Collections.Array args;

    public override Task Execute()
    {
        Debug.LogU(this, $"Executed {GetNode(node)}");

        GetNode(node).Callv(method, args);
        return Task.CompletedTask;
    }
}
