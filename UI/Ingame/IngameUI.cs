using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

[Tool]
public class IngameUI : Control
{
    [Export] private PackedScene heartScene;
    [Export] public int maxHealth;
    [Export] public int currentHealth;

    private Label storerForCoinLabel, storerForLevelLabel;
    private Control storerForWeaponContainer, storerForHeartContainer;
    private ProgressBar storerForXpProgress;

    private Texture emptyHeart = GD.Load<Texture>("res://UI/Ingame/EmptyHeart.png");
    private Texture fullHeart = GD.Load<Texture>("res://UI/Ingame/FullHeart.png");

    [Export]
    private bool Updade { get => false; set => CallDeferred(nameof(UpdateHealthDisplay)); }

    public Label CoinLabel => this.LazyGetNode(ref storerForCoinLabel, "%CoinDisplay/Label");
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
        CoinLabel.Text = amount.ToString();
    }

    [TroughtSignal]
    private void OnLevelChanged()
    {
        Leveling leveling = GetOwner<Player>().Leveling;
        LevelLabel.Text = leveling.CurrentLevelIndex.ToString();
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
