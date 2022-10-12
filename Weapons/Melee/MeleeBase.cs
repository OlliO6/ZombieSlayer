using System.Linq;
using Additions;
using Godot;

public class MeleeBase : WeaponBase
{
    public struct AttackData
    {
        public string animName;
        public int damage;
        public float comboReactTime;
        public float transitionTime;
        public float extraTime;
    }

    public AttackData[] attacks;

    protected float currentExtraTime;
    protected int currentCombo;
    protected SceneTreeTween tween;

    private DamagingArea storerForDamageDealed;

    public AttackData CurrentAttack => attacks[currentCombo];
    public DamagingArea DamageDealer => this.LazyGetNode(ref storerForDamageDealed, "%DamageDealer");
    public virtual int GetDamageAmount(int combo = 0) => Mathf.RoundToInt(attacks[combo].damage * (Player.Exists ? Player.currentPlayer.damageMultiplier : 1));

    protected override void ApplyData()
    {
        base.ApplyData();
        var attacksData = data.Get<Godot.Collections.Array>("Attacks").Cast<Godot.Collections.Dictionary>().ToList();
        attacks = new AttackData[attacksData.Count];

        int defaultDamage = (int)data.GetOrDefault<float>("DefaultDamage", 0);
        float defaultComboReactTime = data.GetOrDefault<float>("DefaultComboReactTime", 0);
        float defaultTransTime = data.GetOrDefault<float>("DefaultTransTime", 0);
        float defaultExtraTime = data.GetOrDefault<float>("DefaultExtraTime", 0);

        for (int i = 0; i < attacks.Length; i++)
        {
            attacks[i] = new AttackData()
            {
                animName = attacksData[i].Get<string>("AnimName"),
                damage = (int)attacksData[i].GetOrDefault<float>("Damage", defaultDamage),
                comboReactTime = attacksData[i].GetOrDefault<float>("ComboReactTime", defaultComboReactTime),
                transitionTime = attacksData[i].GetOrDefault<float>("TransTime", defaultTransTime),
                extraTime = attacksData[i].GetOrDefault<float>("ExtraTime", defaultExtraTime)
            };
        }
    }

    protected override void AttackInputStarted()
    {
        if (!isAttacking) Attack();
    }

    public override void Attack()
    {
        isAttacking = true;

        if (tween is not null && currentCombo < attacks.Length - 1 && tween.GetTotalElapsedTime() < CurrentAttack.comboReactTime + currentExtraTime)
        {
            currentCombo++;
            if (currentCombo > attacks.Length)
                currentCombo = 0;
        }
        else
            currentCombo = 0;

        DamageDealer.DamageAmount = GetDamageAmount();
        AnimationPlayer.Stop();
        AnimationPlayer.Play(CurrentAttack.animName);
        tween?.Kill();

        EmitSignal(nameof(AttackStarted));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (isAttacking) return;

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

    protected override void OnAnimationFinished(string animation)
    {
        if (animation == CurrentAttack.animName)
        {
            currentExtraTime = CurrentAttack.extraTime;
            OnAttackFinished();
        }
    }

    protected override async void OnAttackFinished()
    {
        tween = CreateTween();
        Animation reset = AnimationPlayer.GetAnimation("RESET");

        tween.SetParallel(true)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);

        for (int i = 0; i < reset.GetTrackCount(); i++)
        {
            string[] path = ((string)reset.TrackGetPath(i)).Split(':');

            tween.TweenProperty(GetNode(path[0]), path[1], reset.TrackGetKeyValue(i, 0), CurrentAttack.transitionTime);
        }

        if (currentExtraTime != 0) await new TimeAwaiter(this, currentExtraTime);
        isAttacking = false;
        EmitSignal(nameof(AttackFinished));
    }
}
