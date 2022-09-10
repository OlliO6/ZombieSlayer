namespace Explanations;
using System.Collections.Generic;
using Additions;
using Godot;

public class ShowHideExplanation : Explanation
{
    [Export] private bool pause = true, dontPausePlayer;
    [Export] private float fadeInTime = 0.5f, fadeOutTime = 0.5f;

    protected override void Start()
    {
        if (pause)
        {
            GetTree().Paused = true;
            if (dontPausePlayer)
                Player.currentPlayer.PauseMode = PauseModeEnum.Process;
        }

        if (fadeInTime > 0)
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "modulate:a", 1f, fadeInTime)
                    .From(0f)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.InOut);
        }

        Show();
    }

    protected override void End()
    {
        if (pause)
        {
            GetTree().Paused = false;
            if (dontPausePlayer)
                Player.currentPlayer.PauseMode = PauseModeEnum.Inherit;
        }

        if (fadeOutTime > 0)
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "modulate:a", 0f, fadeOutTime)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.InOut);

            SignalAwaiter awaiter = ToSignal(tween, "finished");
            awaiter.OnCompleted(() =>
            {
                Hide();
            });
            return;
        }

        Hide();
    }
}
