namespace Diadot.Actions;

using System.Collections.Generic;
using System.Threading.Tasks;
using Additions;
using Godot;

public class Call : DialogAction
{
    [Export] private NodePath node;
    [Export] private string method;
    [Export] private Godot.Collections.Array args;

    public override Task Execute()
    {
        GetNode(node).Callv(method, args);
        return Task.CompletedTask;
    }
}
