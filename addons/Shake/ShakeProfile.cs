namespace Shaking;
using System;
using Godot;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(ShakeProfile))]
public class ShakeProfile : Resource
{
    [Export] public float amplitude = 0.3f, time = 0.15f, speed = 0;
    [Export] public Tween.EaseType easeType = Tween.EaseType.In;
    [Export] public Tween.TransitionType transitionType = Tween.TransitionType.Sine;

    public event Action<ShakeProfile> Finished;
    public bool inUse;
    public float localAmpFactor;

    private Godot.Object bound;
    private float amount;

    public float ShakeAmount => amount * amplitude * localAmpFactor;
    public float FrequencySummand => amount * speed;

    public void Bound(Godot.Object bound) => this.bound = bound;

    public void StartInterpolate(float ampFactor = 1, float timeFactor = 1)
    {
        if (inUse) return;

        localAmpFactor = ampFactor;

        SceneTreeTween tween = null;

        switch (bound)
        {
            case Node node:
                tween = node.CreateTween();
                break;

            case SceneTree tree:
                tween = tree.CreateTween();
                break;

            default:
                GD.PushError("Can't start interpolation of Shaking.ShakeProfile if there its not bound to a Godot.Node or a Godot.SceneTree");
                return;
        }

        inUse = true;

        tween.TweenProperty(this, nameof(amount), 0f, time * timeFactor)
                .From(1f)
                .SetEase(easeType)
                .SetTrans(transitionType);

        tween.Connect("finished", this, nameof(OnTweenFinished));
    }

    private void OnTweenFinished()
    {
        inUse = false;
        Finished?.Invoke(this);
    }
}