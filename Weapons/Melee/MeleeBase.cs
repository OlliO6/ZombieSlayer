using Additions;
using Godot;

public class MeleeBase : WeaponBase
{
    public int[] comboDamageLevels;
    public int maxComboCount;
    public float afterAttackTransitionTime = 1, comboTime = 0.3f;
    public string[] comboAnims;
    public float[] extraTime;

    protected float currentExtraTime;
    protected int currentCombo;
    protected SceneTreeTween tween;

    #region DamageDealee Reference

    private DamagingArea storerForDamageDealed;
    public DamagingArea DamageDealer => this.LazyGetNode(ref storerForDamageDealed, "%DamageDealer");

    #endregion

    public virtual int GetDamageAmount(int combo = 0) => Mathf.RoundToInt(comboDamageLevels[combo.Clamp(0, comboDamageLevels.Length - 1)] * (Player.currentPlayer is null ? 1 : Player.currentPlayer.damageMultiplier));

    protected override void AttackInputStarted()
    {
        if (!isAttacking) Attack();
    }

    public override void Attack()
    {
        isAttacking = true;

        if (tween is not null && tween.GetTotalElapsedTime() < comboTime + currentExtraTime)
        {
            currentCombo++;
            if (currentCombo > maxComboCount || currentCombo >= comboAnims.Length)
                currentCombo = 0;
        }
        else
            currentCombo = 0;

        DamageDealer.DamageAmount = GetDamageAmount();
        AnimationPlayer.Stop();
        AnimationPlayer.Play(comboAnims[currentCombo]);
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
        if (animation == comboAnims[currentCombo])
        {
            currentExtraTime = currentCombo < extraTime.Length ? extraTime[currentCombo] : 0;
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

            tween.TweenProperty(GetNode(path[0]), path[1], reset.TrackGetKeyValue(i, 0), afterAttackTransitionTime);
        }

        if (currentExtraTime != 0) await new TimeAwaiter(this, currentExtraTime);
        isAttacking = false;
        EmitSignal(nameof(AttackFinished));
    }
}
