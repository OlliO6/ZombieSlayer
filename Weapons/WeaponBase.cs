using System;
using Additions;
using Godot;

public abstract class WeaponBase : Node2D
{
    [Signal] public delegate void AttackStarted();
    [Signal] public delegate void AttackFinished();

    [Export] public string weaponName = "";

    public Godot.Collections.Dictionary data;
    public PackedScene weaponPickupScene;
    public Texture Icon => Icons.IconPickupMatrix.ContainsKey(weaponPickupScene) ? Icons.IconPickupMatrix[weaponPickupScene] : null;

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    public bool disabled;
    public bool isAttacking;

    protected virtual void ApplyData()
    {
        data = Database.weaponData[weaponName] as Godot.Collections.Dictionary;
        weaponPickupScene = GD.Load<PackedScene>(data.Get<string>("PickupPath"));
    }

    public override void _Ready()
    {
        ApplyData();
        AnimationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
    }

    public override void _ExitTree()
    {
        Disable();
    }

    public virtual void Disable()
    {
        InputManager.AttackInputStarted -= AttackInputStarted;
        InputManager.AttackInputEnded -= AttackInputEnded;
        Visible = false;
        disabled = true;
    }
    public virtual void Enable()
    {
        InputManager.AttackInputStarted += AttackInputStarted;
        InputManager.AttackInputEnded += AttackInputEnded;
        Visible = true;
        disabled = false;
    }

    public override void _Process(float delta)
    {
        if (InputManager.attackInput && !disabled) AttackInputProcess(delta);
    }

    protected virtual void AttackInputStarted() { }
    protected virtual void AttackInputEnded() { }
    protected virtual void AttackInputProcess(float delta) { }
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
