namespace Diadot;

using System;

public interface IDialogProvider
{
    public string Text { get; }
    public string Name { get; }
    public event Action<IDialogProvider> Ended;
    public void OnTextFinished();
    public void OnTextAdvanced();
}