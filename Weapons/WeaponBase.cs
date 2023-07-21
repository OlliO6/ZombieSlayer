using System;
using Additions;
using Godot;
using Godot.Collections;

public abstract class WeaponBase : Node2D
{
    [Signal] public delegate void AttackStarted();
    [Signal] public delegate void AttackFinished();

    [Export] public string weaponName = "";

    public bool hasAbility;

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
        var ability = data.GetOrDefault<Dictionary>("Ability", null);
    }

    public override void _Ready()
    {
        ApplyData();
        AnimationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
    }

    public override void _ExitTree() => Disable();

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

    public virtual void RotateWeapon()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        LookAt(mousePos);

        if (GlobalPosition.x > mousePos.x)
        {
            Scale = new Vector2(-1, 1);
            Rotate(Mathf.Deg2Rad(180));
            return;
        }
        Scale = new Vector2(1, 1);
    }

    public override void _PhysicsProcess(float delta)
    {
        RotateWeapon();
    }

    public override void _Process(float delta)
    {
        if (InputManager.attackInput && !disabled)
            AttackInputProcess(delta);
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
