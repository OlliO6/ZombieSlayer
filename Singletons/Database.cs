using System;
using Additions;
using Godot;
using Godot.Collections;

public class Database : Node
{
    public Database() => LoadData();

    private const string WeaponDataJsonPath = "res://Data/WeaponData.json";

    public static Dictionary weaponData;


    public static void LoadData()
    {
        File weaponDataFile = new();
        weaponDataFile.Open(WeaponDataJsonPath, File.ModeFlags.Read);
        weaponData = JSON.Parse(weaponDataFile.GetAsText()).Result as Dictionary;
    }
}