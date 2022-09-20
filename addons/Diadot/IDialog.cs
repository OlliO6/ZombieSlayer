namespace Diadot;

using System;
using System.Threading.Tasks;
using Godot;

public interface IDialogProvider
{
    public string Name { get; }
    public string Text { get; }
    public string Character { get; }
    public event Action<IDialogProvider, NodePath> Ended;
    public event Action<IDialogProvider> DialogChanged;
    public string[] GetOptions();
    public void ProcessOptionPress(string option);
    public Task ProcessUnhandeledExpression(string expression);
    public void OnTextFinished();
    public void OnTextAdvanced();
    public void OnStarted();
    public void Finish();
}