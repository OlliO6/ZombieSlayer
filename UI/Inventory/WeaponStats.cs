using Godot;
using System;
using Additions;

public class WeaponStats : Control
{
    #region NameLabel Reference

    private Label storerForNameLabel;
    public Label NameLabel => this.LazyGetNode(ref storerForNameLabel, _NameLabel);
    [Export] private NodePath _NameLabel = "NameLabel";

    #endregion
    #region TypeLabel Reference

    private Label storerForTypeLabel;
    public Label TypeLabel => this.LazyGetNode(ref storerForTypeLabel, _TypeLabel);
    [Export] private NodePath _TypeLabel = "TypeLabel";

    #endregion
    #region StatsLabel Reference

    private Label storerForOtherStatsLabel;
    public Label StatsLabel => this.LazyGetNode(ref storerForOtherStatsLabel, _OtherStatsLabel);
    [Export] private NodePath _OtherStatsLabel = "StatsLabel";

    #endregion

    public void ShowStats(WeaponBase weapon)
    {
        Show();

        NameLabel.Text = weapon.Filename.GetFile().BaseName();

        TypeLabel.Text = $"{GetWeaponType(NameLabel.Text)}";

        StatsLabel.Text = GetWeaponStats(NameLabel.Text, weapon);
    }

    private string GetWeaponType(string weapon)
    {
        switch (weapon)
        {
            case "Pistol" or "Rifle": return "Gun";
        }

        return "Unknown";
    }

    private string GetWeaponStats(string weaponName, WeaponBase weapon)
    {
        System.Text.StringBuilder sb = new();

        switch (weapon)
        {
            case GunBase gun:
                sb.AppendLine($"Shoot Speed: {(1 / gun.AnimationPlayer.GetAnimation("Shoot").Length).ToString("0.0").Replace(',', '.').TrimEnd('0').RStrip(".")}");
                sb.AppendLine($"Damage: {gun.bulletDamage}");
                sb.AppendLine($"Spread: {gun.bulletSpread}");
                break;
        }

        switch (weaponName)
        {
        }

        return sb.ToString();
    }
}
