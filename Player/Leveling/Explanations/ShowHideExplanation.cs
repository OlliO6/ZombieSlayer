using System.Collections.Generic;
using Additions;
using Godot;

public class ShowHideExplanation : Explanation
{
    [Export] private bool pause = true;
    [Export] private float fadeInTime = 0, fadeOutTime = 0;

    protected override void Start()
    {
        if (pause) GetTree().Paused = true;

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
        if (pause) GetTree().Paused = false;

        if (fadeOutTime > 0)
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "modulate:a", 0f, fadeOutTime)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.InOut);

            ToSignal(tween, "finished")
                    .OnCompleted(Hide);
            return;
        }

        Hide();
    }
}
