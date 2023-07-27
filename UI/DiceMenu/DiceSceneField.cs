using System;
using Additions;
using Godot;

[Tool]
public class DiceSceneField : NinePatchRect
{
    [Export]
    public PackedScene Scene
    {
        get => _scene;
        set
        {
            _scene = value;

            if (value is null)
            {
                CallDeferred(nameof(Deactivate));
                return;
            }

            if (Icons.IconPickupMatrix.ContainsKey(value)) CallDeferred(nameof(SetIcon), Icons.IconPickupMatrix[value]);
        }
    }

    private PackedScene _scene;

    private void SetIcon(Texture icon)
    {
        IconRect.Show();
        IconRect.Texture = icon;
        Modulate = new Color(Colors.White);
    }

    private void Deactivate()
    {
        IconRect.Hide();
        Modulate = new Color(Colors.White, 0.5f);
    }

    private TextureRect storerForIconRect;
    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "Icon");

    // public DescriptionContainer DescriptionContainer => GetNode<DescriptionContainer>("DescriptionContainer");

    // public override void _Ready()
    // {
    //     DescriptionContainer.text = Utils.GetDescriptionForScene(Scene);
    // }

    [TroughtEditor]
    private void OnFocusEntered()
    {
        // DescriptionContainer.Show();
    }

    [TroughtEditor]
    private void OnFocusExited()
    {
        // DescriptionContainer.Hide();
    }
}
