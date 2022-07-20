using Additions;
using Godot;
using System;

[Tool]
public class WeaponField : NinePatchRect
{
    [Export]
    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            SelectFrame.SetDeferred("visible", value);
        }
    }
    [Export]
    public Texture Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            IconRect.SetDeferred("texture", value);
        }
    }

    private bool _selected;
    private Texture _icon;

    public int Index => GetIndex();

    #region SelectFrame Reference

    private Control storerForSelectFrame;
    public Control SelectFrame => this.LazyGetNode(ref storerForSelectFrame, "SelectFrame");

    #endregion

    #region IconRect Reference

    private TextureRect storerForIconRect;
    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "Icon");

    #endregion

    public void SetWeapon(WeaponBase weapon)
    {
        Icon = weapon.icon;
    }
}
