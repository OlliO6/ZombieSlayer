using Godot;
using System.Linq;
using Additions;

[Tool]
public class DiceField : NinePatchRect
{
    [Export]
    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            SelectFrame.SetDeferred("visible", value);

            if (value) EmitSignal(nameof(ShowDiceScenes), dice);
        }
    }

    public Dice dice;

    private bool _selected;

    [Signal] public delegate void ShowDiceScenes(Dice from);

    #region SelectFrame Reference

    private Control storerForSelectFrame;
    public Control SelectFrame => this.LazyGetNode(ref storerForSelectFrame, "SelectFrame");

    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        DiceContainer diceContainer = GetParentOrNull<DiceContainer>();

        if (diceContainer is not null)
        {
            Connect(nameof(ShowDiceScenes), diceContainer, nameof(DiceContainer.ShowDiceScenes));
        }

        SetTexture();
    }

    private void SetTexture()
    {
        TextureRect icon = GetNode<TextureRect>("TextureRect");

        if (icon.Material is not ShaderMaterial material || !material.Shader.HasParam("frames") || !material.Shader.HasParam("currentFrame"))
            return;

        Vector2 frames = (Vector2)icon.GetShaderParam("frames");

        if (dice is null) AssignRandomNumber();
        else AssignDiceSceneCount();

        void AssignRandomNumber()
        {
            RandomNumberGenerator rng = new();
            rng.Randomize();
            icon.SetShaderParam("currentFrame", new Vector2(
                x: rng.RandiRange(0, Mathf.RoundToInt(frames.x - 1)),
                y: rng.RandiRange(0, Mathf.RoundToInt(frames.y - 1))
                ));
        }

        void AssignDiceSceneCount()
        {
            int count = dice.scenes.Count((PackedScene scene) => scene is not null);

            icon.SetShaderParam("currentFrame", new Vector2(
                x: Mathf.CeilToInt(count * 0.5f) - 1,
                y: count % 2 == 0 ? 1 : 0
                ));
        }
    }
}
