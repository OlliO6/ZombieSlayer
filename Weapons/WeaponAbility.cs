using Additions;
using Godot;
using System;

public class WeaponAbility : Node2D
{
    [Signal] public delegate void GotReady();
    [Signal] public delegate void Used();

    [Export] public float cooldown = 10;

    private float _cooldownTimer;

    private bool _isReady;

    public float CooldownTimer
    {
        get => _cooldownTimer;
        set
        {
            _cooldownTimer = value;
            if (CooldownTimer >= cooldown)
                IsReady = true;
        }
    }

    public bool IsReady
    {
        get => _isReady;
        set
        {
            if (value == IsReady)
                return;

            _isReady = value;
            if (value)
            {
                CooldownProgress = 1;
                EmitSignal(nameof(GotReady));
                return;
            }
            CooldownTimer = 0;
        }
    }

    public float CooldownProgress
    {
        get => Mathf.Clamp(CooldownTimer / cooldown, 0, 1);
        set => CooldownTimer = value * cooldown;
    }

    public WeaponBase Weapon => GetOwner<WeaponBase>();

    public override void _Ready()
    {
        Weapon.Connect(nameof(WeaponBase.Enabled), this, nameof(OnWeaponEnabled));
        Weapon.Connect(nameof(WeaponBase.Disabled), this, nameof(OnWeaponDisabled));
        IsReady = false;
    }

    private void OnWeaponEnabled()
    {
        InputManager.AbilityInputEnded += TryToUse;
    }

    private void OnWeaponDisabled()
    {
        InputManager.AbilityInputEnded -= TryToUse;
    }

    public override void _Process(float delta)
    {
        if (!IsReady)
            CooldownTimer += delta;
    }

    public void TryToUse()
    {
        if (IsReady && !Weapon.disabled)
            Use();
    }

    public virtual void Use()
    {
        IsReady = false;
        EmitSignal(nameof(Used));
        Debug.LogU(this, "Used.");
    }
}
