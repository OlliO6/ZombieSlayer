using Godot;
using System;
using System.Threading.Tasks;

public class Transitions : CanvasLayer
{
    public const string TransitionBlackFade = "BlackFade";
    public const string TransitionPixel = "Pixel";
    public const string TransitionBlocks = "Blocks";
    public const string TransitionHorizontal = "Horizontal";

    private const string StartAnimSuffix = "_start";
    private const string EndAnimSuffix = "_end";

    public static Transitions Instance { get; private set; }

    public AnimationPlayer Anim => GetNode<AnimationPlayer>(nameof(AnimationPlayer));

    public override void _EnterTree()
    {
        Instance = this;
    }

    public static async void StartTransition(string transitionType, float speed, Action onFinish, bool pause = true, bool disableInput = true)
    {
        if (pause)
            Instance.GetTree().Paused = true;

        if (disableInput)
            Instance.GetTree().Root.GuiDisableInput = true;

        Instance.GetNode<CanvasItem>(TransitionBlackFade).Visible = transitionType == TransitionBlackFade;
        Instance.GetNode<CanvasItem>(TransitionPixel).Visible = transitionType == TransitionPixel;
        Instance.GetNode<CanvasItem>(TransitionBlocks).Visible = transitionType == TransitionBlocks;
        Instance.GetNode<CanvasItem>(TransitionHorizontal).Visible = transitionType == TransitionHorizontal;

        Instance.Anim.Play(transitionType + StartAnimSuffix, customSpeed: speed);
        await Instance.ToSignal(Instance.Anim, "animation_finished");
        await Instance.ToSignal(Instance.GetTree(), "idle_frame");

        onFinish?.Invoke();
    }

    public static void StartTransition(string transitionType, Action onFinish, bool pause = true, bool disableInput = true)
    {
        StartTransition(transitionType, 1, onFinish, pause, disableInput);
    }

    public static async void EndTransition(string transitionType, float speed, Action onFinish = null, bool unpause = true, bool unpauseAtStart = false)
    {
        if (unpauseAtStart && unpause)
            Instance.GetTree().Paused = false;

        Instance.GetNode<CanvasItem>(TransitionBlackFade).Visible = transitionType == TransitionBlackFade;
        Instance.GetNode<CanvasItem>(TransitionPixel).Visible = transitionType == TransitionPixel;
        Instance.GetNode<CanvasItem>(TransitionBlocks).Visible = transitionType == TransitionBlocks;
        Instance.GetNode<CanvasItem>(TransitionHorizontal).Visible = transitionType == TransitionHorizontal;

        Instance.Anim.Play(transitionType + EndAnimSuffix, customSpeed: speed);
        await Instance.ToSignal(Instance.Anim, "animation_finished");

        if (!unpauseAtStart && unpause)
            Instance.GetTree().Paused = false;

        Instance.GetTree().Root.GuiDisableInput = false;
        Instance.GetNode<CanvasItem>(transitionType).Visible = false;
        onFinish?.Invoke();
    }

    public static void EndTransition(string transitionType, Action onFinish = null, bool unpause = true, bool unpauseAtStart = false)
    {
        EndTransition(transitionType, 1, onFinish, unpause, unpauseAtStart);
    }
}
