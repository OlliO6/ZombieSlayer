using Godot;
using System;

[Tool]
public class InteractInstruction : ButtonIcons
{
    private bool _isShown;
    private SceneTreeTween _alphaTween;

    [Export] public bool hideWhileInputDisabled = true;
    [Export] public float tweenDuration = 0.5f;

    [Export]
    public bool IsShown
    {
        get => _isShown;
        set
        {
            _isShown = value;
            CallDeferred(nameof(TweenAlpha), value ? 1 : 0);
        }
    }

    public void FadeIn() => IsShown = true;

    public void FadeOut() => IsShown = false;

    private void TweenAlpha(float alpha)
    {
        if (IsInstanceValid(_alphaTween) && _alphaTween.IsValid())
            _alphaTween.Kill();

        _alphaTween = CreateTween();
        _alphaTween.TweenProperty(this, "modulate:a", alpha, tweenDuration);
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        InputManager.InputDisabled += OnInputDisabled;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        InputManager.InputDisabled -= OnInputDisabled;
    }

    private void OnInputDisabled(bool disabled)
    {
        if (!hideWhileInputDisabled || !IsShown)
            return;

        TweenAlpha(disabled ? 0 : 1);
    }
}
