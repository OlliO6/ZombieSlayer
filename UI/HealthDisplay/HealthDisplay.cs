using Godot;
using System;
using Additions;

[Tool]
public class HealthDisplay : Control
{
    [Export] private PackedScene heartScene;
    [Export] private NodePath heartContainer;

    [Export] public int maxHealth;
    [Export] public int currentHealth;

    [Export]
    private bool Updade { get => false; set => CallDeferred(nameof(UpdateDisplay)); }

    [TroughtSignal]
    private void OnPlayerHealthChanged()
    {
        maxHealth = GetOwner<Player>().MaxHealth;
        currentHealth = GetOwner<Player>().CurrentHealth;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Control container = GetNode<Control>(heartContainer);

        AdjustChildCount();

        for (int i = 0; i < container.GetChildCount(); i++)
        {
            container.GetChild<TextureRect>(i).SetShaderParam("currentFrame", new Vector2(i >= currentHealth ? 1 : 0, 0));
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


}
