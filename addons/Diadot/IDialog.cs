namespace Diadot;

using System;
using Godot;

public interface IDialogProvider
{
    public string Text { get; }
    public string Name { get; }
    public event Action<IDialogProvider, NodePath> Ended;
    public event Action<IDialogProvider> DialogChanged;
    public void OnTextFinished();
    public void OnTextAdvanced();
    public void OnStarted();
    public void Finish();
}