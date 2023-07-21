using Additions;
using Godot;
using System;

public class WeaponAbility : Node2D
{
    [Export] public float cooldown = 10;

    private float cooldownTimer;

    public float CooldownProgress
    {
        get => Mathf.Clamp(cooldownTimer / cooldown, 0, 1);
        set => cooldownTimer = value * cooldown;
    }

    public WeaponBase Weapon => GetOwner<WeaponBase>();

    public override void _Process(float delta)
    {
        if (cooldownTimer < cooldown)
            cooldownTimer += delta;
    }

    public virtual void Invoke()
    {
        cooldownTimer = 0;
        Debug.LogU(this, "Invoked.");
    }
}
