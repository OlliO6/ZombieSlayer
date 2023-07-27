using Godot;
using System.Linq;
using Additions;
using System;

[Tool]
public class DiceField : NinePatchRect, ISelectable
{
    [Export]
    public bool IsSelected
    {
        get => _selected;
        set
        {
            SelectFrame?.SetDeferred("visible", value);

            if (value == IsSelected) return;

            _selected = value;

            EmitSignal(value ? nameof(Selected) : nameof(Deselected), this);
        }
    }

    [Export] public bool allowDeselect;
    [Export] public bool autoSelect;

    public Dice dice;

    private bool _selected, _watched;

    [Signal] public delegate void Selected(DiceField from);
    [Signal] public delegate void Deselected(DiceField from);

    private Control storerForSelectFrame;
    public Control SelectFrame => this.LazyGetNode(ref storerForSelectFrame, "SelectFrame");

    public override void _Ready()
    {
        SetTexture();
        GetNode<ColorRect>("ColorRect").SelfModulate = (dice is null ? true : dice.broken) ? new("747474") : Colors.White;
        Connect("focus_entered", this, nameof(OnFocusEntered));
    }

    private void OnFocusEntered()
    {
        if (autoSelect)
            IsSelected = true;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (allowDeselect && (@event.IsActionPressed("ui_accept") || (@event is InputEventMouseButton mouseInput && mouseInput.IsPressed() && mouseInput.ButtonIndex is (int)ButtonList.Left)))
        {
            IsSelected = !IsSelected;
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
