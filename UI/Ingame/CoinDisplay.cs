using System.Collections.Generic;
using Additions;
using Godot;

public class CoinDisplay : HBoxContainer
{
    public int coinsDisplayed;
    public int targetCoins;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, _AnimationPlayer);
    [Export] private NodePath _AnimationPlayer = "AnimationPlayer";

    #endregion
    #region CoinLabel Reference

    private Label storerForCoinLabel;
    public Label CoinLabel => this.LazyGetNode(ref storerForCoinLabel, _CoinLabel);
    [Export] private NodePath _CoinLabel = "CoinLabel";

    #endregion

    public override void _Ready()
    {
        AnimationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
    }

    public void ChangeCoinsAmount(int coins)
    {
        targetCoins = coins;

        IncreaseCoins();
    }

    private void CenterLabel()
    {
        CoinLabel.RectPivotOffset = RectSize * 0.5f;
    }

    private void OnAnimationFinished(string anim)
    {
        IncreaseCoins();
    }

    private void IncreaseCoins()
    {
        if (coinsDisplayed >= targetCoins) return;

        coinsDisplayed++;
        CoinLabel.Text = coinsDisplayed.ToString();

        CallDeferred(nameof(CenterLabel));
        AnimationPlayer.Play("Collect");
    }
}
