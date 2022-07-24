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
            SelectFrame?.SetDeferred("visible", value);

            if (value == Selected) return;

            _selected = value;

            EmitSignal(value ? nameof(OnSelected) : nameof(OnDeselected), this);
        }
    }

    [Export]
    public bool Watched
    {
        get => _watched;
        set
        {
            _watched = value;

            GetNode<ColorRect>("ColorRect")?.SetDeferred("modulate", value ? new("747474") : Colors.White);

            if (value) EmitSignal(nameof(OnWatched), this);
        }
    }

    public Dice dice;
    public DiceMenu diceMenu;

    private bool _selected, _watched;

    [Signal] public delegate void OnSelected(DiceField from);
    [Signal] public delegate void OnDeselected(DiceField from);
    [Signal] public delegate void OnWatched(DiceField from);

    #region SelectFrame Reference

    private Control storerForSelectFrame;
    public Control SelectFrame => this.LazyGetNode(ref storerForSelectFrame, "SelectFrame");

    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (diceMenu is not null)
        {
            GD.Print("Connected");
            Connect(nameof(OnSelected), diceMenu, nameof(DiceMenu.OnDiceFieldSelected));
            Connect(nameof(OnDeselected), diceMenu, nameof(DiceMenu.OnDiceFieldDeselected));
            Connect(nameof(OnWatched), diceMenu, nameof(DiceMenu.OnDiceFieldWatched));
        }

        SetTexture();
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseInput && mouseInput.IsPressed() && mouseInput.ButtonIndex is (int)ButtonList.Left)
        {
            if (Watched)
            {
                Selected = !Selected;
                return;
            }

            Watched = true;
        }
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
