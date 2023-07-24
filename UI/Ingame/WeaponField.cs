using Additions;
using Godot;
using System;

[Tool]
public class WeaponField : NinePatchRect, ISelectable
{
    [Signal] public delegate void Selected(DiceField from);
    [Signal] public delegate void Deselected(DiceField from);

    public WeaponBase Weapon
    {
        get => _weapon;
        set
        {
            if (IsInstanceValid(Weapon) && Weapon.hasAbility)
                Weapon.ability.Disconnect(nameof(WeaponAbility.GotReady), this, nameof(OnAbilityGotReady));

            _weapon = value;

            if (!IsInstanceValid(Weapon))
            {
                Icon = null;
                AbilityProgressBar.Visible = false;
                return;
            }

            Icon = Weapon.Icon;
            AbilityProgressBar.Visible = Weapon.hasAbility;

            if (Weapon.hasAbility)
                Weapon.ability.Connect(nameof(WeaponAbility.GotReady), this, nameof(OnAbilityGotReady));
        }
    }

    [Export]
    public bool IsSelected
    {
        get => _selected;
        set
        {
            if (value == IsSelected) return;

            _selected = value;

            SelectFrame?.SetDeferred("visible", value);

            EmitSignal(value ? nameof(Selected) : nameof(Deselected), this);
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

    private bool _selected;
    private Texture _icon;
    private Control storerForSelectFrame;
    private TextureRect storerForIconRect;
    private Range storerForAbilityProgressBar;
    private WeaponBase _weapon;
    private WeaponAbility _ability;

    public Control SelectFrame => this.LazyGetNode(ref storerForSelectFrame, "SelectFrame");
    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "Icon");
    public Range AbilityProgressBar => this.LazyGetNode(ref storerForAbilityProgressBar, "AbilityProgressBar");

    public override void _Process(float delta)
    {
        if (IsInstanceValid(Weapon) && Weapon.hasAbility)
        {
            AbilityProgressBar.Value = Weapon.ability.CooldownProgress;
        }
    }

    private void OnAbilityGotReady()
    {
        var tween = CreateTween();
        tween.TweenProperty(AbilityProgressBar, "modulate:a", 1, 0.1f);
        tween.TweenProperty(AbilityProgressBar, "modulate:a", 0.2f, 0.5f);
    }
}
