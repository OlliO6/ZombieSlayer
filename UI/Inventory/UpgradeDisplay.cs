using Godot;
using System;
using Additions;

[Tool]
public class UpgradeDisplay : HBoxContainer
{
    [Export] public CSharpScript upgradeType;

    [Export]
    public Texture Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            IconRect?.SetDeferred("texture", value);
        }
    }

    private Texture _icon;

    #region IconRect Reference

    private TextureRect storerForIconRect;
    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "Icon");

    #endregion

    #region AmountLabel Reference

    private Label storerForAmountLabel;
    public Label AmountLabel => this.LazyGetNode(ref storerForAmountLabel, "AmountLabel");

    #endregion

    public void SetAmount(int amount) => AmountLabel.Text = amount.ToString();

    public override void _Ready()
    {
        HintTooltip = upgradeType.Call("GetDescription") as string;
    }
}
