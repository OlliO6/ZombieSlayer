namespace Diadot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Additions;
using Godot;

public class SignalAction : Action
{
    [Signal] public delegate void Executed();

    public override Task Execute()
    {
        EmitSignal(nameof(Executed));
        return Task.CompletedTask;
    }
}
