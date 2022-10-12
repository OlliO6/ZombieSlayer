using System.Collections.Generic;
using Additions;
using Godot;

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
    #region StatLabelContainer Reference

    private Container storerForLabelContainer;
    public Container StatLabelContainer => this.LazyGetNode(ref storerForLabelContainer, _LabelContainer);
    [Export] private NodePath _LabelContainer = "StatLabelContainer";

    #endregion

    private Font font = GD.Load<Font>("res://UI/Fonts/PondeSmall.tres");

    public void ShowStats(WeaponBase weapon)
    {
        Show();

        NameLabel.Text = weapon.Filename.GetFile().BaseName();

        TypeLabel.Text = GetWeaponType(NameLabel.Text);

        ShowWeaponStats(NameLabel.Text, weapon);
    }

    private string GetWeaponType(string weapon)
    {
        switch (weapon)
        {
            case "Pistol" or "Rifle": return "Gun";
            case "Dolphin" or "Orca": return "Load Gun";
            case "Dagger": return "Sword";
        }

        return "Unknown";
    }

    private void ShowWeaponStats(string weaponName, WeaponBase weapon)
    {
        Dictionary<string, object> stats = new();
        Dictionary<string, string> tooltips = new();

        RemoveLabels();

        switch (weapon)
        {
            case ShootingWeaponBase gun:
                AddGunBaseStats(gun);
                break;

            case MeleeBase sword:
                AddSwordBaseStats(sword);
                break;
        }

        switch (weaponName)
        {
        }

        AddLabels();

        void AddGunBaseStats(ShootingWeaponBase gun)
        {
            LoadGun loadGun = gun as LoadGun;
            float timeBetweenShots = gun.AnimationPlayer.GetAnimation("Shoot").Length + (loadGun is null ? 0 : loadGun.loadTime);
            float bulletsPerSecond = 1 / timeBetweenShots;

            stats.Add("Shoot Speed", bulletsPerSecond);
            stats.Add("Damage", gun.GetBulletDamage());
            // stats.Add("Spread", loadGun is null ? gun.bulletSpread : (loadGun.spreadOverPower.InterpolateBaked(1) * loadGun.bulletSpread));
            stats.Add("DPS", (int)stats["Damage"] * bulletsPerSecond);
            tooltips.Add("DPS", "Best possible damage per second");
        }
        void AddSwordBaseStats(MeleeBase sword)
        {
            float swingsPerSecond = 1 / sword.AnimationPlayer.GetAnimation("Attack").Length;
            stats.Add("Attack Speed", swingsPerSecond);
            stats.Add("Damage", sword.GetDamageAmount());
            stats.Add("DPS", (int)stats["Damage"] * swingsPerSecond);
            tooltips.Add("DPS", "Best possible damage per second");
        }



        void RemoveLabels()
        {
            foreach (Label label in StatLabelContainer.GetChildren())
                label.QueueFree();
        }
        void AddLabels()
        {
            foreach (KeyValuePair<string, object> stat in stats)
            {
                Label label = new()
                {
                    Text = $"{stat.Key}: {StatToString(stat.Value)}",
                    Align = Label.AlignEnum.Center,
                    MouseFilter = MouseFilterEnum.Stop,
                    HintTooltip = tooltips.ContainsKey(stat.Key) ? tooltips[stat.Key] : ""
                };

                label.Set("custom_fonts/font", font);

                StatLabelContainer.AddChild(label);
            }
        }

        string StatToString(object stat)
        {
            switch (stat)
            {
                case string _string: return _string;
                case float _float: return FloatToFormattedString(_float);
            }

            return stat.ToString();
        }
    }

    private string FloatToFormattedString(float value) => value.ToString("0.0").Replace(',', '.').TrimEnd('0').RStrip(".");
}
