namespace Diadot.Actions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Additions;
using Godot;

public class Signal : DialogAction
{
    [Signal] public delegate void Executed();

    public override Task Execute()
    {
        EmitSignal(nameof(Executed));
        return Task.CompletedTask;
    }
}
