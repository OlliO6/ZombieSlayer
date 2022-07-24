using Additions;
using Godot;
using System;

[Tool]
public class WeaponField : NinePatchRect, ISelectable
{
    [Export]
    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;

            SelectFrame?.SetDeferred("visible", value);

            EmitSignal(value ? nameof(OnSelected) : nameof(OnDeselected), this);
        }
    }
    [Export]
    public Texture Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            IconRect?.SetDeferred("texture", value);
        }
    }

    public int Index => GetIndex();

    [Signal] public delegate void OnSelected(DiceField from);
    [Signal] public delegate void OnDeselected(DiceField from);

    private bool _selected;
    private Texture _icon;

    #region SelectFrame Reference

    private Control storerForSelectFrame;
    public Control SelectFrame => this.LazyGetNode(ref storerForSelectFrame, "SelectFrame");

    #endregion

    #region IconRect Reference

    private TextureRect storerForIconRect;
    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "Icon");

    #endregion
}
