using System;
using Additions;
using Godot;
using Godot.Collections;

public abstract class WeaponBase : Node2D
{
    [Signal] public delegate void AttackStarted();
    [Signal] public delegate void AttackFinished();
    [Signal] public delegate void Enabled();
    [Signal] public delegate void Disabled();

    [Export] public string weaponName = "";

    public bool hasAbility;
    public WeaponAbility ability;

    public Godot.Collections.Dictionary data;
    public PackedScene weaponPickupScene;
    public Texture Icon => Icons.IconPickupMatrix.ContainsKey(weaponPickupScene) ? Icons.IconPickupMatrix[weaponPickupScene] : null;

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    public bool disabled;
    public bool isAttacking;

    protected virtual void ApplyData()
    {
        data = Database.weaponData[weaponName] as Dictionary;
        weaponPickupScene = data.Get<PackedScene>("PickupScene");
        hasAbility = data.GetOrDefault<bool>("HasAbility", false);
        if (hasAbility)
            ability = GetNode<WeaponAbility>("Ability");
    }

    public override void _Ready()
    {
        ApplyData();
        AnimationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
    }

    public override void _ExitTree() => Disable();

    public virtual void Disable()
    {
        Visible = false;
        disabled = true;
        EmitSignal(nameof(Disabled));
    }
    public virtual void Enable()
    {
        Visible = true;
        disabled = false;
        EmitSignal(nameof(Enabled));
    }

    public virtual void RotateWeapon() => InputManager.RotateWeapon(this, true);

    public override void _PhysicsProcess(float delta)
    {
        RotateWeapon();
    }

    public virtual void AttackInputStarted() { }
    public virtual void AttackInputEnded() { }
    public virtual void AttackInputProcess(float delta) { }
    public virtual void Attack() { isAttacking = true; EmitSignal(nameof(AttackStarted)); }
    protected virtual void OnAnimationFinished(string animation)
    {
        if (animation is "Attack")
        {
            isAttacking = false;
            OnAttackFinished();
        }
    }
    protected virtual void OnAttackFinished() { EmitSignal(nameof(AttackFinished)); }
}
