using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;
using Leveling;

public class IngameUI : Control
{
    [Export] private PackedScene heartScene;
    [Export] public int maxHealth;
    [Export] public int currentHealth;

    private CoinDisplay storerForCoinLabel;
    private Label storerForLevelLabel;
    private Control storerForWeaponContainer, storerForHeartContainer;
    private ProgressBar storerForXpProgress;
    private AnimationPlayer storerForLevelUpAnim;

    private Texture emptyHeart = GD.Load<Texture>("res://UI/Ingame/EmptyHeart.png");
    private Texture fullHeart = GD.Load<Texture>("res://UI/Ingame/FullHeart.png");

    public CoinDisplay CoinsDisplay => this.LazyGetNode(ref storerForCoinLabel, "%CoinDisplay");
    public Control WeaponContainer => this.LazyGetNode(ref storerForWeaponContainer, "%WeaponContainer");
    public Control HeartContainer => this.LazyGetNode(ref storerForHeartContainer, "%HeartContainer");
    public ProgressBar XpProgress => this.LazyGetNode(ref storerForXpProgress, "%XpProgress");
    public Label LevelLabel => this.LazyGetNode(ref storerForLevelLabel, "%LevelLabel");

    [TroughtSignal]
    private void OnPlayerHealthChanged()
    {
        maxHealth = GetOwner<Player>().MaxHealth;
        currentHealth = GetOwner<Player>().CurrentHealth;
        UpdateHealthDisplay();
    }

    [TroughtSignal]
    private void OnCoinsAmountChanged(int amount)
    {
        CoinsDisplay.ChangeCoinsAmount(amount);
    }

    [TroughtSignal]
    private void OnLevelChanged()
    {
        LevelingSystem leveling = GetOwner<Player>().Leveling;
        LevelLabel.Text = leveling.CurrentLevelIndex.ToString();

        // Hide hearts when on lvl 0
        HeartContainer.Visible = leveling.CurrentLevelIndex is 0 ? false : true;
    }

    private void UpdateHealthDisplay()
    {
        AdjustChildCount();

        for (int i = 0; i < HeartContainer.GetChildCount(); i++)
        {
            HeartContainer.GetChild<TextureRect>(i).Texture = (i >= currentHealth) ? emptyHeart : fullHeart;
        }

        void AdjustChildCount()
        {
            int childCount = HeartContainer.GetChildCount();

            if (childCount == maxHealth) return;

            if (childCount > maxHealth)
            {
                for (int i = maxHealth; i < childCount; i++)
                {
                    HeartContainer.GetChild(i).QueueFree();
                }
                return;
            }

            if (childCount < maxHealth)
            {
                for (int i = 0; i < maxHealth - childCount; i++)
                {
                    HeartContainer.AddChild(heartScene.Instance());
                }
                return;
            }
        }
    }

    [TroughtSignal]
    private void OnWeaponChanged(int index)
    {
        List<WeaponBase> weapons = Player.currentPlayer.GetWeapons().ToList();

        for (int i = 0; i < WeaponContainer.GetChildCount(); i++)
        {
            WeaponField field = WeaponContainer.GetChild<WeaponField>(i);

            if (i >= weapons.Count)
            {
                field.Visible = false;
                continue;
            }

            WeaponBase weapon = weapons[i];

            field.Visible = true;
            field.IsSelected = i == index;

            if (weapon is null)
            {
                field.Icon = null;
                continue;
            }

            field.Icon = weapon.icon;
        }
    }

    public override void _Process(float delta)
    {
        XpProgress.Value = GetOwner<Player>().Leveling.interpolatedXp;
    }
}
