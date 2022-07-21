using Godot;
using System.Collections.Generic;
using System.Linq;
using Additions;

[Tool]
public class IngameUI : Control
{
    [Export] private PackedScene heartScene;
    [Export] private NodePath heartContainer, coinLabel, weaponFields;

    [Export] public int maxHealth;
    [Export] public int currentHealth;

    [Export]
    private bool Updade { get => false; set => CallDeferred(nameof(UpdateHealthDisplay)); }

    private Texture emptyHeart = GD.Load<Texture>("res://UI/Ingame/EmptyHeart.png");
    private Texture fullHeart = GD.Load<Texture>("res://UI/Ingame/FullHeart.png");

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
        GetNode<Label>(coinLabel).Text = amount.ToString();
    }

    private void UpdateHealthDisplay()
    {
        Control container = GetNode<Control>(heartContainer);

        AdjustChildCount();

        for (int i = 0; i < container.GetChildCount(); i++)
        {
            container.GetChild<TextureRect>(i).Texture = (i >= currentHealth) ? emptyHeart : fullHeart;
        }

        void AdjustChildCount()
        {
            int childCount = container.GetChildCount();

            if (childCount == maxHealth) return;

            if (childCount > maxHealth)
            {
                for (int i = maxHealth; i < childCount; i++)
                {
                    container.GetChild(i).QueueFree();
                }
                return;
            }

            if (childCount < maxHealth)
            {
                for (int i = 0; i < maxHealth - childCount; i++)
                {
                    container.AddChild(heartScene.Instance());
                }
                return;
            }
        }
    }

    [TroughtSignal]
    private void OnWeaponChanged(int index)
    {
        List<WeaponBase> weapons = Player.currentPlayer.GetWeapons().ToList();

        Control container = GetNode<Control>(weaponFields);

        for (int i = 0; i < container.GetChildCount(); i++)
        {
            WeaponField field = container.GetChild<WeaponField>(i);

            if (i >= weapons.Count)
            {
                field.Visible = false;
                continue;
            }

            WeaponBase weapon = weapons[i];

            field.Visible = true;
            field.Selected = i == index;

            if (weapon is null)
            {
                field.Icon = null;
                continue;
            }

            field.Icon = weapon.icon;
        }
    }
}
